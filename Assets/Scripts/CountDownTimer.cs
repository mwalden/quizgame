using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountDownTimer : MonoBehaviour {

	public float countDownSeconds;
	public Slider slider;

	public bool stopped = true;

	private float currentCountDownSeconds;
	private GameScript gameScript;

	void Start(){
		slider = GetComponent<Slider> ();
		currentCountDownSeconds = countDownSeconds;
		GameObject go  = GameObject.FindGameObjectWithTag ("container");
		gameScript = go.GetComponent<GameScript> ();
		slider.maxValue = countDownSeconds;
	}
	
	// Update is called once per frame
	void Update () {
		if (stopped)
			return;
		if (currentCountDownSeconds >= 0) {
			slider.value += Time.deltaTime;
			currentCountDownSeconds -= Time.deltaTime;
		} else {
			stopped = true;
			gameScript.TimesUp ();
		}
	}

	public void StopTimer(){
		stopped = true;
	}

	public void StartTimer(){
		slider.value = 0;
		stopped = false;
	}

	public void ResetTimer(){
		currentCountDownSeconds = countDownSeconds;
	}

	void SetMaxSeconds(float maxValue){
		slider.maxValue = maxValue;		
	}

}
