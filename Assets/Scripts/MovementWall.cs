using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CollisionFlagsHandler))]
public class MovementWall : MonoBehaviour{

    #region Variables

    private Rigidbody2D rb; // Reference to Rigidbody2D
    private CollisionFlagsHandler cf; // Reference to CollisionFlagsHandler

    public InputManager im; // Reference to InputManager

    #endregion

    private void Start () {
        rb = GetComponent<Rigidbody2D>();
        cf = GetComponent<CollisionFlagsHandler>();
    }

    private void Update () {

        if (cf.collisionFlags == CollisionFlags.Sides) {

        }
    }

}