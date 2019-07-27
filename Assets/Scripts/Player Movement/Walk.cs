using UnityEngine;

[RequireComponent(typeof(Character))]
public class Walk : MonoBehaviour
{
    #region Variables

    private Character _character;
    private InputManager _inputManager;

    private float _curSpeed = 0; // Current speed of the player

#pragma warning disable IDE1006 // Naming Styles
    public float MoveSpeed = 10;

    /// <summary>
    /// Time for the character to stop moving
    /// </summary>
    public float SmoothStop = 10;

    /// <summary>
    /// Time for the character to start moving
    /// </summary>
    public float SmoothMove = 7.5f;

    /// <summary>
    /// Percentage of the movement when on air
    /// </summary>
    public float AirMovement = .75f;

#pragma warning restore IDE1006 // Naming Styles

    #endregion Variables

    private void Start()
    {
        _character = GetComponent<Character>();
        _inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        var smoothTime = (_inputManager.RawAxis.x == 0) ? SmoothStop : SmoothMove; // Either smoothStop or smoothMove
        var targetSpeed = (_inputManager.RawAxis.x == 0) ? 0 : MoveSpeed; // Either 0 or the default speed
        smoothTime *= _character.CollisionInfo.ExactlyEqual(CollisionInfo.None) ? AirMovement : 1;
        _curSpeed = Mathf.Lerp(_curSpeed, targetSpeed, smoothTime * Time.deltaTime); // Lerp for smooth movement

        var exactlyRight = _character.CollisionInfo == CollisionInfo.Right && _inputManager.RawAxis.x == 1;
        var exactlyLeft = _character.CollisionInfo == CollisionInfo.Left && _inputManager.RawAxis.x == -1;

        if (exactlyRight || exactlyLeft)
        {
            _curSpeed = 0;
        }

        if (_curSpeed != 0)
            _character.SetVelocityX(_inputManager.RawAxis.x * _curSpeed);
    }
}