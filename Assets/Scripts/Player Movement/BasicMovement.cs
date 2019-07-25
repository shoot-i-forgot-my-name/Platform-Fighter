using UnityEngine;

[RequireComponent(typeof(Player))]
public class BasicMovement : MonoBehaviour {

    #region Variables

    private Player player { get; set; }

    private float curSpeed { get; set; } // Current speed of the player

    [Header("Basic Movement")]
    public float moveSpeed = 10; // Default movement speed

    public float smoothStop = 10; // Time for the character to stop moving
    public float smoothMove = 7.5f; // Time for the character to start moving
    public float airMovement = .75f; // Percentage of the movement when on air

    #endregion Variables

    private void Start () {
        player = GetComponent<Player>();
    }

    private void Update () {
        DoAction(player.rawInput);
    }

    private void DoAction (Vector2 inputRaw) {
        {
            var smoothTime = (inputRaw.x == 0) ? smoothStop : smoothMove; // Either smoothStop or smoothMove
            var targetSpeed = (inputRaw.x == 0) ? 0 : moveSpeed; // Either 0 or the default speed
            smoothTime *= player.collisionInfo.ExactlyEqual(CollisionInfo.None) ? airMovement : 1;
            curSpeed = Mathf.Lerp(curSpeed, targetSpeed, smoothTime * Time.deltaTime); // Lerp for smooth movement
        }

        player.SetVelocity(player.modifiedInput.x * curSpeed, null, 0);
    }
}