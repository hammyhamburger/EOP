using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;
using Fusion;

public class CharacterInputHandler : MonoBehaviour
{
    // Input-related variables
    [Tooltip("What layers block the player's targeting ray")]
    public LayerMask layersToHit;
    Vector2 moveInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;
    NetworkId targettedEntity;

    // Other components
    CharacterNetworkHandler characterNetworkHandler;
    CameraControls cameraControls;
    private CharacterInput _charInput;
    private PlayerInput _playerInput;

    private PlayerController _playerController;
    private void Awake()
    {
        cameraControls = GetComponentInChildren<CameraControls>();
        characterNetworkHandler = GetComponentInChildren<CharacterNetworkHandler>();
        _playerController = GetComponentInChildren<PlayerController>();
        _charInput = GetComponentInChildren<CharacterInput>();
        _playerInput = GetComponentInChildren<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move input
        moveInputVector.x = _charInput.move.x;
        moveInputVector.y = _charInput.move.y;

        if (_playerInput.actions["jump"].triggered) {isJumpButtonPressed = true;};

        ClickTarget();
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        // View data
        networkInputData.aimVector = cameraControls.CinemachineCameraTarget.transform.forward;
        
        // Move data
        networkInputData.movementInput = moveInputVector;

        // Jump data
        networkInputData.isJumpPressed = isJumpButtonPressed;
        // Reset bool when jump has been performed
        isJumpButtonPressed = false;

        networkInputData.targettedEntity = targettedEntity;
        
        return networkInputData;
    }

    private void ClickTarget()
    {
        Ray clickRay;
        if (_playerInput.actions["clickTarget"].triggered && !(_playerInput.actions["rightClick"].IsInProgress()))
        {
            clickRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(clickRay, out RaycastHit hitData, layersToHit))
            {
                if (hitData.collider.GetComponent<Targetable>() != null)
                {
                    targettedEntity = hitData.collider.GetComponent<NetworkObject>().Id;
                }
            }
        }
    }
}
