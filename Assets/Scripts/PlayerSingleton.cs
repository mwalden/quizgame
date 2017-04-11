using System.Collections;
using System.Collections.Generic;

public class PlayerSingleton  {
	//ip of the server
	public string HOST = "http://hackweek.ddns.net:9000";
	public string DISPLAY_NAME =  "displayName";

	//name of the player 
	public string playerName;
	//if host, this will be the name of the game. used to the genre selector.
	public string nameOfGame;
	//if the player decided he wanted to be the host or not.
	public bool isHost;
	//once a game is created, we hold onto the id of the game that is stored in mongo.
	public string idOfGame;
	//this is reference to the player script
	public NewPlayerScript player;
	//dictionary of all the players names in the game
	public Dictionary<string,string> playerNames;
	//could probably use this instead of idOfGame?
	public AvailableGame gameToJoin;
	//genre Id that a host has selected
	public int genreId;
	private static PlayerSingleton instance;


	private PlayerSingleton() {}

	public static PlayerSingleton Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new PlayerSingleton();
			}
			return instance;
		}
	}
}
