using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;


//probably should move code to set the name into here.
public class GenreMananger : MonoBehaviour {
	public GameObject hostGamePanel;
	public Button buttonPrefab;
	public Canvas genreCanvas;
	public GameObject genrePanel;
	public Text title;
//	public AudioSource beep;

	private GenreCollection genreCollections = new GenreCollection();

	void Start () {
//		GetGenres ();
	}

	void AddListener (Button b, Genre g){
		b.onClick.AddListener (() => onGenreClick (g.id));	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void StartGame(bool isHost){
		PlayerSingleton.Instance.isHost = isHost;
		PlayerSingleton.Instance.nameOfGame = GetGameName ();
	}

	public void onGenreClick(int id){
//		beep.Play ();
		title.text = "NAME YOUR GAME";
		genrePanel.SetActive (false);
		PlayerSingleton.Instance.genreId = id;
		hostGamePanel.SetActive (true);		
	}

	public void StartHostingGame(){

		WWWForm form = new WWWForm ();
		PlayerSingleton.Instance.nameOfGame = GetGameName ();
		form.AddField("name",PlayerSingleton.Instance.nameOfGame);
		form.AddField("ip",Network.player.ipAddress.ToString());
		form.AddField("genreId",PlayerSingleton.Instance.genreId);
		form.AddField("hostId",1);
		form.AddField("started",0);

		WWW www = new WWW (PlayerSingleton.Instance.HOST+"/api/games/",form);
		StartCoroutine(WaitForRequest(www));
	}

	IEnumerator WaitForRequest(WWW www){
		yield return www;

		AvailableGame result = JsonUtility.FromJson<AvailableGame>(www.text);
		PlayerSingleton.Instance.isHost = true;

		StopAllCoroutines();

		if (www.error == null && !www.text.Equals("")) {
			Debug.Log ("SUCCESS!!!");
			PlayerSingleton.Instance.idOfGame = result.id;
			SceneManager.LoadScene ("Lobby");
		} else {
			Debug.Log ("FAILED!!!");
			Debug.Log (www.error);
		}
	}

	IEnumerator WaitForGenres(WWW www){
		yield return www;

		if (www.error == null) {
			genreCollections = JsonUtility.FromJson<GenreCollection> (www.text);
			foreach (Genre g in genreCollections.genres) {
				Button b = Instantiate (buttonPrefab);
				Text title = b.GetComponentInChildren<Text> ();
				title.text = g.name;
				AddListener (b,g);
				b.transform.SetParent (genreCanvas.transform, false);
			}
		}
	}

	private void GetGenres(){
		WWW www = new WWW (PlayerSingleton.Instance.HOST + "/api/genres/all");
		StartCoroutine(WaitForGenres(www));
	}

	string GetGameName(){
		GameObject go = GameObject.FindGameObjectWithTag ("gameNameInput");
		Text name = go.GetComponent<Text> ();
		return name.text;
	}
}
