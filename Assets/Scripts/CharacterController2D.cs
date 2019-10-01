using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Collider of the player
    /// </summary>
    [HideInInspector]
    public BoxCollider2D Collider { get; private set; } = null;

    /// <summary>
    /// Velocity of the player
    /// </summary>
    [HideInInspector]
    public Vector2 Velocity = Vector2.zero;

    /// <summary>
    /// If the player is on the ground
    /// </summary>
    [HideInInspector]
    public bool Grounded { get; private set; } = false;

    #endregion

    #region Methods

    private void Start()
    {
        Collider = GetComponent<BoxCollider2D>();
    }

    public void Move()
    {
        transform.Translate(Velocity * Time.deltaTime);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, Collider.size, 0);

        Grounded = false;

        foreach (Collider2D collider in colliders)
        {
            if (collider == this.Collider)
            {
                continue;
            }

            ColliderDistance2D separation = collider.Distance(this.Collider);

            if (separation.isOverlapped)
            {
                // Move the player by the seperation (Direction and Distance)
                transform.Translate(separation.pointA - separation.pointB);

                float surfaceAngle = Vector2.Angle(separation.normal, Vector2.up);

                if (surfaceAngle < 90 && Velocity.y < 0)
                {
                    Grounded = true;
                }
            }
        }
    }

    #endregion
}
