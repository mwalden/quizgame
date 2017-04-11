using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridLayoutFormatter : MonoBehaviour {
	GridLayoutGroup groupLayout;
	void Start () {
		groupLayout = GetComponent<GridLayoutGroup> ();
		Debug.Log (Screen.width - 20);
		groupLayout.cellSize.Set (Screen.width - 20, 30);
	}
}
