using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour {

    #region Static Variables

    public static readonly float limAngle = 45; // Limiting angle

    #endregion

    #region Private Variables

    private CollisionType collideType = CollisionType.None; // CollisionFlags alternative
    private Rigidbody2D rb; // Reference to rigidbody
    private Vector2 inputModified; // Input that only accepts 1 or -1

    private int calculateCounter = 2; // Dictates whenever we should recalculate on next collision
    private Vector2 velocity = Vector2.zero; // Velocity of the gameObject
    private float defaultGravScale; // Default gravity scale of the gameObject

    #endregion

    #region Public Variables

    [HideInInspector]
    public float curSpeed; // Current speed of the player

    [Header("Basic Movement")]
    public float moveSpeed = 10; // Default movement speed
    public float smoothStop = 10; // Time for the character to stop moving
    public float smoothMove = 7.5f; // Time for the character to start moving
    public float airMovement = .75f; // Percentage of the movement when on air
    [Header("Jumping")]
    public float jumpForce = 10; // Jump height of the character; Dependent on gravity
    [Header("Wall Movement")]
    public float gravityScale = .025f; // Percentage of the mass when colliding with a wall

    #endregion

    #region Private Methods

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
        defaultGravScale = rb.gravityScale;
    }

    private void Update () {
        var inputRaw = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputModified.x = (inputRaw.x == 0) ? inputModified.x : inputRaw.x; // Accepts -1 or 1
        inputModified.y = (inputRaw.y == 0) ? inputModified.y : inputRaw.y; // Accepts -1 or 1

        var jumping = Input.GetButton("Jump") && collideType == CollisionType.Below;
        var clingingWalls = collideType == CollisionType.Right + CollisionType.Left && inputRaw.x != 0;

        rb.velocity = Move(inputRaw, jumping, clingingWalls);
    }

    private Vector2 Move (Vector2 inputRaw, bool jumping, bool clingingWalls) {
        velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        rb.gravityScale = defaultGravScale;

        {
            var smoothTime = (inputRaw.x == 0) ? smoothStop : smoothMove; // Either smoothStop or smoothMove
            var targetSpeed = (inputRaw.x == 0) ? 0 : moveSpeed; // Either 0 or the default speed
            curSpeed = Mathf.Lerp(curSpeed, targetSpeed, smoothTime * Time.deltaTime); // Lerp for smooth movement
        }

        #region Velocity

        velocity.x = inputModified.x * curSpeed * ((collideType == CollisionType.None) ? airMovement : 1); // Percentage of movement when on air

        // This fixes not falling while moving to the right or left AND colliding with a wall
        if ((collideType == CollisionType.Right && inputModified.x == 1) || (collideType == CollisionType.Left && inputModified.x == -1)) {
            velocity.x = 0;
        }

        if ((collideType == CollisionType.Right && inputRaw.x == 1) || (collideType == CollisionType.Left && inputRaw.x == -1)) {
            velocity.y = Mathf.Sign(velocity.y) == -1 ? velocity.y : 0.0005f;
            rb.gravityScale = defaultGravScale * gravityScale;
        }

        if (jumping) {
            velocity.y = jumpForce;
        }

        #endregion

        return velocity; // Return final value
    }

    private void UpdateCollisionType (Vector2 normal) {
        var aboveBottom = Mathf.Rad2Deg * Mathf.Asin(normal.y); // Calculate angle
        var rightLeft = Mathf.Rad2Deg * Mathf.Asin(normal.x); // Calculate angle

        // Get collision types
        if (aboveBottom < -limAngle) {
            collideType += CollisionType.Above;
        }

        if (aboveBottom > limAngle) {
            collideType += CollisionType.Below;
        }

        if (rightLeft < -limAngle) {
            collideType += CollisionType.Right;
        }

        if (rightLeft > limAngle) {
            collideType += CollisionType.Left;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision) {
        UpdateCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionStay2D (Collision2D collision) {
        if (calculateCounter == 0) {
            UpdateCollisionType(collision.GetContact(0).normal);
            calculateCounter++;
        }
    }

    private void OnCollisionExit2D () {
        calculateCounter--;
        collideType -= CollisionType.None;
    }

    #endregion

}
