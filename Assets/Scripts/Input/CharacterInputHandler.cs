using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;

public class CharacterInputHandler : MonoBehaviour
{
    // Input-related variables
    [Tooltip("What layers block the player's targeting ray")]
    public LayerMask layersToHit;
    Vector2 moveInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;
    int targettedEntity = 0;

    // Other components
    CharacterNetworkHandler characterNetworkHandler;
    CameraControls cameraControls;
    private CharacterInput _charInput;
    private PlayerInput _playerInput;
    private void Awake()
    {
        cameraControls = GetComponentInChildren<CameraControls>();
        characterNetworkHandler = GetComponentInChildren<CharacterNetworkHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _charInput = GetComponent<CharacterInput>();
        _playerInput = GetComponent<PlayerInput>();
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
                    targettedEntity = hitData.collider.GetComponent<Targetable>().targetId;
                }
                else
                {
                    targettedEntity = -1;
                }
            }
        }
    }
}
