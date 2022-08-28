using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour
{
    // Input-related variables
    [Tooltip("What layers block the player's targeting ray")]
    public LayerMask layersToHit;
    Vector2 viewInputVector = Vector2.zero;
    Vector2 moveInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;

    // Other components
    CharacterMovementHandler characterMovementHandler;
    CameraControls cameraControls;
    private CharacterInput _charInput;
    private PlayerInput _playerInput;

    private void Awake()
    {
        cameraControls = GetComponentInChildren<CameraControls>();
        characterMovementHandler = GetComponentInChildren<CharacterMovementHandler>();
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
        _playerInput.actions["jump"].performed += _ => {isJumpButtonPressed = true;};
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

        return networkInputData;
    }

    // Targeting, for later. not even sure if this belongs here..
    // private void Target()
    // {
    //     //playerInput.actions["clickTarget"].performed += _ => TargetObject();

    //     // Used to debug ray
    //     // if (_input.clickTarget)
    //     //     TargetObject();

    //     void TargetObject()
    //     {
    //         // TO DO - make entity stats a struct to send to server. health, mana, etc. need to live there.

    //         // _clickRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    //         // if (Physics.Raycast(_clickRay, out RaycastHit hitData, layersToHit))
    //         // {
    //         //     _playerTarget = hitData.transform.gameObject;

    //         //     if (_playerTarget.tag == "Player")
    //         //     {
    //         //         targetEntityStats = _playerTarget.GetComponent<EntityStats>();
    //         //         Debug.Log(targetEntityStats.health);
    //         //     }

    //         //     if (_playerTarget.tag == "Enemy")
    //         //     {
                    
    //         //     }
    //         // }
    //     }   
    // }
}
