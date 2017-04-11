using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class NewPlayerScript : NetworkBehaviour {
	public string playerName;
	public bool isHost;


	LobbyScript lobbyScript;
	public GameScript gameScript;

	void Awake(){
		DontDestroyOnLoad (gameObject);
	}

	public override void OnStartLocalPlayer ()
	{
		isHost = PlayerSingleton.Instance.isHost;
		playerName = PlayerSingleton.Instance.playerName;
		gameObject.name = playerName;
		PlayerSingleton.Instance.player = this;
		lobbyScript.SetPlayer (this);
		base.OnStartLocalPlayer ();
	}

	public override void OnStartClient()
	{
		lobbyScript = GameObject.FindObjectOfType<LobbyScript>();
	}

	public void setGameScript(){
		gameScript = GameObject.FindObjectOfType<GameScript> ();
	}

	public void AlertPlayerJoined(string newPlayerName){
		CmdNewPlayerJoined (newPlayerName);
	}
	public void SetScore(string playerId, int score){
		CmdSetScore (playerId, score);
	}


	[Command]
	void CmdSetScore(string playerId, int score){
		if (gameScript == null) {
			gameScript = GameObject.FindObjectOfType<GameScript> ();
		}
		gameScript.SetPlayerScore (playerId, score);
	}

	[Command]
	void CmdNewPlayerJoined(string newPlayerName){
		lobbyScript.AlertPlayerJoined (newPlayerName);
	}

//	
//	public void giveClientsQuestions(){
//		CmdGiveClientsQuestions ();
//	}

//	[Command]
//	void CmdGiveClientsQuestions(){
//		if (gameScript == null) {
//			gameScript = GameObject.FindObjectOfType<GameScript> ();
//		} 
//		gameScript.GivingPlayersQuestions ();
//	}

	public void loadQuestion(GameScript.Question question){
		CmdLoadQuestion (question);
	}

	public void SubmitAnswer(string id,int answerId){
		CmdSubmitAnswer (id, answerId);
	}



	[Command]
	void CmdSubmitAnswer(string id, int answerId){
		/**
		 * hack alert
		 * not sure why this needs to be done? :(
		 */ 
		if (gameScript == null) {
			gameScript = GameObject.FindObjectOfType<GameScript> ();
		}
		gameScript.CollectPlayerAnswer (id,answerId);
	}
	[Command]
	void CmdLoadQuestion(GameScript.Question question){
		if (gameScript == null) {
			gameScript = GameObject.FindObjectOfType<GameScript> ();
		} 
		gameScript.loadQuestion(question);
	}


}
