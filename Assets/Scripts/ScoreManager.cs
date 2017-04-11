using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {
	
	public List<string> playerNames;
	public GameObject infoPanel;
	public GameObject playerScoreItem;
	public GameObject bottomPlayerScoreItem;
	public GameObject playerScores;
	public Text genreText;
	public Text questionProgressText;
	public Dictionary<string,int> scores;
	public int currentQuestionNumber;


	public void ShowScores(){
		int count = 1;
		List<PlayerScore> pScores = new List<PlayerScore> ();
		foreach (string name in playerNames) {
			string pName = name.Substring (0, name.IndexOf ("("));

			int score = 0;
			if (scores.ContainsKey (name))
				score = scores [name];

			PlayerScore pScore = new PlayerScore (pName, score);
			pScores.Add (pScore);
		}
		pScores.Sort ();
		foreach (PlayerScore ps in pScores) {
			GameObject go = Instantiate (playerScoreItem);
			Text[] texts = go.GetComponentsInChildren<Text> ();
			texts [0].text = count.ToString();
			texts [1].text = ps.name;
			texts [2].text = ps.score.ToString();
			go.transform.SetParent (playerScores.transform, false);
			count++;
		}

		gameObject.SetActive (true);
	}
	public void HideScores(){
		
		foreach (Transform scoreItem in playerScores.transform) {
			Destroy (scoreItem.gameObject);
		}
		gameObject.SetActive (false);
	}

}
