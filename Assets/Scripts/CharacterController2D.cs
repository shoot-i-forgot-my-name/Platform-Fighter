using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    #region Variables

    [HideInInspector]
    public new BoxCollider2D collider { get; private set; } = null;
    [HideInInspector]
    public Vector2 velocity = Vector2.zero;
    [HideInInspector]
    public bool grounded { get; private set; } = false;

    #endregion

    #region Methods

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public void Move()
    {
        transform.Translate(velocity * Time.deltaTime);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, collider.size, 0);

        // Reset boolean
        grounded = false;

        foreach (Collider2D collider in colliders)
        {
            if (collider == this.collider)
            {
                continue;
            }

            ColliderDistance2D separation = collider.Distance(this.collider);

            if (separation.isOverlapped)
            {
                // Move the player by the seperation
                transform.Translate(separation.pointA - separation.pointB);

                float surfaceAngle = Vector2.Angle(separation.normal, Vector2.up);

                if (surfaceAngle < 90 && velocity.y < 0)
                {
                    grounded = true;
                }
            }
        }
    }

    #endregion
}
