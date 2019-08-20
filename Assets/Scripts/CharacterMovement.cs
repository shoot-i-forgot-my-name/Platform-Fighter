using System.Collections;
using System.Collections.Generic;
using Serializable = System.SerializableAttribute;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class CharacterMovement : MonoBehaviour
{
    #region Variables

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
        float input = Input.GetAxisRaw("Horizontal");
        bool running = Input.GetKey(KeyCode.LeftShift);
        bool jumping = Input.GetButton("Jump");

        Move(input, running);

        if (controller.grounded)
        {
            controller.velocity.y = 0;

            wJumping.ResetCount();
            Jump(jumping);
        }
        else
        {
            controller.velocity.y += Physics2D.gravity.y * Time.deltaTime;

            wJumping.JumpOff(controller, jumping);
        }

        controller.Move();
    }

    private void Jump(bool jumping)
    {
        if (jumping)
        {
            controller.velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        }
    }

    private void Move(float input, bool running)
    {
        float acceleration = controller.grounded ? groundAcceleration : airAcceleration;
        float deceleration = controller.grounded ? groundDeceleration : 0;

        if (input == 0)
        {
            controller.velocity.x = Mathf.MoveTowards(controller.velocity.x, 0, deceleration * Time.deltaTime);
        }
        else
        {
            controller.velocity.x = Mathf.MoveTowards(controller.velocity.x, (running ? runSpeed : walkSpeed) * input, acceleration * Time.deltaTime);
        }
    }

    [Serializable]
    private class WallJumping
    {
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

            Collider2D[] hits = Physics2D.OverlapBoxAll(controller.transform.position, controller.collider.size, 0);

            foreach (Collider2D hit in hits)
            {
                if (hit == controller.collider || JumpCount >= _stages.Length)
                {
                    continue;
                }

                ColliderDistance2D seperation = hit.Distance(controller.collider);

                if (seperation.isOverlapped)
                {
                    float surfaceAngle = Vector2.Angle(seperation.normal, Vector2.up);

                    if (surfaceAngle >= 90 && surfaceAngle <= 92.5)
                    {
                        Vector2 takenStage = _stages[JumpCount];

                        if (!Mathf.Approximately(takenStage.x, 0))
                        {
                            controller.velocity.x = takenStage.x * seperation.normal.x;
                        }

                        if (!Mathf.Approximately(takenStage.y, 0))
                        {
                            controller.velocity.y = takenStage.y;
                        }

                        JumpCount++;
                        return;
                    }
                }
            }
        }

        public void ResetCount ()
        {
            JumpCount = 0;
        }

    }
}