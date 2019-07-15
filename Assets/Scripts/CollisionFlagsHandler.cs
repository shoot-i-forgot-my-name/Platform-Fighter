using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class CollisionFlagsHandler : MonoBehaviour {

    public CollisionFlags collisionFlags { get; private set; } = CollisionFlags.None; // CollisionFlags
    private static float limAngle = 45; // Limiting angle

    private void OnCollisionEnter2D (Collision2D Collision) {
        var normal = Collision.GetContact(0).normal; // Get normal
        var angle = Mathf.Rad2Deg * Mathf.Asin(normal.y); // Calculate angle

        // Get collision flags
        if (angle < -limAngle) {
            collisionFlags = CollisionFlags.Above;
        } else if (angle > limAngle) {
            collisionFlags = CollisionFlags.Below;
        } else {
            collisionFlags = CollisionFlags.Sides;
        }
    }

    private void OnCollisionExit2D () {
        // Get collision flags
        collisionFlags = CollisionFlags.None;
    }

}
