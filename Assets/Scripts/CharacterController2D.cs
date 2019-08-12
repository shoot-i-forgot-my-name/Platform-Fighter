using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    private new BoxCollider2D collider = null;
    private Vector2 velocity = Vector2.zero;

    /// <summary>
    /// If the player is on the ground
    /// </summary>
    private bool grounded = false;

    private Vector2 wCurrentJumpingForce = Vector2.zero;

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
    public bool wJumpingEnabled = true;

    public Vector2 wJumpingForce = Vector2.zero;

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

            var acceleration = this.acceleration * (grounded ? 1 : airPercentage);
            var deceleration = grounded ? this.deceleration : 0;

            if (input == 0)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, GetSpeed(running) * input, acceleration * Time.deltaTime);
            }
        }

        #endregion Movement

        #region Jumping, gravity handling, wall jumping and clinging

        if (grounded)
        {
            velocity.y = 0;

            #region Jumping

            if (Input.GetButton("Jump"))
            {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }

            #endregion Jumping
            
            wCurrentJumpingForce = wJumpingForce;
        }
        else
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;

            var hits = Physics2D.OverlapBoxAll(transform.position, collider.size, 0);

            if (hits.Length > 1)
            {
                var separation = hits[1].Distance(collider);
                // Get angle of the surface
                var surfaceAngle = Vector2.Angle(separation.normal, Vector2.up);

                // Test if the colliding collider is an actual wall
                if (surfaceAngle >= 85 && surfaceAngle <= 95)
                {
                    #region Wall Jumping

                    if (Input.GetButton("Jump") && wJumpingEnabled && wCurrentJumpingForce.y > 0.5f)
                    {
                        // Turn the direction of the wall jumping force to the opposite of the normal
                        wCurrentJumpingForce.x *= -Mathf.Sign(separation.normal.x);
                        velocity = wCurrentJumpingForce;
                        wCurrentJumpingForce /= 1.75f;
                    }

                    #endregion Wall Jumping
                }
            }
        }

        #endregion Jumping, gravity handling, wall jumping and clinging

        transform.Translate(velocity * Time.deltaTime);

        #region Collision solver and grounded updater

        grounded = false;

        {
            // This overlap also includes the collider itself
            var hits = Physics2D.OverlapBoxAll(transform.position, collider.size, 0);

            foreach (var hit in hits)
            {
                // If it is the same collider
                if (hit == collider)
                {
                    continue;
                }

                var separation = hit.Distance(collider);

                // Tells us if both colliders are touching
                if (separation.isOverlapped)
                {
                    // Move the player by the distance and direction
                    transform.Translate(separation.pointA - separation.pointB);

                    #region Grounded updater

                    // Get the angle of the surface
                    var surfaceAngle = Vector2.Angle(separation.normal, Vector2.up);

                    if (surfaceAngle < 90 && velocity.y < 0)
                    {
                        grounded = true;
                    }

                    #endregion Grounded updater
                }
            }
        }

        #endregion Collision solver and grounded updater
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