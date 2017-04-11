using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MyNetworkManager : NetworkManager {


	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		Debug.Log(">> OnServerAddPlayer: "+conn + " / "+playerControllerId);
		base.OnServerAddPlayer(conn, playerControllerId);
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		Debug.Log(">> OnClientConnect: "+conn);
		base.OnClientConnect(conn);
	}

}
