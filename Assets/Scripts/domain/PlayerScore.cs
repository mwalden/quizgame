using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class PlayerScore : IComparable<PlayerScore>{
	public string name;
	public int score;

	public PlayerScore(string name,int score){
		this.name = name;
		this.score = score;
	}
	public int CompareTo(PlayerScore playerScore){
		return playerScore.score.CompareTo (this.score);
	}

}
