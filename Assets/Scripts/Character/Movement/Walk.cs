using UnityEngine;

[RequireComponent(typeof(Character))]
public class Walk : MonoBehaviour
{
    #region Variables

    public Character character;
    private InputManager im;

    public float curSpeed { get; private set; } = 0; // Current speed of the player

#pragma warning disable IDE1006 // Naming Styles

    public float runSpeed = 17.5f;
    public float walkSpeed = 10;

    /// <summary>
    /// Time for the character to stop moving
    /// </summary>
    public float smoothStop = 10;

    /// <summary>
    /// Time for the character to start moving
    /// </summary>
    public float smoothMove = 7.5f;

    /// <summary>
    /// Percentage of the movement when on air
    /// </summary>
    public float airMovement = .75f;

#pragma warning restore IDE1006 // Naming Styles

    #endregion Variables

    private void Start ()
    {
        character = GetComponent<Character>();
        im = GetComponent<InputManager>();
    }

    private void Update ()
    {
        var smoothTime = (im.rawAxis.x == 0) ? smoothStop : smoothMove; // Either smoothStop or smoothMove
        var targetSpeed = (im.rawAxis.x == 0) ? 0 : walkSpeed; // Either 0 or the default speed

        targetSpeed = (im.rawAxis.x != 0 && im.running) ? runSpeed : targetSpeed;

        smoothTime *= character.CollisionInfo.ExactlyEqual(CollisionInfo.None) ? airMovement : 1;
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed, smoothTime * Time.deltaTime); // Lerp for smooth movement

        var exactlyRight = character.CollisionInfo == CollisionInfo.Right && im.rawAxis.x == 1;
        var exactlyLeft = character.CollisionInfo == CollisionInfo.Left && im.rawAxis.x == -1;

        if (exactlyRight || exactlyLeft)
        {
            curSpeed = 0;
        }

        if (curSpeed != 0)
        {
            character.SetVelocityX(im.rawAxis.x * curSpeed);
        }
    }
}