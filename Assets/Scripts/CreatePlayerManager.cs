using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class CreatePlayerManager : MonoBehaviour {


	void Start () {
		if (PlayerSingleton.Instance.isHost) {
			NetworkManager.singleton.networkPort = 27017;
			NetworkManager.singleton.StartHost ();
		} else {
			AvailableGame game = PlayerSingleton.Instance.gameToJoin;
			NetworkManager.singleton.networkAddress = game.ip;
			NetworkManager.singleton.networkPort = 27017;
//			NetworkManager.singleton.ud
//			NetworkManager.singleton.networkAddress = "192.168.1.47";
			NetworkManager.singleton.StartClient ();
		}
	}
}
