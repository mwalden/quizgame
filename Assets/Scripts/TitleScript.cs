using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScript : MonoBehaviour {
//	public AudioSource startMusic;

//	void Awake(){
//		DontDestroyOnLoad (startMusic);
//	}

	


	void Start(){
//		startMusic.Play ();
//		PlayerPrefs.DeleteAll ();
	}
	public void onClick(){
//		if (!PlayerPrefs.HasKey(PlayerSingleton.Instance.DISPLAY_NAME))
			//SceneManager.LoadScene ("Login");
		PlayerPrefs.SetString(PlayerSingleton.Instance.DISPLAY_NAME,"temp name " + Random.Range(0,1000));
		
		SceneManager.LoadScene ("Start");
	}
}
