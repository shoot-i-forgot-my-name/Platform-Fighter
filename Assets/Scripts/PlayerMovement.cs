using System;
using System.Timers;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour {

    #region Private Variables

    #region Wall Jumping

    private Func<Vector2, Vector2, Vector2> OnceEditVelocity; // Helper for clinging walls; Done once on start of clinging walls, resets

    private float defaultGravityScale; // Default gravity scale of the gameObject

    #endregion

    #endregion Private Variables

    #region Public Variables

    #region Jumping

    [Header("Jumping")]
    public bool jumpingEnabled = true;

    public float jumpForce = 15;

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
        OnceEditVelocity = Utility.CallFuncOnce((Vector2 a) => a);

        if (!airJumpEnabled) {
            // TODO: Do this
        }
    }

    private void Update () {
        var jumping = Input.GetButtonDown("Jump") && collisionType == CollisionInfo.Below;
        var wallSliding = (collisionType.ExactlyEqual(CollisionInfo.Right) || collisionType.ExactlyEqual(CollisionInfo.Left)) && inputRaw.x != 0;
        var wallJumping = wallSliding && Input.GetButtonDown("Jump");

        rb.velocity = Move(inputRaw, jumping, wallSliding, wallJumping);
    }

    private Vector2 Move (Vector2 inputRaw, bool jumping, bool wallSliding, bool wallJumping) {

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
        if ((collisionType == CollisionInfo.Right && direction.x == 1) || (collisionType == CollisionInfo.Left && direction.x == -1)) {
            velocity.x = 0;
        }

        return velocity;
    }

    #region Movement Methods

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

}