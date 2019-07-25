using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Player : MonoBehaviour {

    #region Variables

    private static float limAngle = 45;

    private List<Priority<float?[]>> priorityQueue = new List<Priority<float?[]>>();

    private Rigidbody2D rb { get; set; }
    private Action<Vector2> recalculateCollisionType { get; set; } // Succesful when collision exits; resets after another collision exit

    [HideInInspector]
    public CollisionInfo collisionInfo;
    [HideInInspector]
    public Vector2 rawInput { get; private set; }
    [HideInInspector]
    public Vector2 modifiedInput { get; private set; }

    #endregion Variables

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update () {
        rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        {
            var x = rawInput.x == 0 ? modifiedInput.x : rawInput.x;
            var y = rawInput.y == 0 ? modifiedInput.y : rawInput.y;
            modifiedInput = new Vector2(x, y);
        }

        for (int i = 0; i < priorityQueue.Count; i++) {
            var x = priorityQueue[i].value[0] ?? rb.velocity.x;
            var y = priorityQueue[i].value[1] ?? rb.velocity.y;

            rb.velocity = new Vector2(x, y);
        }

        priorityQueue.Clear();
    }

    #region Set Collision Info

    private void UpdateCollisionType (Vector2 normal) {
        var aboveBottom = Mathf.Rad2Deg * Mathf.Asin(normal.y); // Calculate angle
        var rightLeft = Mathf.Rad2Deg * Mathf.Asin(normal.x); // Calculate angle

        // Get collision types
        if (aboveBottom < -limAngle) {
            collisionInfo += CollisionInfo.Above;
        }

        if (aboveBottom > limAngle) {
            collisionInfo += CollisionInfo.Below;
        }

        if (rightLeft < -limAngle) {
            collisionInfo += CollisionInfo.Right;
        }

        if (rightLeft > limAngle) {
            collisionInfo += CollisionInfo.Left;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision) {
        UpdateCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionStay2D (Collision2D collision) {
        recalculateCollisionType(collision.GetContact(0).normal);
    }

    private void OnCollisionExit2D () {
        recalculateCollisionType = Utility.CallActionOnce<Vector2>(UpdateCollisionType);
        collisionInfo = CollisionInfo.None;
    }

    #endregion Set Collision Info

    private int GetIndex (Priority<float?[]> target, int index) {
        try {
            // NOTE: priorityIndex zero (0) is the nearest

            // 'target' has more/equal priority
            if (target.priorityIndex <= priorityQueue[index].priorityIndex) {
                return index;
            } else { // 'target' has lesser priority
                return GetIndex(target, index - 1);
            }
        }
        catch {
            if (target.priorityIndex >= priorityQueue[priorityQueue.Count - 1].priorityIndex) {
                return priorityQueue.Count;
            } else {
                return GetIndex(target, index - 1);
            }
        }
    }

    public void SetVelocity (float? x, float? y, int priority) {
        var target = new Priority<float?[]>(new float?[2] { x, y }, priorityQueue.Count);
        var index = GetIndex(target, priority);

        priorityQueue.Insert(index, target);
    }

    public void SetVelocity (Vector2 vector, int priority) {
        SetVelocity(vector.x, vector.y, priority);
    }

    public void SetVelocity (float? n, int priority) {
        SetVelocity(n, n, priority);
    }

    private struct Priority<T> {
        public T value { get; set; }
        public int priorityIndex { get; }

        public Priority (T value, int priorityIndex) {
            this.value = value;
            this.priorityIndex = priorityIndex;
        }
    }
}