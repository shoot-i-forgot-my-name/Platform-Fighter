using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

sealed class CollisionType {

    #pragma warning disable CS0628 // New protected member declared in sealed class
    protected bool[] id; // Is faster than strings
    protected string idName = "none"; // Convert id to string
    #pragma warning restore CS0628 // New protected member declared in sealed class

    public CollisionType (bool bit0, bool bit1, bool bit2, bool bit3) {
        id = new bool[4] { bit0, bit1, bit2, bit3 };

        if (bit0 || bit1 || bit2 || bit3) {
            idName = "";
        }

        if (bit0) {
            idName += "below ";
        }

        if (bit1) {
            idName += "above ";
        }

        if (bit2) {
            idName += "left ";
        }

        if (bit3) {
            idName += "right ";
        }

        // Remove the space at the end
        idName.Remove(idName.Length - 1);
    }

    #region Public Methods

    public override int GetHashCode () {
        int rightAdd = (id[0] ? 1 : 0) + (id[1] ? 1 : 0) + (id[2] ? 1 : 0) + (id[3] ? 1 : 0);
        return (int) (idName.Length * 3.14) ^ id.Length / 2 + rightAdd;
    }

    public override bool Equals (object obj) {
        if (ReferenceEquals(this, obj)) {
            return true;
        }

        if (ReferenceEquals(obj, null)) {
            return false;
        }

        return Equals((CollisionType) obj);
    }

    /// <summary>
    /// Returns a nicely formatted string form of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString () {
        return idName;
    }

    #endregion

    #region Operators

    /// <summary>
    /// Replaces false bits with true bits only when available
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static CollisionType operator + (CollisionType left, CollisionType right) {
        return new CollisionType(left.id[0] || right.id[0],
                                 left.id[1] || right.id[1],
                                 left.id[2] || right.id[2],
                                 left.id[3] || right.id[3]);
    }

    /// <summary>
    /// Replaces true bits with false bits only when available
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static CollisionType operator - (CollisionType left, CollisionType right) {
        return new CollisionType(!(!left.id[0] || !right.id[0]), !(!left.id[1] || !right.id[1]), !(!left.id[2] || !right.id[2]), !(!left.id[3] || !right.id[3]));
    }

    /// <summary>
    /// Returns true if the left contains a part of right
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator == (CollisionType left, CollisionType right) {
        return ((left.id[0] || right.id[0]) && left.id[0] == right.id[0]) ||
               ((left.id[1] || right.id[1]) && left.id[1] == right.id[1]) ||
               ((left.id[2] || right.id[2]) && left.id[2] == right.id[2]) ||
               ((left.id[3] || right.id[3]) && left.id[3] == right.id[3]);
    }
    // false, false, false, true
    // true, false, false, false

    /// <summary>
    /// Returns true if the left dosen't contain a part of right
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator != (CollisionType left, CollisionType right) {
        return !(left.id[0]
                 || right.id[0]
                 && left.id[0] == right.id[0]
                 || left.id[1]
                 || right.id[1]
                 && left.id[1] == right.id[1]
                 || left.id[2]
                 || right.id[2]
                 && left.id[2] == right.id[2]
                 || left.id[3]
                 || right.id[3]
                 && left.id[3] == right.id[3]);
    }

    #endregion

    #region Static Variables

    // 0, 0, 0, 1
    public static readonly CollisionType Right = new CollisionType(false, false, false, true);
    // 0, 0, 1, 0
    public static readonly CollisionType Left = new CollisionType(false, false, true, false);
    // 0, 1, 0, 0
    public static readonly CollisionType Above = new CollisionType(false, true, false, false);
    // 1, 0, 0, 0
    public static readonly CollisionType Below = new CollisionType(true, false, false, false);
    // 0, 0, 0, 0
    public static readonly CollisionType None = new CollisionType(false, false, false, false);

    #endregion
}
