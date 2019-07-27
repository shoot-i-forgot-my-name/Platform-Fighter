using UnityEngine;

[RequireComponent(typeof(Character), typeof(InputManager))]
public class Jump : MonoBehaviour
{
    #region Variables

    private Character _character;
    private InputManager _inputManager;

#pragma warning disable IDE1006 // Naming Styles
    public float ForceY = 15;
#pragma warning restore IDE1006 // Naming Styles

    #endregion Variables

    private void Start()
    {
        _character = GetComponent<Character>();
        _inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (_character.CollisionInfo == CollisionInfo.Below && _inputManager.PressingJump)
        {
            _character.SetVelocityY(ForceY);
        }
    }
}
