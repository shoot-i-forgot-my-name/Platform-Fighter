using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class AnimationHandler : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;

    private Character character;
    private InputManager im;

    private WallCling wallCling;
    private Walk walk;

    private void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        character = GetComponent<Character>();
        im = GetComponent<InputManager>();

        wallCling = GetComponent<WallCling>();
        walk = GetComponent<Walk>();
    }

    private void Update ()
    {
        #region Look at direction

        var val = transform.localScale;

        if ((im.rawAxis.x < 0 && val.x < 0) || (im.rawAxis.x > 0 && val.x > 0))
        {
            val.x = -val.x;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, val, 500 * Time.deltaTime);

        #endregion

        if (walk)
        {
            var percentage = walk.curSpeed / walk.runSpeed;
            anim.SetFloat("MovePercent", percentage);
        }

        if (wallCling)
        {
            // Colliding with a wall on your right in the air and moving to the right
            var exclusiveRight = character.CollisionInfo.ExactlyEqual(CollisionInfo.Right) && im.rawAxis.x == 1;
            // Colliding with a wall on your left in the air and moving to the left
            var exclusiveLeft = character.CollisionInfo.ExactlyEqual(CollisionInfo.Left) && im.rawAxis.x == -1;

            anim.SetBool("Clinging", exclusiveLeft || exclusiveRight );
        }

    }
}
