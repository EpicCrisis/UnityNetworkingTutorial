using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// PlayerController can be controlled by a player.

public class PlayerController : NetworkBehaviour
{
    float speed = 5.0f;
    Vector3 velocity;
    Vector3 direction;

    // Position closes to correctness of the player.
    // Note: if we are authority, we will directly use position.transform instead.
    Vector3 bestGuessPosition;

    // This is a constantly update value about our latency to the server.
    // i.e. how many seconds it takes to receive a one-way message.
    // Something we get from the PlayerConnectionObject.
    float ourLatency = 1.0f;

    // The higher this value, the faster the position will match the best guess position.
    float latencySmoothFactor = 10.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // This function runs for all units, NOT just my own.
        // How to verify only I can mess around with my GameObject?

        // Code running here is running for all versions of this object,
        // even if it is not authoritative copy.
        // Even if we're not the owner, we should predict where the object
        // will be based on its previous velocity update.

        if ( !hasAuthority )
        {
            // We aren't the authority of this object but we still need
            // to update our local position based on our best guess on where
            // it probably is on the owning player's screen.

            bestGuessPosition = bestGuessPosition + ( velocity * Time.deltaTime );

            // Instead of teleporting the position to the best guess position,
            // we can slowly Lerp towards it.

            transform.position = Vector3.Lerp( transform.position, bestGuessPosition,
                latencySmoothFactor * Time.deltaTime );

            return;
        }

        float deltaSpeed = speed * Time.deltaTime;

        float xAxis = Input.GetAxis( "Horizontal" ) * deltaSpeed;
        float yAxis = Input.GetAxis( "Vertical" ) * deltaSpeed;

        velocity = new Vector3( xAxis, 0.0f, yAxis );

        // If we get to here, we are authority.
        transform.Translate( velocity );
        CmdUpdateVelocity( velocity, transform.position );

        //if ( Input.GetKey( KeyCode.W ) )
        //{
        //    transform.Translate( new Vector3(0.0f, 0.0f, deltaSpeed) );
        //}
        //if ( Input.GetKey( KeyCode.A ) )
        //{
        //    transform.Translate( new Vector3( -deltaSpeed, 0.0f, 0.0f) );
        //}
        //if ( Input.GetKey( KeyCode.S ) )
        //{
        //    transform.Translate( new Vector3( 0.0f, 0.0f, -deltaSpeed ) );
        //}
        //if ( Input.GetKey( KeyCode.D ) )
        //{
        //    transform.Translate( new Vector3( deltaSpeed, 0.0f, 0.0f ) );
        //}

        if ( Input.GetKeyDown( KeyCode.Space ) )
        {
            velocity = new Vector3( 1.0f, 0.0f, 0.0f );

            CmdUpdateVelocity( velocity, transform.position );
        }

        /*
        if (Input.GetKeyDown (KeyCode.F)) {
			Destroy (this.gameObject);
		}
        */
    }

    [Command]
    void CmdUpdateVelocity( Vector3 _velocity, Vector3 _transform )
    {
        transform.position = _transform;
        velocity = _velocity;

        // If we know our current latency, we could do something like this.
        // transform.position = _transform + (_velocity * (thisPlayerLatencyToServer));

        RpcUpdateVelocity( velocity, transform.position );
    }

    [ClientRpc]
    void RpcUpdateVelocity( Vector3 _velocity, Vector3 _transform )
    {
        //I am on a client.

        if ( hasAuthority )
        {
            // This is my object, so I get the best accuracy than the server.
            // Depending on the game, I might have to change to patch this info.
            // even though this might be wonky to the user.

            //Assume we ignore message from server.
            return;
        }

        // I am non-authoritative client, so I will listen to the server.

        // If we know our current latency, we could do something like this.
        // transform.position = _transform + (_velocity * (ourLatency + theirLatency));

        velocity = _velocity;
        bestGuessPosition = _transform + ( _velocity * ( ourLatency ) );

        // Now position of player is as close as possible on all players' screen.

        // In fact, we don't want to directly update transform.position because
        // players will teleport and blinking as the game updates.
    }
}
