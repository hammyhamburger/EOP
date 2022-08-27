using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/*
This class is designed for movements that need to be sent to the server.
It's currently used for walking forward or rotating the player.
It could be used for spells that displace the player.
*/
public class CharacterMovementHandler : NetworkBehaviour
{
    Vector2 viewInput;

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
        }
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
