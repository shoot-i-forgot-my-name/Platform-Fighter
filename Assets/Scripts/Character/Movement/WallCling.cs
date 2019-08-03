using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class WallCling : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Reference to the Character
    /// </summary>
    private Character character;

    /// <summary>
    /// Reference to the InputManager
    /// </summary>
    private InputManager inputManager;

    private IEnumerator crSetVelocity { get; set; }

#pragma warning disable IDE1006 // Naming Styles

    [Tooltip("A number to set velocity-y while clinging")]
    public float yVelocitySetter = -.5f;

    [Tooltip("Seconds of not sliding off the wall")]
    public float holdingTime = 2;

#pragma warning restore IDE1006 // Naming Styles

    #endregion Variables

    private void Start ()
    {
        // Get references
        character = GetComponent<Character>();
        inputManager = GetComponent<InputManager>();
        crSetVelocity = SetVelocity();
    }

    private void Update ()
    {
        // Colliding with a wall on your right in the air and moving to the right
        var exclusiveRight = character.CollisionInfo.ExactlyEqual(CollisionInfo.Right) && inputManager.rawAxis.x == 1;
        // Colliding with a wall on your left in the air and moving to the left
        var exclusiveLeft = character.CollisionInfo.ExactlyEqual(CollisionInfo.Left) && inputManager.rawAxis.x == -1;

        // If either conditions above are true..
        if (exclusiveRight || exclusiveLeft)
        {
            // Start the coroutine
            StartCoroutine(crSetVelocity);
        }
        else
        {
            // Stop the coroutine
            StopCoroutine(crSetVelocity);
            // Reset crSetVelocity
            crSetVelocity = SetVelocity();
        }
    }

    private IEnumerator SetVelocity ()
    {
        var timeLeft = holdingTime;

        while (timeLeft >= 0)
        {
            character.SetVelocityY(yVelocitySetter);

            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }

    private void OnDisable ()
    {
        StopAllCoroutines();
    }
}