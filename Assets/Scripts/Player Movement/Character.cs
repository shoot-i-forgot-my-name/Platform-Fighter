using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Character : MonoBehaviour
{
    #region Variables

    private const float LimAngle = 45;

    private Rigidbody2D _rb;
    private readonly List<float?[]> _velocitySetList = new List<float?[]>();

    private Action<Vector2> _recalculateCollisionType; // Successful when collision exits; resets after another collision exit

    [HideInInspector]
    public CollisionInfo CollisionInfo { get; private set; } = CollisionInfo.None;

    #endregion Variables

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _recalculateCollisionType = Utility.CallActionOnce<Vector2>(UpdateCollisionType);
    }

    private void Update()
    {
        foreach (var item in _velocitySetList)
        {
            _rb.velocity = new Vector2(item[0] ?? _rb.velocity.x, item[1] ?? _rb.velocity.y);
        }

        _velocitySetList.Clear();
    }

    #region Update CollisionInfo

    private void UpdateCollisionType(Vector2 normal)
    {
        var aboveBottom = Mathf.Rad2Deg * Mathf.Asin(normal.y); // Calculate angle
        var rightLeft = Mathf.Rad2Deg * Mathf.Asin(normal.x); // Calculate angle

        // Get collision types
        if (aboveBottom < -LimAngle)
        {
            CollisionInfo += CollisionInfo.Above;
        }

        if (aboveBottom > LimAngle)
        {
            CollisionInfo += CollisionInfo.Below;
        }

        if (rightLeft < -LimAngle)
        {
            CollisionInfo += CollisionInfo.Right;
        }

        if (rightLeft > LimAngle)
        {
            CollisionInfo += CollisionInfo.Left;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        _recalculateCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionExit2D()
    {
        _recalculateCollisionType = Utility.CallActionOnce<Vector2>(UpdateCollisionType);
        CollisionInfo = CollisionInfo.None;
    }

    #endregion Update CollisionInfo

    #region GetVelocity methods

    public Vector2 GetVelocity ()
    {
        var topmostVector = _velocitySetList[_velocitySetList.Count];

        var x = topmostVector[0] ?? _rb.velocity.x;
        var y = topmostVector[1] ?? _rb.velocity.y;

        if (_velocitySetList.Count >= 2)
        {
            var secondmostVector = _velocitySetList[_velocitySetList.Count - 1];
            x = secondmostVector[0] ?? x;
            y = secondmostVector[1] ?? y;
        }

        return new Vector2(x, y);
    }

    public Vector2 GetRawVelocity ()
    {
        return _rb.velocity;
    }

    #endregion

    #region SetVelocity methods

    public void SetVelocity(float? x, float? y)
    {
        _velocitySetList.Add(new float?[2] { x, y });
    }

    public void SetVelocity(Vector2 vector)
    {
        _velocitySetList.Add(new float?[2] { vector.x, vector.y });
    }

    public void SetVelocityX(float? x)
    {
        _velocitySetList.Add(new float?[2] { x, null });
    }

    public void SetVelocityY(float? y)
    {
        _velocitySetList.Add(new float?[2] { null, y });
    }

    #endregion SetVelocity methods

    #region AddForce methods

    public void AddForce(Vector2 vector, ForceMode2D forceMode)
    {
        _rb.AddForce(vector, forceMode);
    }

    public void AddForce(Vector2 vector)
    {
        AddForce(vector, ForceMode2D.Force);
    }

    public void AddForce(float x, float y, ForceMode2D forceMode)
    {
        AddForce(new Vector2(x, y), forceMode);
    }

    public void AddForce(float x, float y)
    {
        AddForce(x, y, ForceMode2D.Force);
    }

    public void AddForceX(float x, ForceMode2D forceMode)
    {
        AddForce(x, 0, forceMode);
    }

    public void AddForceX(float x)
    {
        AddForceX(x, ForceMode2D.Force);
    }

    public void AddForceY(float y, ForceMode2D forceMode)
    {
        AddForce(y, forceMode);
    }

    public void AddForceY(float y)
    {
        AddForceY(y, ForceMode2D.Force);
    }

    public void AddForce(float n, ForceMode2D forceMode)
    {
        AddForce(n, n, forceMode);
    }

    public void AddForce(float n)
    {
        AddForce(n, n, ForceMode2D.Force);
    }

    #endregion AddForce methods
}
