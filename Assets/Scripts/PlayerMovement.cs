using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour {

    #region Static Variables

    public static readonly float limAngle = 45; // Limiting angle

    #endregion

    #region Private Variables

    private CollisionType collideType = CollisionType.None; // CollisionFlags alternative
    private Rigidbody2D rb; // Reference to rigidbody
    private Vector2 inputModified; // Input that only accepts 1 or -1

    private Action<Vector2> OnceCollisionType; // Recalculates the CollisionType of the gameObject; Done once on exit of collision, resets
    private Func<Vector2, Vector2, Vector2> OnceReturnVelocity; // Helper for clinging walls; Done once on start of clinging walls, resets
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
    [Header("Wall Clinging")]
    public float gravityScale = .75f; // Percentage of the mass when colliding with a wall
    public float yVelocityStart = .025f; // Starting velocity of the gameObject when clinging

    #endregion

    #region Private Methods

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
        defaultGravScale = rb.gravityScale;
        OnceCollisionType = CallOnlyOnce<Vector2>(UpdateCollisionType);
        OnceReturnVelocity = CallOnlyOnce((Vector2 a) => a );
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

        velocity.x = inputModified.x * curSpeed * ((collideType == CollisionType.None) ? airMovement : 1); // Percentage of movement when on air

        // This fixes not falling while moving to the right or left AND colliding with a wall
        print((collideType == CollisionType.Left && inputModified.x == -1));
        if ((collideType == CollisionType.Right && inputModified.x == 1) || (collideType == CollisionType.Left && inputModified.x == -1)) {
            velocity.x = 0;
        }

        #region Clinging Walls

        if (clingingWalls) {
            if (Mathf.Sign(velocity.y) == -1)
                rb.gravityScale = defaultGravScale * gravityScale;

            velocity = OnceReturnVelocity(new Vector2(velocity.x, -yVelocityStart), velocity);
        }else {
            OnceReturnVelocity = CallOnlyOnce((Vector2 a) => a);
        }

        #endregion

        if (jumping) {
            velocity.y = jumpForce;
        }

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
        OnceCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionExit2D () {
        collideType -= CollisionType.None;
        OnceCollisionType = CallOnlyOnce<Vector2>(UpdateCollisionType);
    }

    public Action<T> CallOnlyOnce <T> (Action<T> action) {
        var contextCalled = false;
        Action<T> ret = (T param) => {
            if (!contextCalled) {
                action(param);
                contextCalled = true;
            }
        };

        return ret;
    }

    public Func<T, TResult, TResult> CallOnlyOnce<T, TResult> (Func<T, TResult> func) {
        var contextCalled = false;

        Func<T, TResult, TResult> @return = (T param, TResult failParam) => {
            if (!contextCalled) {
                contextCalled = true;
                return func(param);
            }

            return failParam;
        };

        return @return;
    }


    #endregion

}