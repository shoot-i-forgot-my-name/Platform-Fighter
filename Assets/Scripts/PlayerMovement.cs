using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;

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
    private bool basicMovement = true;

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
    public float jumpForce = 15; // Jump height of the character; Dependent on gravity
    [Header("Wall Clinging")]
    public bool wallCling = true;
    public float gravityScale = .75f; // Percentage of the mass when colliding with a wall
    public float yVelocityStart = .025f; // Starting velocity of the gameObject when clinging
    public float pushOffX = 30; 
    public float pushOffY = 30;
    public int milesecondTimeout = 500;

    #endregion

    #region Private Methods

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
        defaultGravScale = rb.gravityScale;
        OnceCollisionType = CallActionOnce<Vector2>(UpdateCollisionType);
        OnceReturnVelocity = CallFuncOnce((Vector2 a) => a );
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
        #region Initialize velocity

        velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        rb.gravityScale = defaultGravScale;

        #endregion

        #region Moving in x-axis
        if (basicMovement) {

            {
                var smoothTime = (inputRaw.x == 0) ? smoothStop : smoothMove; // Either smoothStop or smoothMove
                var targetSpeed = (inputRaw.x == 0) ? 0 : moveSpeed; // Either 0 or the default speed
                curSpeed = Mathf.Lerp(curSpeed, targetSpeed, smoothTime * Time.deltaTime); // Lerp for smooth movement
            }

            velocity.x = inputModified.x * curSpeed * ((collideType == CollisionType.None) ? airMovement : 1); // Percentage of movement when on air

            // This fixes not falling while moving to the right or left AND colliding with a wall
            if ((collideType == CollisionType.Right && inputRaw.x == 1) || (collideType == CollisionType.Left && inputRaw.x == -1)) {
                velocity.x = 0;
            }

        }

        #endregion

        #region Clinging walls

        if (wallCling) {
            if (clingingWalls) {
                velocity = OnceReturnVelocity(new Vector2(velocity.x, -yVelocityStart), velocity);

                if (Mathf.Sign(velocity.y) == -1) {
                    rb.gravityScale = defaultGravScale * gravityScale;
                }

                if (Input.GetButtonDown("Jump")) {
                    velocity.x = -inputModified.x * pushOffX;
                    velocity.y = pushOffY;
                    
                    // Refactor this somehow
                    basicMovement = false;
                    Timer tmr = new Timer();
                    tmr.Interval = milesecondTimeout;
                    tmr.Elapsed += (object sender, ElapsedEventArgs e) => {
                        basicMovement = true;
                        tmr.Stop();
                    };

                    tmr.Start();
                }

            } else {
                // Reset when calling once
                OnceReturnVelocity = CallFuncOnce((Vector2 a) => a);
            }
        }

        #endregion

        #region Jumping

        if (jumping) {
            velocity.y = jumpForce;
        }

        #endregion

        return velocity; // Return final value
    }

    private void Tmr_Elapsed (object sender, ElapsedEventArgs e) {
        throw new NotImplementedException();
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
        OnceCollisionType = CallActionOnce<Vector2>(UpdateCollisionType);
    }

    public Action<T> CallActionOnce <T> (Action<T> action) {
        var contextCalled = false;
        Action<T> ret = (T param) => {
            if (!contextCalled) {
                action(param);
                contextCalled = true;
            }
        };

        return ret;
    }

    public Func<T, TResult, TResult> CallFuncOnce <T, TResult> (Func<T, TResult> func) {
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

    public Func<T, TResult> CallFuncOnce <T, TResult> (Func<T, TResult> func, TResult failParam) {
        var contextCalled = false;

        Func<T, TResult> @return = (T param) => {
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