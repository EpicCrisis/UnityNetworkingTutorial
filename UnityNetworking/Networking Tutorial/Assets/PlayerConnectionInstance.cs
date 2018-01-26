using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionInstance : NetworkBehaviour
{

	public GameObject PlayerUnitPrefab;

	// Use this for initialization
	void Start ()
	{
		// Is this Start actually for my own PlayerObject?
		if (!isLocalPlayer) {
			// This object belongs to another player.
			return;
		}

		// If the PlayerObject is invisible and not part of the world,
		// give me something I can physically move around!

		Debug.Log ("PlayerObject : Spawn my own personal unit.");

		// Instantiate() only creates an object on the LOCAL COMPUTER.
		// Even if it has a NetworkIdentity, it still will NOT exist on
		// the network (and therefore not on any other client) UNLESS
		// NetworkServer.Spawn is called on this object.

		//Instantiate (PlayerUnitPrefab);

		// Command politely the server to SPAWN our unit.
		CmdSpawnMyUnit ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Remember Update() runs on everyone's computer, whether or not they own this
		// particular game object.

		if (!isLocalPlayer) {
			return;
		}

		if (Input.GetKeyDown (KeyCode.S)) {
			// Spacebar was hit, we can instruct the server to do something for our unit.
			CmdSpawnMyUnit ();
		}
	}

	//===========COMMANDS===========//
	// Commands are special functions that only get executed on the server.

	//GameObject myPlayerUnit;

	[Command]
	void CmdSpawnMyUnit ()
	{
		// We are guaranteed to be on the server now.
		GameObject go = Instantiate (PlayerUnitPrefab);

		//myPlayerUnit = go;

		//go.GetComponent<NetworkIdentity> ().AssignClientAuthority (connectionToClient);

		// Now that the object exists on the server, propagate it to all
		// the clients and also wire up to NetworkIdentity.
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
	}

	/*
	[Command]
	void CmdMovePlayer ()
	{
		if (myPlayerUnit == null) {
			return;
		}

		myPlayerUnit.transform.Translate (0.0f, 1.0f, 0.0f);
	}
	*/
}
