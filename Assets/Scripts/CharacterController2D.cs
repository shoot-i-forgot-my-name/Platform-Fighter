using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{

    private new BoxCollider2D collider = null;

    private Vector2 velocity = Vector2.zero;
    private bool collidingBelow = false;

    [Tooltip("Speed of the player when running"), Header("Speed values")]
    public float runSpeed = 5;
    [Tooltip("Speed of the player when walking")]
    public float walkSpeed = 3;

    [Tooltip("Acceleration of the player"), Header("Changing speeds")]
    public float acceleration = 5;
    [Tooltip("Deceleration of the player")]
    public float deceleration = 5;
    [Tooltip("Percentage of acceleration when on air"), Range(0, 1)]
    public float airPercentage = 1;

    [Tooltip("Jump height of the player"), Header("Jumping")]
    public float jumpHeight = 5;

    [Header("Wall Jumping")]
    public Vector2 jumpForce = Vector2.zero;

    private void Start ()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    private void Update ()
    {

        #region Movement

        {
            var input = Input.GetAxisRaw("Horizontal");
            var running = Input.GetKey(KeyCode.LeftShift);

            var acceleration = this.acceleration * (collidingBelow ? 1 : airPercentage);
            var deceleration = collidingBelow ? this.deceleration : 0;

            if (input == 0)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, GetSpeed(running) * input, acceleration * Time.deltaTime);
            }
        }

        #endregion

        // Adding gravity
        velocity.y += Physics2D.gravity.y * Time.deltaTime;

        #region Jumping

        if (collidingBelow)
        {

            velocity.y = 0;

            if (Input.GetButton("Jump"))
            {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }

        }

        #endregion

        #region Collision Resolution, Updating Grounded and Wall Jumping

        collidingBelow = false;

        {
            var hits = Physics2D.OverlapBoxAll(transform.position, collider.size, 0);

            foreach (Collider2D hit in hits)
            {
                // The same collider?
                if (hit == collider)
                {
                    continue;
                }

                var colliderDistance = hit.Distance(collider);

                // Tells us if both colliders are touching
                if (colliderDistance.isOverlapped)
                {
                    transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                    var surfaceAngle = Vector2.Angle(colliderDistance.normal, Vector2.up);

                    if (surfaceAngle < 90 && velocity.y < 0)
                    {
                        collidingBelow = true;
                    }

                    #region Wall Jumping

                    if (surfaceAngle >= 85 && surfaceAngle <= 95 && velocity.x != 0)
                    {
                        if (Input.GetButton("Jump"))
                        {
                            velocity.y = jumpForce.y;
                            velocity.x = -Mathf.Sign(velocity.x) * jumpForce.x;
                        }
                    }

                    #endregion
                }
            }
        }

        #endregion

        transform.Translate(velocity * Time.deltaTime);
    }

    private float GetSpeed (bool running)
    {
        if (running)
        {
            return runSpeed;
        }

        return walkSpeed;
    }

}
