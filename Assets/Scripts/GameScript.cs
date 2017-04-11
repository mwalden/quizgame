using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameScript : NetworkBehaviour {

	private Dictionary<int,string> genreDictionary = new Dictionary<int,string>(){
		{16,"Top 40"},{5,"Country"},{12,"ROCK"},{1,"ALTERNATIVE"},{104,"R & B"},{77,"DANCE"}
	};

	private NewPlayerScript player;
	public GameObject buttonPrefab;
	public Font font;
	public int currentAnswerId;
	//these are being set in the editor 
	//via the game script on the Question Canvas
	public GameObject scoresPanel;
	public GameObject audioPreviewPanel;
	public GameObject gameOverPanel;
	public GameObject timesUpPanel;
	public GameObject answerResponsePanel;
	public Image answerResponseImage;
	public Sprite correctSprite;
	public Sprite incorrectSprite;
	public Text answerResponseText;
	public Text answerResponseSubText;
	public Text questionText;
	public Text subQuestionText;
	public AudioSource source;
	public float previewSeconds;


	private int score;
	private Question audioQuestion;
	private bool previewHasStarted;
	private NetworkIdentity identity;
	private Dictionary<string,int> playersAnswers;
	private Dictionary<string,int> playersScores;
	private string[] letters = {"A","B","C","D","E"};
	private Question currentQuestion;
	private int totalPlayers;
	private List<Question> allQuestions;
	private int currentQuestionIndex;
	private bool timesUp;
	public  CountDownTimer countDownTimer;
	private bool playingAudioPreview;
	private int totalQuestionCount;
	private string genreName;



	//this is used for when they get the wrong answer and we show the panel.
	private string correctAnswerText;
	
	
	void Start () {
		
//		GameObject.FindGameObjectWithTag ("themeTitleAudio").GetComponent<AudioSource>().Stop() ;

		player = PlayerSingleton.Instance.player;
		if (player.gameScript == null) {
			player.setGameScript ();
		}
		identity = this.player.GetComponent<NetworkIdentity> ();
		countDownTimer.StopTimer ();
		resetAnswerDictionary ();
		playersScores = new Dictionary<string, int> ();


	}
//	[ClientRpc]
	IEnumerator Delay(){
		yield return new WaitForSeconds(2	);
	}
//
	bool requested = false;
	bool sendQuestionsToClients = false;
	void Update () {
		if (countDownTimer == null)
			countDownTimer = GetComponentInChildren<CountDownTimer> ();
		if (player.isServer && requested == false) {
			requested = true;
			Delay ();
			RequestQuestionsFromServer ();
		}

		if (sendQuestionsToClients == false && isServer && allQuestions != null) {
			sendQuestionsToClients = true;
			RpcGetQuestions (allQuestions.ToArray());
			RpcSetTotalQuestion (allQuestions.Count,genreDictionary[PlayerSingleton.Instance.genreId]);
		}

		if (currentQuestion.previewUrl != null && currentQuestion.previewUrl != "") {
			if (source.clip != null && source.clip.loadState == AudioDataLoadState.Loaded && !previewHasStarted) {			
				previewHasStarted = true;
//				source.Play ();
			}
			if (previewSeconds > 0 && previewHasStarted) {
				previewSeconds -= Time.deltaTime;
			}
			if (previewSeconds <= 0 && source.clip != null) {
				source.Stop ();
				audioPreviewPanel.SetActive (false);
				source.clip = null;
				LocalLoadQuestion (audioQuestion);
			}
		}
	}

	public void loadQuestion(Question question){
		if (isServer) {
			RpcLoadQuestion (question);
		}
	}

	void LoadAudio(Question question){		
		WWW www = new WWW (question.previewUrl);
		StartCoroutine(getAudio (www,question));
	}

	IEnumerator getAudio(WWW www,Question question){
		yield return www;
		if (www.error == null) {
			audioPreviewPanel.SetActive (true);
			Text[] texts = audioPreviewPanel.GetComponentsInChildren<Text> ();
			texts [0].text = question.question;
			AudioClip clip = www.GetAudioClip (false, true);
			source.clip = clip;
//			currentQuestion = question;
			audioQuestion = question;
			source.Play ();
			previewHasStarted = true;
			Debug.Log ("SUCCESS!");
		} else {
			Debug.Log ("ERRORROROROROROROROR)R)R)R)R)!");
		}
	}


	void LocalLoadQuestion(Question question){
		Debug.Log ("Setting questions on client");

		ClearBoard ();
		correctAnswerText = question.answers[question.answerId-1].answer;

		if (question.previewUrl != "" && previewHasStarted == false) {
			LoadAudio (question);
			return;
		}
		previewSeconds = 10;

		GameObject panel = GameObject.FindGameObjectWithTag ("answers");
		questionText.text = question.question;
		subQuestionText.text = question.subQuestion;

		foreach(Answer answer in question.answers){
			GameObject go = (GameObject)Instantiate (buttonPrefab);

			Button button = go.GetComponent<Button> ();
			AnswerButtonScript answerButtonScript = button.GetComponent<AnswerButtonScript> ();
			answerButtonScript.id = answer.id;
			button.GetComponentInChildren<Text> ().text = letters [answer.id - 1] + ". " + answer.answer;

			button.transform.SetParent (transform, false);
			button.onClick.AddListener (() => {								
				SubmitAnswer (answerButtonScript.id);
			});
			button.transform.SetParent(panel.transform,false);
		}
		previewHasStarted = false;
		ResetTimer ();
	}

	[ClientRpc]
	void RpcLoadQuestion(Question question){
		toggleQuestionsObjects (true);
		LocalLoadQuestion (question);
	}

	void SubmitAnswer(int answerId){
		AnswerButtonScript[] scripts = transform.GetComponentsInChildren<AnswerButtonScript> ();
		foreach (AnswerButtonScript abs in scripts) {
			abs.updateColor (answerId);
		}

		countDownTimer.StopTimer ();
		player.SubmitAnswer (identity.ToString (),answerId);
	}

	public void SetPlayerScore(string playerId, int score){
		if (isServer) {
			RpcSetPlayerScore (playerId, score);
		}
	}

	[ClientRpc]
	void RpcGetQuestions(Question[] questions){
		List<Question> qs = new List<Question>();
		foreach (Question q in questions){
			qs.Add (q);
		}

		this.allQuestions = qs;
		loadQuestion ();
	}

//	public void GivingPlayersQuestions(){
//		RpcGetQuestions (allQuestions);
//	}

	public void CollectPlayerAnswer(string playerId, int answerId){
		if (isServer) {
			setPlayerAnswer(playerId,answerId);
			List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Player"));
			if (playersAnswers.Keys.Count == players.Count) {
				List<string> correctPlayers = new List<string> ();
				foreach (string key in playersAnswers.Keys) {
					if (playersAnswers [key] == currentQuestion.answerId) {
						correctPlayers.Add (key);
					}
				}
				RpcAlertCorrectPlayers (correctPlayers.ToArray());
			}
		}
	}

	[ClientRpc]
	void RpcSetPlayerScore(string playerId, int score){
		playersScores[playerId] = score;
	}



	[ClientRpc]
	void RpcAlertCorrectPlayers(string[] playerIds){
		bool wasCorrect = false;

		foreach (string playerId in playerIds) {			
			if (identity.ToString () == playerId) {
				answerResponseImage.sprite = correctSprite;
				answerResponseText.text = "CORRECT!";
				score += 10;
				answerResponseSubText.text = "Congratulations! You answered correctly!";
				wasCorrect = true;
			}
		}
		if (!wasCorrect) {
			answerResponseText.text = "WRONG ANSWER!";
			answerResponseSubText.text = correctAnswerText;//currentQuestion.answers [currentQuestion.answerId-1].answer;
			answerResponseImage.sprite = incorrectSprite;
		}

//		playAudioFeedback (wasCorrect);

		if (!timesUp)
			answerResponsePanel.gameObject.SetActive (true);
		player.SetScore (identity.ToString (), score);
		StartCoroutine (LoadNextQuestion (1.0f));

	}

	IEnumerator LoadNextQuestion(float amount){
		yield return new WaitForSeconds(amount);
		answerResponsePanel.gameObject.SetActive (false);

		if (isServer) {
			
			resetAnswerDictionary ();
			currentQuestionIndex++;
			if (currentQuestionIndex == allQuestions.Count)
				RpcGameComplete ();			
		}
		if (isServer)
			RpcSetQuestionIndex (currentQuestionIndex);
		showScores ();
	}

	[ClientRpc]
	private void RpcGameComplete(){
		gameOverPanel.SetActive(true);
	}

	public void onQuitGameClicked(){
		if (isServer) {
			NetworkManager.singleton.StopHost ();
		}
		SceneManager.LoadScene ("Start");
	}

	private void toggleQuestionsObjects(bool show){
		questionText.gameObject.SetActive (show);
		subQuestionText.gameObject.SetActive (show);
	}

	private void ClearBoard (){
		GameObject[] answers = GameObject.FindGameObjectsWithTag ("answer");
		foreach (GameObject go in answers) {
			Destroy (go);
		}
	}

	private void resetAnswerDictionary(){
		playersAnswers = new Dictionary<string,int> ();
	}
	private void setPlayerAnswer(string playerId,int answerId){
		if (!playersAnswers.ContainsKey(playerId)){
			playersAnswers.Add (playerId, answerId);
		}
	}
	public void onHideScoresClick(){		
		GameObject []scores = GameObject.FindGameObjectsWithTag ("scoreText");
		foreach (GameObject score in scores) {
			Destroy (score);
		}
	}

	IEnumerator ParseQuestions(WWW www){
		yield return www;
		if (www.error == null) {
			QuestionCollection result = JsonUtility.FromJson<QuestionCollection>(www.text);
			List<Question> questionList = new List<Question> ();
			foreach (QuestionDAO question in result.questions){
				List<Answer> answers = new List<Answer>();
				if (question.answers == null) {
					Debug.Log ("answers are null!!!!!");
					continue;
				}
				foreach (AnswerDAO answer in question.answers) {
					answers.Add(new Answer(answer.id,answer.answer));
				}
				Question qStruct = new Question (question.question, question.subQuestion, question.previewUrl,answers.ToArray (), question.answerId);
				questionList.Add (qStruct);
			}
			allQuestions = questionList;

			loadQuestion ();
		} else {
			Debug.Log ("ERROR FACE!!!");
		}
	}

	[ClientRpc]
	public void RpcSetQuestionIndex(int currentQuestionIndex){
		if (!isServer)
			this.currentQuestionIndex = currentQuestionIndex;
	}

	[ClientRpc]
	public void RpcSetTotalQuestion(int total, string genreName){
		totalQuestionCount = total;
		this.genreName = genreName;

	}
	public void RequestQuestionsFromServer(){
//		Answer [] answers = new Answer[] (){new Answer(1,"123")};
//		List<Answer> answers = new List<Answer>(){new Answer(1,"123")};
//		Question question = new Question ("this is hte quesiotn","","",answers.ToArray(),1);
//		List<Question> questions = new List<Question> ();
//		questions.Add (question);
//		allQuestions = questions;
//		loadQuestion ();
		WWW www = new WWW (PlayerSingleton.Instance.HOST+"/api/games/"+PlayerSingleton.Instance.idOfGame+"/questions");
		StartCoroutine (ParseQuestions(www));
	}

	private void ResetTimer(){
		timesUp = false;
		countDownTimer.ResetTimer ();
		countDownTimer.StartTimer ();
	}

	public void TimesUp(){
		timesUp = true;
		timesUpPanel.SetActive (true);
		StartCoroutine (TimesUpReset ());
	}

	private IEnumerator TimesUpReset(){
		yield return new WaitForSeconds (3);
		//this is to submit a bad answer to kick off the answer flow.
		SubmitAnswer (-1);
		StopAllCoroutines ();
		timesUpPanel.SetActive (false);
	}

	private void showScores(){
		ClearBoard ();
		toggleQuestionsObjects (false);
		List<string> names = new List<string> ();
		foreach (string name in playersScores.Keys) {
			names.Add (name);
		};
		ScoreManager scoreManager = scoresPanel.GetComponent<ScoreManager> ();
		scoreManager.questionProgressText.text = totalQuestionCount - currentQuestionIndex +"/"+ totalQuestionCount + " REMAINING";
		scoreManager.genreText.text = genreName;
		scoreManager.playerNames = names;
		scoreManager.scores = playersScores;
		scoreManager.ShowScores ();
		StartCoroutine(hideScores ());
	}

	private IEnumerator hideScores(){
		yield return new WaitForSeconds (3);
		StopAllCoroutines ();
		ScoreManager scoreManager = scoresPanel.GetComponent<ScoreManager> ();
		scoreManager.HideScores ();
		scoresPanel.SetActive (false);
		toggleQuestionsObjects (true);
		loadQuestion ();
	}

	private void loadQuestion (){
		//this turns off the timer coroutine
		Question question = allQuestions [currentQuestionIndex];
		currentQuestion = question;
		currentAnswerId = question.answerId;
		player.loadQuestion (question);
	}

	public struct Answer
	{
		public int id;
		public string answer;
		public Answer(int id, string answer){
			this.id = id;
			this.answer = answer;
		}
	}
	public struct Question
	{
		public string question;
		public string subQuestion;
		public Answer[] answers;
		public int answerId;
		public string previewUrl;

		public Question(string question, string subQuestion, string previewUrl, Answer[] answers, int answerId){
			this.question = question;
			this.previewUrl = previewUrl;
			this.subQuestion = subQuestion;
			this.answers = answers;
			this.answerId = answerId;
		}
	}
}
