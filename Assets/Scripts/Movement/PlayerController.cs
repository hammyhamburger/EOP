using UnityEngine;
using Cinemachine;
using Fusion;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[OrderBefore(typeof(NetworkTransform))]
public class PlayerController : NetworkTransform
{
    public float viewUpDownRotationSpeed = 50.0f;

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 4.5f;

    [Tooltip("Walk speed of the character in m/s")]
    public float WalkSpeed = 1.5f;

    [Tooltip("Testing...")]
    public float rotationSpeed= 25f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 30.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Tooltip("Testing..")]
    public float braking = 10.0f;

    // Player 
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // Networking
    [Networked]
    [HideInInspector]
    public bool IsGrounded { get; set; }

    [Networked]
    [HideInInspector]
    public Vector3 Velocity { get; set; }

    /// <summary>
    /// Sets the default teleport interpolation velocity to be the CC's current velocity.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToPosition"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;

    /// <summary>
    /// Sets the default teleport interpolation angular velocity to be the CC's rotation speed on the Z axis.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToRotation"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationAngularVelocity => new Vector3(0f, 0f, _rotationVelocity);

    // Timeout deltatime
    private float _fallTimeoutDelta;

    // Animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    // Targeting
    private Ray _clickRay;
    private GameObject _playerTarget;
    private EntityStats targetEntityStats;

    private PlayerInput _playerInput;
    private Animator _animator;
    public CharacterController _controller{get; private set; }
    private CharacterInput _input;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;

    protected override void Awake()
    {
        base.Awake();
        CacheController();

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<CharacterInput>();
        _playerInput = GetComponent<PlayerInput>();

        AssignAnimationIDs();
    }

    public override void Spawned() 
    {
    base.Spawned();
    CacheController();

    // Caveat: this is needed to initialize the Controller's state and avoid unwanted spikes in its perceived velocity
    _controller.Move(transform.position);
    }

    private void CacheController() 
    {
        if (_controller == null) 
        {
            _controller = GetComponent<CharacterController>();

        Assert.Check(_controller != null, $"An object with {nameof(NetworkCharacterControllerPrototype)} must also have a {nameof(CharacterController)} component.");
        }
    }

    protected override void CopyFromBufferToEngine() 
    {
        // Trick: CC must be disabled before resetting the transform state
        _controller.enabled = false;

        // Pull base (NetworkTransform) state from networked data buffer
        base.CopyFromBufferToEngine();

        // Re-enable CC
        _controller.enabled = true;
    }

    private void Start()
    {
        // reset our timeouts on start
        _fallTimeoutDelta = FallTimeout;
        
    }
    
    private void Update()
    {
        GroundedCheck();
        JumpAndGravity();
    }

    private void LateUpdate()
    {
        // CameraZoom();
        // CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }
        }
        else
        {
            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    public virtual void Move(Vector3 direction) 
    { 
      var deltaTime    = Runner.DeltaTime;
      var previousPos  = transform.position;
      var moveVelocity = Velocity;

      direction = direction.normalized;

      if (IsGrounded && moveVelocity.y < 0) {
      moveVelocity.y = 0f;
      }

      moveVelocity.y += Gravity * Runner.DeltaTime;

      var horizontalVel = default(Vector3);
      horizontalVel.x = moveVelocity.x;
      horizontalVel.z = moveVelocity.z;

      if (direction == default) {
      horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
      } else {
      horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * SpeedChangeRate * deltaTime, MoveSpeed);
      }

      moveVelocity.x = horizontalVel.x;
      moveVelocity.z = horizontalVel.z;

      _controller.Move(moveVelocity * deltaTime);

      Velocity   = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
      IsGrounded = _controller.isGrounded;
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
    public void Rotate(float rotationY)
    {
        transform.Rotate(0, rotationY * Runner.DeltaTime * rotationSpeed, 0);
    }
}