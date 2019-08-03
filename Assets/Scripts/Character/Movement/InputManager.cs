using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// No modification of WASD input
    /// </summary>
    public Vector2 rawAxis { get; private set; } = Vector2.zero;

    /// <summary>
    /// WASD input but only accepts non-zero values
    /// </summary>
    public Vector2 modifiedAxis { get; private set; } = Vector2.zero;

    /// <summary>
    /// If the user is pressing the jump button
    /// </summary>
    public bool jumping { get; private set; } = false;

    /// <summary>
    /// If the user is pressing the run button
    /// </summary>
    public bool running { get; private set; } = false;

    [Header("WASD movement")]
    public KeyCode upButton = KeyCode.W;
    public KeyCode downButton = KeyCode.S;
    public KeyCode rightButton = KeyCode.D;
    public KeyCode leftButton = KeyCode.A;
    [Header("Other buttons")]
    public KeyCode runButton = KeyCode.LeftShift;
    public KeyCode jumpButton = KeyCode.Space;


    #endregion Variables

    public void Update ()
    {
        Vector2 vectorTemp = Vector2.zero;
        vectorTemp.x = Input.GetKey(rightButton) ? 1 :
                    (Input.GetKey(leftButton) ? -1 : 0);
        vectorTemp.y = Input.GetKey(upButton) ? 1 :
                    (Input.GetKey(downButton) ? -1 : 0);
        rawAxis = vectorTemp;

        {
            var x = rawAxis.x == 0 ? modifiedAxis.x : rawAxis.x;
            var y = rawAxis.y == 0 ? modifiedAxis.y : rawAxis.y;
            modifiedAxis = new Vector2(x, y);
        }

        jumping = Input.GetKey(jumpButton);
        running = Input.GetKey(runButton);
    }
}