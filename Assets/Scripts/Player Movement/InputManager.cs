using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Variables

    [HideInInspector]
    public Vector2 RawAxis { get; private set; } = Vector2.zero;

    [HideInInspector]
    public Vector2 ModifiedAxis { get; private set; } = Vector2.zero;

    [HideInInspector]
    public bool PressingJump { get; private set; } = false;

    #endregion Variables

    public void Update()
    {
        RawAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        {
            var x = RawAxis.x == 0 ? ModifiedAxis.x : RawAxis.x;
            var y = RawAxis.y == 0 ? ModifiedAxis.y : RawAxis.y;
            ModifiedAxis = new Vector2(x, y);
        }

        PressingJump = Input.GetButton("Jump");
    }
}
