using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [Header("Cinemachine")]

    [Tooltip("Virtual camera following CinemachineCameraTarget")]
    public CinemachineVirtualCamera CinemachinePlayerCam;

    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    [Tooltip("How fast the player can move the camera")]
    public float cameraSensitivity = 1;

    [Tooltip("How far the player can zoom out the camera")]
    public float cameraMaxZoomDistance = 15;

    // Cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private float _cinemachineCameraDistance = 6.0f;
    private float _cinemachineFollowDistance;
    private const float _threshold = 0.01f;
    private bool _canRotate = false;
    private Vector2 _cineMachineDistance;
    private Vector2 _mousePosition; // Holds mouse cursor pos
    
    // In case I use a transposer instead of 3rd person follow
    // private Vector3 _cinemachineFollowDistance = new Vector3(0,3,0);
    CinemachineComponentBase _componentBase;

    // Other Components
    private CharacterInput _charInput;
    private PlayerInput _playerInput;

    void Awake()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _componentBase = CinemachinePlayerCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        _cinemachineFollowDistance = (_componentBase as Cinemachine3rdPersonFollow).CameraDistance;
    }

    void Start()
    {
        _charInput = GetComponentInChildren<CharacterInput>();
        _playerInput = GetComponentInChildren<PlayerInput>();
    }

    // Late update because of camera shenanigans
    void LateUpdate()
    {
        CameraRotation();
        CameraZoom();
        RightClickCheck();
    }

    public void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_charInput.look.sqrMagnitude >= _threshold && !LockCameraPosition && _canRotate)
        {
            // Constantly warping the mouse to make sure it doesnt leave the frame it appears
            Mouse.current.WarpCursorPosition(_mousePosition);

            _cinemachineTargetYaw += _charInput.look.x * cameraSensitivity;
            _cinemachineTargetPitch += _charInput.look.y * cameraSensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    public void CameraZoom()
    {
        float scroll = _charInput.scroll;
        // Camera distance should not be greater than 15 or negative
        if (scroll != 0 && 
            scroll > 0 && _cinemachineCameraDistance > cameraMaxZoomDistance || 
            scroll < 0 && _cinemachineCameraDistance < 0 || 
            (_cinemachineCameraDistance > 0 && _cinemachineCameraDistance < cameraMaxZoomDistance))
        {
            _cinemachineCameraDistance -= scroll / 300f;
        }
        (_componentBase as Cinemachine3rdPersonFollow).CameraDistance = _cinemachineCameraDistance;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    //There must be a better way..
    private void RightClickCheck()
    {
        // If they start holding, hide cursor and record pos
        _playerInput.actions["rightClick"].started += _ => 
        {
            _mousePosition = Mouse.current.position.ReadValue();
            Cursor.visible = false;
            _canRotate = true;
        };

        // If they stop holding, show cursor and warp it a last time to the recorded spot
        _playerInput.actions["rightClick"].canceled += _ => 
        {
            Cursor.visible = true;
            Mouse.current.WarpCursorPosition(_mousePosition);
            _canRotate = false;
        };
    }
}
