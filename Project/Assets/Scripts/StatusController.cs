﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
	public int health;
	public int points;

	public GameControllerScript gcs;

	// Use this for initialization
	void Start ()
	{
		gcs = GameObject.FindGameObjectWithTag ("GameController").GetComponent< GameControllerScript> ();
		Restart ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (health < 0) {
			gcs.PlayerDead (0);
		}	
	}

	void Restart ()
	{
		health = 3;
		points = 0;
	}

	public void DamagePlayer (int damage)
	{
		health -= damage;
	}

	public void GivePoints (int points)
	{
		this.points += points;
	}

	public void Suicide ()
	{
		DamagePlayer (health + 1);
	}
}