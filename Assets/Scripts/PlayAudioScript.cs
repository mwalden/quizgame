using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;
using System.Collections;

public class PlayAudioScript : MonoBehaviour {
	public AudioSource source;
	public bool hasStarted;
	public float seconds;
	public float countDownSeconds;
	public Animator animator;
	public Slider slider;
	public Text timesUp;

	private float inc;

	// Use this for initialization
	void Start () {
		inc = countDownSeconds / 100;
		slider.maxValue = countDownSeconds;
		WWW www = new WWW ("http://listen.vo.llnwd.net/g3/0/1/1/6/6/1211266110.mp3");
		StartCoroutine (getAudio(www));
	}
	
	// Update is called once per frame
	void Update () {
		if (countDownSeconds >= 0) {
			slider.value += Time.deltaTime;
			countDownSeconds -= Time.deltaTime;
		} else {
			timesUp.gameObject.SetActive (true);
		}


		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene ("Title");
		}
		if (source.clip != null && source.clip.loadState == AudioDataLoadState.Loaded && !hasStarted) {			
			source.Play ();
			hasStarted = true;
		}
		if (seconds > 0 && hasStarted) {
			seconds -= Time.deltaTime;
		}
		if (seconds <= 0) {
			source.Stop ();
		}

	}

	IEnumerator getAudio(WWW www){
		yield return www;
		if (www.error == null) {
			AudioClip clip = www.GetAudioClip (false, true);
			source.clip = clip;
			Debug.Log ("SUCCESS!");
		} else {
			Debug.Log ("ERRORROROROROROROROR)R)R)R)R)!");
		}

	}

	IEnumerator FindGames(WWW www){
		yield return www;
		if (www.error == null) {
			Debug.Log ("SUCCESS!");
			Debug.Log (www.text);
			QuestionCollection result = JsonUtility.FromJson<QuestionCollection>(www.text);
			Debug.Log (result.questions [0].question);
		} else {
			Debug.Log ("ERROR FACE!!!");
		}
	}

	public void onClick(){
		Debug.Log ("in here?");
		Debug.Log(PlayerSingleton.Instance.HOST+"/api/games/asdf/questions");
		WWW www = new WWW (PlayerSingleton.Instance.HOST+"/api/games/asdf/questions");
		//WWW www = new WWW ("http://10.89.196.59:3000/games");
		StartCoroutine (FindGames(www));
	}

	IEnumerator WaitForRequest(WWW www){
		yield return www;
		if (www.error == null) {
			Debug.Log ("SUCCESS!!!");
			Debug.Log (www.text);
		} else {
			Debug.Log ("FAILED!!!");
			Debug.Log (www.error);
		}
		StopAllCoroutines ();
	}
}
