using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour {

    #region Static Variables

    public static float limAngle = 45; // Limiting angle

    #endregion

    #region Variables

    private int calculateCounter = 2; // Dictates whenever we should recalculate on next collision
    private CollisionType collideType = CollisionType.None; // CollisionFlags alternative
    private Rigidbody2D rb; // Reference to rigidbody
    [HideInInspector]
    public float curSpeed; // Current speed of the player
    private Vector2 inputModified; // Input that only accepts 1 or -1

    public float moveSpeed = 10; // Default movement speed
    public float smoothStop = 10f; // Time for the character to stop moving
    public float smoothMove = 7.5f; // Time for the character to start moving

    public float jumpHeight = 37.5f; // Jump height of the character; Dependent on gravity

    public float airMovement = .75f; // Percentage of the movement when on air

    #endregion

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update () {
        var inputRaw = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputModified.x = (inputRaw.x == 0) ? inputModified.x : inputRaw.x; // Accepts -1 or 1
        inputModified.y = (inputRaw.y == 0) ? inputModified.y : inputRaw.y; // Accepts -1 or 1

        var jumping = Input.GetButton("Jump") && collideType == CollisionType.Below;

        JumpPlayer(jumping); // Call this regardless of input

        var xMovement = GetMoveX(inputRaw); // Get movement on x-axis

        if (!(collideType == CollisionType.Below)) {
            xMovement.x *= airMovement; // Percentage of movement when on air
        }

        transform.Translate(xMovement * Time.deltaTime); // Move the player
    }

    private Vector2 GetMoveX (Vector2 inputRaw) {
        {
            var smoothTime = (inputRaw.x == 0) ? smoothStop : smoothMove; // Either smoothStop or smoothMove
            var targetSpeed = (inputRaw.x == 0) ? 0 : moveSpeed; // Either 0 or the default speed
            curSpeed = Mathf.Lerp(curSpeed, targetSpeed, smoothTime * Time.deltaTime); // Lerp for smooth movement
        }

        return Vector2.right * inputModified * curSpeed; // Return final value
    }

    private void JumpPlayer (bool jumping) {
        if (jumping) {
            // Assuming gravity is negative
            // y-velocity = jumpHeight / gravity * -2
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight / Physics2D.gravity.y * -2);
        }
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
        collideType = CollisionType.None;
    }

}
