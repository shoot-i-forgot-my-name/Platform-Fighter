using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Character), typeof(InputManager))]
public class WallJump : MonoBehaviour
{
    #region Variables

    private Character _character;
    private InputManager _inputManager;
    private WallCling _wallCling;
    private Walk _walk;

    private IEnumerator _crEnableComponents;
    private float _jumpPercentage = 100;

#pragma warning disable IDE1006 // Naming Styles

    /// <summary>
    /// Force on jumped
    /// </summary>
    public Vector2 Force = Vector2.zero;

    /// <summary>
    /// Percentage subtracted from 100
    /// </summary>
    public float PercentageSubtracted = .15f;

    /// <summary>
    /// Seconds after wall jumping of helplessness
    /// </summary>
    public float WalkTimeout = .25f;

#pragma warning restore IDE1006 // Naming Styles

    #endregion Variables

    private void Start()
    {
        // Get references
        _character = GetComponent<Character>();
        _inputManager = GetComponent<InputManager>();
        _wallCling = GetComponent<WallCling>();
        _walk = GetComponent<Walk>();

        _crEnableComponents = EditComponents();
    }

    private void Update()
    {
        var exactlySides = _character.CollisionInfo.ExactlyEqual(CollisionInfo.Left) || _character.CollisionInfo.ExactlyEqual(CollisionInfo.Right);

        if (exactlySides && _inputManager.PressingJump)
        {
            _jumpPercentage -= PercentageSubtracted;

            // Prevent negative percent
            if (_jumpPercentage >= 0)
            {
                var forceX = Force.x * -_inputManager.ModifiedAxis.x * _jumpPercentage;
                var forceY = Force.y * _jumpPercentage;
                _character.SetVelocity(forceX, forceY);
            }
        }

        if (_character.CollisionInfo == CollisionInfo.Below)
        {
            // Reset the "jumpCounter"
            _jumpPercentage = 1;
        }

        // Check if both of the to-disable components even exist and..
        // check if the player is colliding exclusively to the right/left
        if ((_walk != null || _wallCling != null) && exactlySides)
        {
            if (_inputManager.PressingJump)
            {
                StartCoroutine(_crEnableComponents);
            }
        }
    }

    private IEnumerator EditComponents()
    {
        #region Component Disabling

        if (_wallCling)
        {
            _wallCling.enabled = false;
        }

        if (_walk)
        {
            _walk.enabled = false;
        }

        #endregion Component Disabling

        var timeLeft = WalkTimeout;

        while (timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        #region Component Enabling

        if (_wallCling)
        {
            _wallCling.enabled = true;
        }

        if (_walk)
        {
            _walk.enabled = true;
        }

        #endregion Component Enabling

        // Reset _crEnableWalk
        _crEnableComponents = EditComponents();
    }
}
