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
    private Vector2 mousePosition; // Holds mouse cursor pos
    bool isJumpButtonPressed = false;
    bool canRotate = false;

    // Other components
    CharacterMovementHandler characterMovementHandler;
    CharacterInput characterInput;
    PlayerInput playerInput;
    PlayerController playerController;
    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        characterInput = GetComponent<CharacterInput>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        viewInputVector = Vector2.zero;
        canRotate = RightClickCheck();
        if (canRotate)
        {
            viewInputVector.x = characterInput.look.x;
            viewInputVector.y = characterInput.look.y;
        }
        characterMovementHandler.SetViewInputVector(viewInputVector);
        

        //Move input
        moveInputVector.x = characterInput.move.x;
        moveInputVector.y = characterInput.move.y;

        isJumpButtonPressed = Input.GetButtonDown("Jump");
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        //View data
        networkInputData.rotationInput = viewInputVector.x;

        //Move data
        networkInputData.movementInput = moveInputVector;

        //Jump data
        networkInputData.isJumpPressed = isJumpButtonPressed;

        return networkInputData;
    }

    private bool RightClickCheck()
    {
        playerInput.actions["rightClick"].started += _ => recordCursor();
        playerInput.actions["rightClick"].performed += _ => lockCursor();
        playerInput.actions["rightClick"].canceled += _ => unlockCursor();

        void recordCursor()
        {
            mousePosition = Mouse.current.position.ReadValue();
            Cursor.visible = false;
        }

        void lockCursor()
        {
            canRotate = true;
        }

        void unlockCursor()
        {
            Cursor.visible = true;
            Mouse.current.WarpCursorPosition(mousePosition);
            canRotate = false;
        }

        return canRotate;
    }

    // Targeting, for later
    private void Target()
    {
        playerInput.actions["clickTarget"].performed += _ => TargetObject();

        // Used to debug ray
        // if (_input.clickTarget)
        //     TargetObject();

        void TargetObject()
        {
            // TO DO - make entity stats a struct to send to server. health, mana, etc. need to live there.

            // _clickRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            // if (Physics.Raycast(_clickRay, out RaycastHit hitData, layersToHit))
            // {
            //     _playerTarget = hitData.transform.gameObject;

            //     if (_playerTarget.tag == "Player")
            //     {
            //         targetEntityStats = _playerTarget.GetComponent<EntityStats>();
            //         Debug.Log(targetEntityStats.health);
            //     }

            //     if (_playerTarget.tag == "Enemy")
            //     {
                    
            //     }
            // }
        }   
    }
}
