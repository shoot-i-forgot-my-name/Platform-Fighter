﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CollisionFlagsHandler), typeof(InputManager))]
public class MovementBasic : MonoBehaviour {

    #region Variables

    private Rigidbody2D rb; // Reference to Rigidbody2D
    private CollisionFlagsHandler cf; // Reference to CollisionFlagsHandler
    [HideInInspector]
    public float curSpeed; // Current speed of the player
    public InputManager im; // Reference to InputManager

    public float moveSpeed = 10; // Default movement speed
    public float smoothStop = 10f; // Time for the character to stop moving
    public float smoothMove = 7.5f; // Time for the character to start moving

    public float jumpHeight = 37.5f; // Jump height of the character; Dependent on gravity

    public float airMovement = .75f; // Percentage of the movement when on air

    #endregion

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
        cf = GetComponent<CollisionFlagsHandler>();
    }

    private void Update () {
        var jumping = Input.GetButton("Jump") && cf.collisionFlags == CollisionFlags.Below;

        JumpPlayer(jumping); // Call this regardless of input

        var xMovement = GetMoveX(im.rawInput, im.modifiedInput); // Get movement on x-axis

        if (!(cf.collisionFlags == CollisionFlags.Below)) {
            xMovement.x *= airMovement; // Percentage of movement when on air
        }

        transform.Translate(xMovement * Time.deltaTime); // Move the player
    }

    private Vector2 GetMoveX (Vector2 inputRaw, Vector2 inputModified) {
        {
            var smoothTime = (inputRaw.x == 0) ? smoothStop : smoothMove; // Either smoothStop or smoothMove
            var targetSpeed = (inputRaw.x == 0) ? 0 : moveSpeed; // Either 0 or the default speed
            curSpeed = Mathf.Lerp(curSpeed, targetSpeed, smoothTime * Time.deltaTime); // Lerp for smooth movement
        }

        return Vector2.right * inputModified * curSpeed; // Return final value
    }

    private void JumpPlayer (bool jumping) {
        if (jumping) {
            // Assuming gravity is negative
            // y-velocity = jumpHeight / gravity * -2
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight / Physics2D.gravity.y * -2);
        }
    }

}
