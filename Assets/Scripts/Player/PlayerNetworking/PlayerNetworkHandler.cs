using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/*
This class is designed for things the player does that the server needs to know about.
*/
public class PlayerNetworkHandler : NetworkBehaviour
{
    // Other components
    PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public override void FixedUpdateNetwork()
    {
        //Get the input from the network
        if (GetInput(out NetworkInputData networkInputData))
        {
            // Rotate according to where the player is aiming w/ right click
            transform.forward = networkInputData.aimVector;

            // Don't rotate the player on any other axis but Y
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            //Move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            if (networkInputData.isWalkHeld)
                playerController.Move(moveDirection, 1.5f); // Walking

            playerController.Move(moveDirection, 4.5f); // Running

            if (networkInputData.isJumpPressed)
                playerController.Jump();

            playerController.TargetEntity(networkInputData.targettedEntity);

            CheckFallRespawn();
        }
    }

    void CheckFallRespawn()
    {
        if (transform.position.y < -12)
            transform.position = new Vector3(1, 0, 1);
    }
}
