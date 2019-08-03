using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class WallJump : MonoBehaviour
{
    #region Variables

    public Character character { get; set; }
    private InputManager inputManager;
    private WallCling wallCling;
    private Walk walk;

    private IEnumerator crVulnerableTimeout;
    private float jumpPercentage = 1;

    [Tooltip("Force when jumping off the wall")]
    public Vector2 force = Vector2.one * 20;

    [Tooltip("Percentage to subtract the total jump percent"), Range(0, 1)]
    public float percentageSubtraction = 1;

    [Tooltip("Seconds of vulnerability after jumping off the wall")]
    public float jumpTimeout = .25f;

    #endregion Variables

    private void Start ()
    {
        // Get references
        character = GetComponent<Character>();
        inputManager = GetComponent<InputManager>();
        wallCling = GetComponent<WallCling>();
        walk = GetComponent<Walk>();

        crVulnerableTimeout = VulnerableTimeout();
    }

    private void Update ()
    {
        var colldingLeft = character.CollisionInfo.ExactlyEqual(CollisionInfo.Left);
        var collidingRight = character.CollisionInfo.ExactlyEqual(CollisionInfo.Right);

        if ((colldingLeft || collidingRight) && inputManager.jumping)
        {
            // Prevent negative percent
            if (jumpPercentage >= 0)
            {
                var forceX = force.x * -inputManager.modifiedAxis.x * jumpPercentage;
                var forceY = force.y * jumpPercentage;
                character.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
            }

            jumpPercentage -= percentageSubtraction;
        }

        // Reset the jumpPercentage when colliding with the ground
        if (character.CollisionInfo == CollisionInfo.Below)
        {
            jumpPercentage = 1;
        }

        // Check if both of the to-disable components even exist and..
        // check if the player is colliding exclusively to the right/left
        if ((walk != null || wallCling != null) && (colldingLeft || collidingRight))
        {
            if (inputManager.jumping)
            {
                StartCoroutine(crVulnerableTimeout);
            }
        }
    }

    private IEnumerator VulnerableTimeout ()
    {
        if (wallCling)
            wallCling.enabled = false;

        if (walk)
            walk.enabled = false;

        var timeLeft = jumpTimeout;

        while (timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        if (wallCling)
            wallCling.enabled = true;

        if (walk)
            walk.enabled = true;

        // Reset crEnableComponents
        crVulnerableTimeout = VulnerableTimeout();
    }

    private void OnDisable ()
    {
        StopAllCoroutines();
    }
}