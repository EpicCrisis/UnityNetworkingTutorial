using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionInstance : NetworkBehaviour
{

    public GameObject PlayerUnitPrefab;

    //SyncVars syncs all clients of a variable that changes in the server.
    [SyncVar( hook = "OnPlayerNameChanged" )]
    public string PlayerName = "Anonymous";

    void Start()
    {
        // Is this Start actually for my own PlayerObject?
        if ( !isLocalPlayer )
        {
            // This object belongs to another player.
            return;
        }

        // If the PlayerObject is invisible and not part of the world,
        // give me something I can physically move around!

        Debug.Log( "PlayerObject : Spawn my own personal unit." );

        // Instantiate() only creates an object on the LOCAL COMPUTER.
        // Even if it has a NetworkIdentity, it still will NOT exist on
        // the network (and therefore not on any other client) UNLESS
        // NetworkServer.Spawn is called on this object.

        //Instantiate (PlayerUnitPrefab);
        CmdSpawnMyUnit();
    }

    void Update()
    {
        // Remember Update() runs on everyone's computer, whether or not they own this
        // particular game object.

        if ( !isLocalPlayer )
        {
            return;
        }

        if ( Input.GetKeyDown( KeyCode.Q ) )
        {
            string n = "Vokzole" + Random.Range( 1, 100 );

            // Tell every client what this new name is.
            Debug.Log( "Change player name from " + PlayerName + " to " + n );
            CmdChangePlayerName( n );
        }

        /*
        if (Input.GetKeyDown (KeyCode.S)) {
			CmdSpawnMyUnit ();
		}
        */
    }

    void OnPlayerNameChanged( string newName )
    {
        Debug.Log( "OnPlayerNameChanged: OldName: " + PlayerName + "   NewName: " + newName );

        // WARNING! If you use a hook on a SyncVar, then our local value does NOT get auto update.
        // Explicit variable change required here.
        PlayerName = newName;
        gameObject.name = "PlayerConnectionObject [" + newName + "]";
    }

    //===========COMMANDS===========//
    // Commands are special functions that only get executed on the server.
    //GameObject myPlayerUnit;

    [Command]
    void CmdSpawnMyUnit()
    {
        // We are guaranteed to be on the server now.
        GameObject go = Instantiate( PlayerUnitPrefab );

        //myPlayerUnit = go;
        //go.GetComponent<NetworkIdentity> ().AssignClientAuthority (connectionToClient);

        // Now that the object exists on the server, propagate it to all
        // the clients and also wire up to NetworkIdentity.
        NetworkServer.SpawnWithClientAuthority( go, connectionToClient );
    }

    [Command]
    void CmdChangePlayerName( string n )
    {
        PlayerName = n;

        // Check name for blacklisted words.

        // Tell all clients this is the new player name.
        //RpcChangePlayerName(PlayerName);
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

    //===========RPC===========//
    // RPCs are special functions that ONLY get executed on clients.

    /*
    [ClientRpc]
    void RpcChangePlayerName(string n)
    {
        Debug.Log("Change name on a particular PlayerConnectionObject: " + n);
        PlayerName = n;
    }
    */
}
