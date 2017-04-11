using UnityEngine;
using System.Collections;

public class ScoreScript : MonoBehaviour {
	public RectTransform scoresContainer;
	public GameObject scorePrefab;
	public Canvas canvas;

	void Start(){
		scoresContainer.sizeDelta = new Vector2 ((scoresContainer.rect.width * canvas.scaleFactor)/2, 4f * 30f);
		for (int x = 0; x < 3; x++) {
			GameObject go = Instantiate (scorePrefab);
			go.transform.SetParent (scoresContainer.transform, false);
		}
	}
}
