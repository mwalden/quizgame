using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Collections;

public class StartScript : MonoBehaviour {

	public Text welcomeText;
//	public AudioSource beep;

	void Start(){
		welcomeText.text = "WELCOME " + PlayerPrefs.GetString (PlayerSingleton.Instance.DISPLAY_NAME);
	}


	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)){			
			SceneManager.LoadScene ("Title");
		}
	}

	public void StartHost(){
		StartGame (true);
	}

	public void OnStartClientClicked(){
//		beep.Play ();
		PlayerSingleton.Instance.isHost = false;
		PlayerSingleton.Instance.playerName = GetName();

		SceneManager.LoadScene("FindGame");
	}


	public void StartHostingGameClicked(){
		
		StartGame (true);
	}

	void StartGame(bool isHost){
		PlayerSingleton.Instance.isHost = isHost;
		PlayerSingleton.Instance.playerName = GetName();
//		PlayerSingleton.Instance.nameOfGame = GetGameName ();
		SceneManager.LoadScene ("GenreSelector");
	}

	string GetName(){
		return PlayerPrefs.GetString (PlayerSingleton.Instance.DISPLAY_NAME);
//		GameObject go  = GameObject.FindGameObjectWithTag ("nameText");
//		return go.GetComponent<Text> ().text;
	}


}
