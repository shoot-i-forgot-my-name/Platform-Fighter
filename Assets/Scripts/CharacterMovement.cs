using System.Collections;
using System.Collections.Generic;
using Serializable = System.SerializableAttribute;
using UnityEngine;

/// See the Input Manager to edit the buttons or edit the code

[RequireComponent(typeof(CharacterController2D))]
public class CharacterMovement : MonoBehaviour
{

    #region Variables

    /// <summary>
    /// Reference to the controller
    /// </summary>
    private CharacterController2D controller;

    #region Movement

    [Tooltip("Speed of the player when running"), Header("Speed values")]
    public float runSpeed = 5;

    [Tooltip("Speed of the player when walking")]
    public float walkSpeed = 3;

    [Tooltip("Acceleration of the player"), Header("Changing speeds")]
    public float groundAcceleration = 5;

    [Tooltip("Deceleration of the player")]
    public float groundDeceleration = 5;

    [Tooltip("Acceleration of the player when on the air")]
    public float airAcceleration = 1;

    #endregion Movement

    #region Jumping

    [Tooltip("Jump height of the player")]
    public float jumpHeight = 1;

    #endregion Jumping

    #region Wall Jumping

    [SerializeField]
    private WallJumping wJumping = new WallJumping(Vector2.zero);

    #endregion Wall Jumping

    #endregion Variables

    private void Start()
    {
        controller = GetComponent<CharacterController2D>();
    }

    private void Update()
    {
        // A and D keys or left and right arrow keys
        float input = Input.GetAxisRaw("Horizontal");
        bool running = Input.GetKey(KeyCode.LeftShift);
        bool jumping = Input.GetButton("Jump");

        Move(input, running);

        if (controller.Grounded)
        {
            controller.Velocity.y = 0;

            // Let's reset count when the player lands on the ground
            wJumping.ResetCount();
            Jump(jumping);
        }
        else
        {
            controller.Velocity.y += Physics2D.gravity.y * Time.deltaTime;

            wJumping.JumpOff(controller, jumping);
        }

        controller.Move();
    }

    private void Jump(bool jumping)
    {
        if (jumping)
        {
            controller.Velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        }
    }

    private void Move(float input, bool running)
    {
        float acceleration = controller.Grounded ? groundAcceleration : airAcceleration;
        float deceleration = controller.Grounded ? groundDeceleration : 0;

        if (input == 0)
        {
            controller.Velocity.x = Mathf.MoveTowards(controller.Velocity.x, 0, deceleration * Time.deltaTime);
        }
        else
        {
            controller.Velocity.x = Mathf.MoveTowards(controller.Velocity.x, (running ? runSpeed : walkSpeed) * input, acceleration * Time.deltaTime);
        }
    }

    [Serializable]
    private class WallJumping
    {
        /// <summary>
        /// Stages of jumping off walls force. Starts from 0 and ends at the last index
        /// </summary>
        [SerializeField]
        private Vector2[] _stages = new Vector2[1] { Vector2.zero };

        [HideInInspector]
        public int JumpCount { get; private set; } = 0;

        public WallJumping (params Vector2[] stages)
        {
            _stages = stages;
        }

        public void JumpOff (CharacterController2D controller, bool jumping)
        {
            if (!jumping)
            {
                return;
            }

            Collider2D[] hits = Physics2D.OverlapBoxAll(controller.transform.position, controller.Collider.size, 0);

            foreach (Collider2D hit in hits)
            {
                if (hit == controller.Collider || JumpCount >= _stages.Length)
                {
                    continue;
                }

                ColliderDistance2D seperation = hit.Distance(controller.Collider);

                // Is it actually collding?
                if (seperation.isOverlapped)
                {
                    float surfaceAngle = Vector2.Angle(seperation.normal, Vector2.up);

                    if (surfaceAngle >= 90 && surfaceAngle <= 92.5)
                    {
                        Vector2 takenStage = _stages[JumpCount];

                        if (!Mathf.Approximately(takenStage.x, 0))
                        {
                            controller.Velocity.x = takenStage.x * seperation.normal.x;
                        }

                        if (!Mathf.Approximately(takenStage.y, 0))
                        {
                            controller.Velocity.y = takenStage.y;
                        }

                        JumpCount++;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Sets JumpCount to 0
        /// </summary>
        public void ResetCount ()
        {
            JumpCount = 0;
        }

    }
}