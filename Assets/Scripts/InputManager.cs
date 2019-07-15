using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    
    [HideInInspector]
    public Vector2 rawInput { get; private set; } // Raw input
    [HideInInspector]
    public Vector2 modifiedInput { get; private set; } // Input that only accepts 1 or -1

    void Update() {
        rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        modifiedInput = new Vector2(rawInput.x == 0 ? modifiedInput.x : rawInput.x, rawInput.y == 0 ? modifiedInput.y : rawInput.y);
    }
}
