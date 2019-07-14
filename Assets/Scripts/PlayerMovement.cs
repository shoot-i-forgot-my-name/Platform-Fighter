using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour {

    #region Static Variables

    CollisionFlags collisionFlags = CollisionFlags.None;
    public static float limAngle = 45;

    #endregion

    #region Variables

    private Rigidbody2D rb;
    private float curSpeed;
    private Vector2 inputModified;

    public float moveSpeed = 10;
    public float smoothStop = .5f;
    public float smoothMove = .5f;

    public float jumpHeight = 1;

    public float airMovement = .5f;

    #endregion

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update () {
        var inputRaw = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputModified.x = (inputRaw.x == 0) ? inputModified.x : inputRaw.x;
        inputModified.y = (inputRaw.y == 0) ? inputModified.y : inputRaw.y;

        var jumping = Input.GetButton("Jump") && collisionFlags == CollisionFlags.Below;

        JumpPlayer(jumping);

        var xMovement = GetMoveX(inputRaw);

        if (!(collisionFlags == CollisionFlags.Below)) {
            xMovement.x *= airMovement;
        }

        print(xMovement.x);

        transform.Translate(xMovement * Time.deltaTime);
    }

    private Vector2 GetMoveX (Vector2 inputRaw) {
        {
            var smoothTime = (inputRaw.x == 0) ? smoothStop : smoothMove;
            var targetSpeed = (inputRaw.x == 0) ? 0 : moveSpeed;
            curSpeed = Mathf.Lerp(curSpeed, targetSpeed, smoothTime * Time.deltaTime);
        }

        return Vector2.right * inputModified * curSpeed;
    }

    private void JumpPlayer (bool jumping) {
        if (jumping) {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight / Physics2D.gravity.y * -2);
        }
    }

    private void OnCollisionEnter2D (Collision2D Collision) {
        var normal = Collision.GetContact(0).normal;
        var angle = Mathf.Rad2Deg * Mathf.Asin(normal.y);

        if (angle < -limAngle) {
            collisionFlags = CollisionFlags.Above;
        } else if (angle > limAngle) {
            collisionFlags = CollisionFlags.Below;
        } else {
            collisionFlags = CollisionFlags.Sides;
        }
    }

    private void OnCollisionExit2D (Collision2D collision) {
        collisionFlags = CollisionFlags.None;
    }

}
