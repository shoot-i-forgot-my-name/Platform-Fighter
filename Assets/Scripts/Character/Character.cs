using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D)), RequireComponent(typeof(InputManager), typeof(AnimationHandler))]
public class Character : MonoBehaviour
{
    #region Variables

    private const float limAngle = 45;
    private Rigidbody2D rb;
    /// <summary>
    /// Successful when collision exits; resets after another collision exit
    /// </summary>
    private Action<Vector2> recalculateCollisionType;

    [HideInInspector]
    public CollisionInfo CollisionInfo { get; private set; } = CollisionInfo.None;

    #endregion Variables

    private void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        recalculateCollisionType = Utility.CallActionOnce<Vector2>(UpdateCollisionType);
    }

    #region Update CollisionInfo

    private void UpdateCollisionType (Vector2 normal)
    {
        var aboveBottom = Mathf.Rad2Deg * Mathf.Asin(normal.y); // Calculate angle
        var rightLeft = Mathf.Rad2Deg * Mathf.Asin(normal.x); // Calculate angle

        // Get collision types
        if (aboveBottom < -limAngle)
        {
            CollisionInfo += CollisionInfo.Above;
        }

        if (aboveBottom > limAngle)
        {
            CollisionInfo += CollisionInfo.Below;
        }

        if (rightLeft < -limAngle)
        {
            CollisionInfo += CollisionInfo.Right;
        }

        if (rightLeft > limAngle)
        {
            CollisionInfo += CollisionInfo.Left;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {
        UpdateCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionStay2D (Collision2D collision)
    {
        recalculateCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionExit2D ()
    {
        recalculateCollisionType = Utility.CallActionOnce<Vector2>(UpdateCollisionType);
        CollisionInfo = CollisionInfo.None;
    }

    #endregion Update CollisionInfo

    #region Public Methods

    #region Velocity Methods

    public void SetVelocity (Vector2 velocity)
    {
        rb.velocity = velocity;
    }

    public void SetVelocityX (float x)
    {
        rb.velocity = new Vector2(x, rb.velocity.y);
    }

    public void SetVelocityY (float y)
    {
        rb.velocity = new Vector2(rb.velocity.x, y);
    }

    public Vector2 GetVelocity ()
    {
        return rb.velocity;
    }

    public void AddForce (Vector2 velocity, ForceMode2D mode)
    {
        rb.AddForce(velocity, mode);
    }

    #endregion

    #endregion

}