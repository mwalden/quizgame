using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class FindGamesManager : MonoBehaviour {
	public GameObject gamesContainer;

	public GameObject gamePrefab;
	public AvailableGame[] games;

	private Dictionary<int,string> genreDictionary = new Dictionary<int,string>(){
		{16,"Top 40"},{5,"Country"},{12,"ROCK"},{1,"ALTERNATIVE"},{104,"R & B"},{77,"DANCE"}
	};

	// Use this for initialization
	void Start () {
		WWW www = new WWW (PlayerSingleton.Instance.HOST+"/api/games/all");
//		WWW www = new WWW ("http://10.89.196.59:3000/games");
		StartCoroutine (FindGames(www));
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene ("Start");
		}
	}

	IEnumerator FindGames(WWW www){
		yield return www;
		if (www.error == null) {
			AvailableGamesCollection result = JsonUtility.FromJson<AvailableGamesCollection>(www.text);
			games = result.games;
			foreach (AvailableGame game in result.games) {
				GameObject prefab = Instantiate (gamePrefab);	

				Text[] gameInfoTexts = prefab.GetComponentsInChildren<Text> ();
				for (int x = 0; x < 2; x++) {
					Text text = gameInfoTexts [x];
					if (x == 0) {						
						text.text = game.name.ToUpper();
					}
					if (x == 1) {
						text.text = genreDictionary[game.genreId];
					}
				}
				Button joinButton = prefab.GetComponent<Button> ();
//				AvailableGameScript script = joinButton.GetComponent<AvailableGameScript> ();
//				script.game = game;
				AddListener (joinButton,game);
//				name.text = game.name;
				prefab.transform.SetParent(gamesContainer.transform, false);
			}
		}
		StopAllCoroutines ();
	}

	void AddListener(Button b, AvailableGame game){
		b.onClick.AddListener(() => JoinGame(game));
	}

	void JoinGame(AvailableGame joinedGame){
		foreach (AvailableGame game in games) {
			if (game.id == joinedGame.id) {
				Debug.Log ("Joining " + game.name + " :: " + game.id);
				PlayerSingleton.Instance.gameToJoin = game;
				//hack!! WEEE!!! clients need the name too you know?
				PlayerSingleton.Instance.nameOfGame = game.name;
				break;
			}
		}
		SceneManager.LoadScene ("Lobby");

	}
}
