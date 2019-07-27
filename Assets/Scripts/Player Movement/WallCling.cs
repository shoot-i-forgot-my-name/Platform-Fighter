using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Character), typeof(InputManager))]
public class WallCling : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Reference to the Character
    /// </summary>
    private Character _character;

    /// <summary>
    /// Reference to the InputManager
    /// </summary>
    private InputManager _inputManager;

    private IEnumerator _crSetVelocity;

#pragma warning disable IDE1006 // Naming Styles

    [Tooltip("A number to set velocity-y")]
    public float YVelocitySet = -.5f;

    [Tooltip("Sets the velocity-y to a number for {SetterTimer} seconds")]
    public float SetterTimer = 2;

#pragma warning restore IDE1006 // Naming Styles

    #endregion Variables

    private void Start()
    {
        // Get references
        _character = GetComponent<Character>();
        _inputManager = GetComponent<InputManager>();
        _crSetVelocity = SetVelocity();
    }

    private void Update()
    {
        // Colliding with a wall on your right while on air and [attempting] to move to your right
        var wallRight = _character.CollisionInfo.ExactlyEqual(CollisionInfo.Right) && _inputManager.RawAxis.x == 1;
        // Colliding with a wall on your left while on air and [attempting] to move to your left
        var wallLeft = _character.CollisionInfo.ExactlyEqual(CollisionInfo.Left) && _inputManager.RawAxis.x == -1;

        // If either conditions above are true..
        if (wallRight || wallLeft)
        {
            // Start the coroutine
            StartCoroutine(_crSetVelocity);
        }
        else
        {
            // Stop the coroutine
            StopCoroutine(_crSetVelocity);
            // Reset _crSetVelocity
            _crSetVelocity = SetVelocity();
        }
    }

    private IEnumerator SetVelocity()
    {
        var timeLeft = SetterTimer;

        while (timeLeft >= 0)
        {
            _character.SetVelocityY(YVelocitySet);
            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }
}
