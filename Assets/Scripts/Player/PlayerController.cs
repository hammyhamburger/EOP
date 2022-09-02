using UnityEngine;
using Fusion;

[RequireComponent(typeof(CharacterController))]
[OrderBefore(typeof(NetworkTransform))]
public class PlayerController : NetworkTransform
{

    [Header("Player")]
    [Tooltip("Move speed of the character")]
    public float MoveSpeed = 4.5f;

    [Tooltip("Walk speed of the character")]
    public float WalkSpeed = 1.5f;

    [Tooltip("Testing...")]
    public float rotationSpeed= 25f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 30.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float jumpImpulse = 15f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    // Player 
    private float _animationBlend;
    private float _rotationVelocity;

    // Networking
    [Networked]
    [HideInInspector]
    public bool IsGrounded { get; set; }

    [Networked]
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

    private Animator _animator;
    public CharacterController Controller{get; private set; }

    private EntityStats PlayerEntityStats;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;

    protected override void Awake()
    {
        base.Awake();
        CacheController();

        _hasAnimator = TryGetComponent(out _animator);
        Controller = GetComponent<CharacterController>();
        PlayerEntityStats = GetComponent<EntityStats>();

        AssignAnimationIDs();
    }

    public override void Spawned() 
    {
    base.Spawned();
    CacheController();

    // Caveat: this is needed to initialize the Controller's state and avoid unwanted spikes in its perceived velocity
    Controller.Move(transform.position);
    }

    private void CacheController() 
    {
        if (Controller == null) 
        {
            Controller = GetComponent<CharacterController>();

        Assert.Check(Controller != null, $"An object with {nameof(CharacterController)} must also have a {nameof(CharacterController)} component.");
        }
    }

    protected override void CopyFromBufferToEngine() 
    {
        // Trick: CC must be disabled before resetting the transform state
        Controller.enabled = false;

        // Pull base (NetworkTransform) state from networked data buffer
        base.CopyFromBufferToEngine();

        // Re-enable CC
        Controller.enabled = true;
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    void Update()
    {
        MoveSpeed += PlayerEntityStats.MoveSpeed;
    }

    /// <summary>
    /// Basic implementation of a character controller's movement function based on an intended direction.
    /// <param name="direction">Intended movement direction, subject to movement query, acceleration and max speed values.</param>
    /// </summary>
    public virtual void Move(Vector3 direction, bool walking) 
    { 
        var deltaTime    = Runner.DeltaTime;
        var previousPos  = transform.position;
        var moveVelocity = Velocity;
        var speed = MoveSpeed;
        
        if (walking)
            speed = WalkSpeed;

        direction = direction.normalized;

        if (IsGrounded && moveVelocity.y < 0) {
            moveVelocity.y = 0f;
        }

        moveVelocity.y += Gravity * Runner.DeltaTime;

        var horizontalVel = default(Vector3);
        horizontalVel.x = moveVelocity.x;
        horizontalVel.z = moveVelocity.z;

        if (direction == default) {
            horizontalVel = default;
        } else {
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * SpeedChangeRate * deltaTime, speed);
        }

        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;

        Controller.Move(moveVelocity * deltaTime);

        Velocity   = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
        IsGrounded = Controller.isGrounded;
    }

    /// <summary>
    /// Basic implementation of a jump impulse (immediately integrates a vertical component to Velocity).
    /// <param name="ignoreGrounded">Jump even if not in a grounded state.</param>
    /// <param name="overrideImpulse">Optional field to override the jump impulse. If null, <see cref="jumpImpulse"/> is used.</param>
    /// </summary>
    public virtual void Jump(bool ignoreGrounded = false, float? overrideImpulse = null) {
        if (IsGrounded || ignoreGrounded) {
        var newVel = Velocity;
        newVel.y += overrideImpulse ?? jumpImpulse;
        Velocity =  newVel;
        }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(Controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(Controller.center), FootstepAudioVolume);
        }
    }
}