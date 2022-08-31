using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/*
This class is designed for things the player does that the server needs to know about.
*/
public class CharacterNetworkHandler : NetworkBehaviour
{
    // Other components
    PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

            playerController.Move(moveDirection);

            if (networkInputData.isJumpPressed)
                playerController.Jump();
            
            if (networkInputData.targettedEntity)
            {
                playerController.TargetEntity(networkInputData.targettedEntity);
            }

            CheckFallRespawn();
        }
    }

    void CheckFallRespawn()
    {
        if (transform.position.y < -12)
            transform.position = new Vector3(1, 0, 1);
    }
}
