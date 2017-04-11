using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnswerButtonScript : MonoBehaviour {
	public int id;
	public bool correct;

	private bool clicked;

	void Start(){
		Button button = GetComponent<Button> ();
//		button.onClick.AddListener (() => onClick ());
	}

	void Update(){
		Color color = new Color ();
		ColorBlock cb = new ColorBlock ();
//
//		ColorUtility.TryParseHtmlString ("#1A75ABFF", out color);
//		cb.normalColor = color;
//		Button button = GetComponent<Button> ();
//		button.colors = cb;
	}

	public void updateColor(int id){
		if (this.id != id)
			return;
		Color color = new Color ();
		ColorBlock cb = new ColorBlock ();

		ColorUtility.TryParseHtmlString ("#1A75ABFF", out color);
		cb.normalColor = color;
		cb.highlightedColor = color;
		cb.pressedColor = color;
		cb.colorMultiplier = 1f;
		Button button = GetComponent<Button> ();
		button.colors = cb;
	}
}
