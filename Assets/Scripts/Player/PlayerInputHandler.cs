using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;
using Fusion;

public class PlayerInputHandler : MonoBehaviour
{
    // Input-related variables
    [Tooltip("What layers block the player's targeting ray")]
    public LayerMask layersToHit;
    private bool _isJumpButtonPressed = false;
    private bool _isWalkheld = false;
    private Vector2 moveInputVector = Vector2.zero;
    private NetworkId _targettedEntityId;
    private EntityStats _entityStats;


    // Other components
    private CameraControls _cameraControls;
    private PlayerInputHelper _playerInputHelper;
    private PlayerInput _playerInput;

    private PlayerController _playerController;
    private void Awake()
    {
        _entityStats = GetComponentInChildren<EntityStats>();
        _cameraControls = GetComponentInChildren<CameraControls>();
        _playerController = GetComponentInChildren<PlayerController>();
        _playerInputHelper = GetComponentInChildren<PlayerInputHelper>();
        _playerInput = GetComponentInChildren<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move input
        moveInputVector.x = _playerInputHelper.move.x;
        moveInputVector.y = _playerInputHelper.move.y;

        // Jump input
        if (_playerInput.actions["jump"].triggered) {_isJumpButtonPressed = true;};

        // Walk input
        if (_playerInput.actions["walk"].IsPressed()) {_isWalkheld = true;};

        // Targetting input
        ClickTarget();
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        // View data
        networkInputData.AimVector = _cameraControls.CinemachineCameraTarget.transform.forward;
        
        // Move data
        networkInputData.MovementInput = moveInputVector;

        // Jump data
        networkInputData.IsJumpPressed = _isJumpButtonPressed;
        // Reset bool when jump has been performed
        _isJumpButtonPressed = false;

        // Walk data
        networkInputData.IsWalkHeld = _isWalkheld;
        // Reset bool when walk has stopped
        _isWalkheld = false;

        // Target data
        networkInputData.TargetId = _targettedEntityId;
        
        return networkInputData;
    }

    private void ClickTarget()
    {
        Ray clickRay;
        if (_playerInput.actions["clickTarget"].triggered && !(_playerInput.actions["rightClick"].IsInProgress()))
        {
            clickRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(clickRay, out RaycastHit hitData, layersToHit) && hitData.collider.GetComponent<EntityStats>())
            {
                _targettedEntityId = hitData.collider.GetComponent<NetworkObject>().Id;
            }
            else
            {
                _targettedEntityId = default;
            }
        }
    }
}
