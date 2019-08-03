using UnityEngine;

[RequireComponent(typeof(Character))]
public class Jump : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Reference to Character
    /// </summary>
    public Character character { get; set; }
    /// <summary>
    /// Reference to InputManager
    /// </summary>
    private InputManager inputManager;

#pragma warning disable IDE1006 // Naming Styles
    /// <summary>
    [Tooltip("Force in the y-axis when jump button is pressed")]
    public float forceY = 15;
#pragma warning restore IDE1006 // Naming Styles

    #endregion Variables

    private void Start ()
    {
        character = GetComponent<Character>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update ()
    {
        if (character.CollisionInfo == CollisionInfo.Below && inputManager.jumping)
        {
            character.SetVelocityY(forceY);
        }
    }
}