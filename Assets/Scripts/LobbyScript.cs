using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LobbyScript : NetworkBehaviour {
	public NewPlayerScript playerScript;
	public Font font;
	public List<string> players;
	public Button startGameButton;
	public Text headerText;
	public Text playerCountText;
	public GameObject playerNameContainer;

	void Start(){
		players = new List<string> ();

	}

	public void SetPlayer(NewPlayerScript playerScript){
		this.playerScript = playerScript;
		if (!playerScript.isHost)
			startGameButton.gameObject.SetActive (false);
		
		playerScript.AlertPlayerJoined (playerScript.playerName);
	}

	public void onStartGameClicked(){
		if (isServer) {
			WWWForm form = new WWWForm ();
			form.AddField ("empty", "string");
			WWW www = new WWW (PlayerSingleton.Instance.HOST+"/api/games/"+PlayerSingleton.Instance.idOfGame+"/start",form);
			StartCoroutine (StartGame(www));

		}
	}
	IEnumerator StartGame(WWW www){
		yield return www;
		if (www.error == null) {
			Debug.Log ("starting game");
			RpcStartGame ();
		} else {
			Debug.Log ("error");
		}

	}

	public void AlertPlayerJoined(string playerName){
		if (isServer) {			
			players.Add (playerName);
			RpcUpdateNames (players.ToArray());
		}
	}
	[ClientRpc]
	void RpcStartGame(){
		NetworkManager.singleton.ServerChangeScene("NewGame");
	}

	[ClientRpc]
	void RpcUpdateNames(string[] playerNames){
		headerText.text = PlayerSingleton.Instance.nameOfGame.ToUpper();
		GameObject names = GameObject.FindGameObjectWithTag ("playerNames");
		playerCountText.text = "Player Count: " + playerNames.Length;
		Text[] namesTexts = names.GetComponentsInChildren<Text> ();
		List<string> existingNames = new List<string> ();
		foreach (Text t in namesTexts) {
			existingNames.Add (t.text);
		}
		foreach(string playerName in playerNames){	
			if(!existingNames.Contains(playerName)){	
				GameObject player = Instantiate (playerNameContainer);
				Text[] texts = player.GetComponentsInChildren<Text> ();
				texts [0].text = playerName;
				texts [1].text = "";
				texts [2].text = "";
				player.transform.SetParent (names.transform, false);
			}
		}
	}

}
