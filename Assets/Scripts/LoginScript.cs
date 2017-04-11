using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoginScript : MonoBehaviour {

	public Text login;
	public InputField password;
	public Text userName;
	public float countDown;
	public GameObject successPanel;
	private bool loggedIn;


	public void onLoginClicked(){
		string login = this.login.text;
		string password = this.password.text;
		string userName = this.userName.text;

		WWWForm form = new WWWForm ();
		form.AddField ("userName", login);
		form.AddField ("password", password);
		form.AddField ("host", "webapp");
		form.AddField ("deviceId", "1");
		form.AddField ("deviceName", "UNITY");
		WWW www = new WWW ("http://api2.iheart.com/api/v1/account/login",form);
		StartCoroutine(Login(www,userName));
	}

	void Update(){
		if (loggedIn && countDown > 0) {
			countDown -= Time.deltaTime;
		}
		if (loggedIn && countDown < 0) {
			SceneManager.LoadScene ("Start");
		}
	}

	IEnumerator Login(WWW www, string userName){
		yield return www;

		if (www.error == null) {
			PlayerPrefs.SetString ("displayName", userName);
			loggedIn = true;
			successPanel.SetActive (true);
			Debug.Log ("success!");
		} else {
			Debug.Log ("failed");
		}
	}
}
