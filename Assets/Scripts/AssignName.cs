using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AssignName : MonoBehaviour {


	// Use this for initialization
	void Start () {
		Text text = GetComponent<Text> ();
		text.text = PlayerPrefs.GetString (PlayerSingleton.Instance.DISPLAY_NAME);
		PlayerSingleton.Instance.playerName = text.text;
//		text.text = adjective [Random.Range (0, adjective.Count - 1)] + " " + animals [Random.Range (0, animals.Count - 1)];
	}

}
