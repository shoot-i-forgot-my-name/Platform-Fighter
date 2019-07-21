using System;
using System.Timers;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour {

    #region Static Variables

    public static readonly float limAngle = 45; // Limiting angle

    #endregion Static Variables

    #region Private Variables

    #region General

    private CollisionType collisionType = CollisionType.None; // CollisionFlags alternative
    private Rigidbody2D rb; // Reference to rigidbody
    private Vector2 modifiedInput; // Input that only accepts 1 or -1

    private Vector2 velocity = Vector2.zero; // Velocity of the gameObject

    #endregion

    #region Basic Movement

    private Action<Vector2> OnceCollisionType; // Recalculates the CollisionType of the gameObject; Done once on exit of collision, resets
    private float curSpeed; // Current speed of the player

    #endregion

    #region Wall Jumping

    private Func<Vector2, Vector2, Vector2> OnceEditVelocity; // Helper for clinging walls; Done once on start of clinging walls, resets

    private float defaultGravityScale; // Default gravity scale of the gameObject

    #endregion

    #region Air Jumping

    private bool defaultAirJumpEnabled = true; // Default value

    #endregion

    #endregion Private Variables

    #region Public Variables

    #region Basic Movement

    [Header("Basic Movement")]
    public bool basicMovementEnabled = true;

    public float moveSpeed = 10; // Default movement speed
    public float smoothStop = 10; // Time for the character to stop moving
    public float smoothMove = 7.5f; // Time for the character to start moving
    public float airMovement = .75f; // Percentage of the movement when on air

    #endregion

    #region Jumping

    [Header("Jumping")]
    public bool jumpingEnabled = true;

    public float jumpForce = 15;

    #endregion

    #region Air Jumping

    [Header("Air Jumping")]
    public bool airJumpEnabled = true;

    public float jumpForcePercentage = .5f; // Percentage of jumpForce when double jumping

    #endregion

    #region Wall Sliding

    [Header("Wall Sliding")]
    public bool wallSlidingEnabled = true;

    public float gravityScale = .75f; // Percentage of the mass when colliding with a wall
    public float startVelocityY = 1; // Starting velocity of the gameObject when clinging

    #endregion

    #region Wall Jumping

    [Header("Wall Jumping")]
    public bool wallJumpingEnabled = true;

    public Vector2 wallJumpForce = new Vector2(11.25f, 7.5f);
    public int basicMovementTimeout = 500; // Timeout before the player can move

    #endregion

    #endregion Public Variables

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
        OnceCollisionType = Utility.CallActionOnce<Vector2>(UpdateCollisionType);
        OnceEditVelocity = Utility.CallFuncOnce((Vector2 a) => a);

        if (!airJumpEnabled) {
            // TODO: Do this
        }
    }

    private void Update () {
        var inputRaw = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        modifiedInput.x = (inputRaw.x == 0) ? modifiedInput.x : inputRaw.x; // Accepts -1 or 1
        modifiedInput.y = (inputRaw.y == 0) ? modifiedInput.y : inputRaw.y; // Accepts -1 or 1

        var jumping = Input.GetButtonDown("Jump") && collisionType == CollisionType.Below;
        var wallSliding = (collisionType.ExactlyEqual(CollisionType.Right) || collisionType.ExactlyEqual(CollisionType.Left)) && inputRaw.x != 0;
        var wallJumping = wallSliding && Input.GetButtonDown("Jump");

        rb.velocity = Move(inputRaw, jumping, wallSliding, wallJumping);
    }

    private Vector2 Move (Vector2 inputRaw, bool jumping, bool wallSliding, bool wallJumping) {
        velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        rb.gravityScale = defaultGravityScale;

        if (basicMovementEnabled) {
            MoveBasic(inputRaw);
        }

        if (airJumpEnabled) {
            if (collisionType.ExactlyEqual(CollisionType.None) && Input.GetButtonDown("Jump")) {
                airJumpEnabled = false;
            }
        }

        if (jumpingEnabled) {
            if (jumping) {
                airJumpEnabled = true;
                Jump();
            }
        }

        if (wallSlidingEnabled) {
            if (wallSliding) {
                WallSlide();
            }else {
                OnceEditVelocity = Utility.CallFuncOnce((Vector2 a) => a); // Reset context on OnceReturnVelocity
            }
        }

        if (wallSlidingEnabled) {
            if (wallJumping) {
                WallJump();
            }
        }

        var direction = new Vector2(Mathf.Sign(velocity.x), Mathf.Sign(velocity.y));

        // This fixes not falling while moving to the right or left and colliding
        if ((collisionType == CollisionType.Right && direction.x == 1) || (collisionType == CollisionType.Left && direction.x == -1)) {
            velocity.x = 0;
        }

        return velocity;
    }

    #region Movement Methods

    private void MoveBasic (Vector2 inputRaw) {
        var smoothTime = (inputRaw.x == 0) ? smoothStop : smoothMove; // Either smoothStop or smoothMove
        var targetSpeed = (inputRaw.x == 0) ? 0 : moveSpeed; // Either 0 or the default speed
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed, smoothTime * ((collisionType == CollisionType.None) ? airMovement : 1) * Time.deltaTime); // Lerp for smooth movement

        velocity.x = modifiedInput.x * curSpeed; // Percentage of movement when on air
    }

    private void Jump () {
        velocity.y = jumpForce;
    }

    private void WallSlide () {
        if (Mathf.Sign(velocity.y) == -1) { // If the player is moving downward
            // Set gravity scale to this
            rb.gravityScale = defaultGravityScale * gravityScale;
        }

        velocity = OnceEditVelocity(new Vector2(velocity.x, -startVelocityY), velocity); // Sets it once when you wall cling
    }

    private void WallJump () {
        velocity.x = -modifiedInput.x * wallJumpForce.x;
        velocity.y = wallJumpForce.y;

        basicMovementEnabled = false;

        Utility.SetTimeout((object sender, ElapsedEventArgs e) => basicMovementEnabled = true, basicMovementTimeout);
    }

    #endregion Movement Methods

    #region Calculate Collision Type

    private void UpdateCollisionType (Vector2 normal) {
        var aboveBottom = Mathf.Rad2Deg * Mathf.Asin(normal.y); // Calculate angle
        var rightLeft = Mathf.Rad2Deg * Mathf.Asin(normal.x); // Calculate angle

        // Get collision types
        if (aboveBottom < -limAngle) {
            collisionType += CollisionType.Above;
        }

        if (aboveBottom > limAngle) {
            collisionType += CollisionType.Below;
        }

        if (rightLeft < -limAngle) {
            collisionType += CollisionType.Right;
        }

        if (rightLeft > limAngle) {
            collisionType += CollisionType.Left;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision) {
        UpdateCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionStay2D (Collision2D collision) {
        OnceCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionExit2D () {
        OnceCollisionType = Utility.CallActionOnce<Vector2>(UpdateCollisionType);
        collisionType -= CollisionType.None;
    }

    #endregion Calculate Collision Type
}