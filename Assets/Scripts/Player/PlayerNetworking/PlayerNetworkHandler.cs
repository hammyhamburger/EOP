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
    private PlayerController _playerController;
    private EntityStats _targetEntityStats;
    private Camera _camera;
    
    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _targetEntityStats = GetComponent<EntityStats>();
        _camera = GetComponentInChildren<Camera>();
    }

    public override void FixedUpdateNetwork()
    {
        //Get the input from the network
        if (GetInput(out NetworkInputData networkInputData))
        {
            // Rotate according to where the camera is aiming
            transform.forward = networkInputData.aimVector;

            // Don't rotate the player on any other axis but Y
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            //Move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            _playerController.Move(moveDirection, networkInputData.isWalkHeld); // If true then walking

            if (networkInputData.isJumpPressed)
                _playerController.Jump();

            _targetEntityStats.TargetEntity(networkInputData.targettedEntity);

            CheckFallRespawn();
        }
    }

    void CheckFallRespawn()
    {
        if (transform.position.y < -12)
            transform.position = new Vector3(1, 0, 1);
    }
}
