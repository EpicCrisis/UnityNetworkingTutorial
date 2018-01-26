using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// PlayerController can be controlled by a player.

public class PlayerController : NetworkBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		// This function runs for all units, NOT just my own.

		// How to verify only I can mess around with my GameObject?

		if (!hasAuthority) {
			return;
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			this.transform.Translate (0.0f, 1.0f, 0.0f);
		}

		if (Input.GetKeyDown (KeyCode.D)) {
			Destroy (this.gameObject);
		}
	}
}
