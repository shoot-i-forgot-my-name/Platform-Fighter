public sealed class CollisionInfo
{
#pragma warning disable CS0628 // New protected member declared in sealed class

    /// <summary>
    /// ID of the collision
    /// </summary>
    protected bool[] id;
    /// <summary>
    /// Name of the collision based on the ID
    /// </summary>
    protected string idName = "none"; // Convert bool array to a string

#pragma warning restore CS0628 // New protected member declared in sealed class

    private CollisionInfo (bool bit0, bool bit1, bool bit2, bool bit3)
    {
        id = new bool[4] { bit0, bit1, bit2, bit3 };

        // Assign name based on ID
        if (bit0 || bit1 || bit2 || bit3)
        {
            idName = "";
        }

        if (bit0)
        {
            idName += "below ";
        }

        if (bit1)
        {
            idName += "above ";
        }

        if (bit2)
        {
            idName += "left ";
        }

        if (bit3)
        {
            idName += "right ";
        }

        // Remove the space at the end
        idName.Remove(idName.Length - 1);
    }

    #region Public Methods

    /// <summary>
    /// If it is exactly equal to the other object
    /// </summary>
    /// <param name="right"></param>
    /// <returns></returns>
    public bool ExactlyEqual (CollisionInfo right)
    {
        return id[0] == right.id[0] &&
               id[1] == right.id[1] &&
               id[2] == right.id[2] &&
               id[3] == right.id[3];
    }

    /// <summary>
    /// Returns true if the left contains a part of right
    /// </summary>
    /// <param name="right"></param>
    /// <returns></returns>
    public bool Contains (CollisionInfo right)
    {
        // Don't remove the parenthises, it's the only thing keeping the code from going bongers
        return ((id[0] || right.id[0]) && id[0] == right.id[0]) ||
               ((id[1] || right.id[1]) && id[1] == right.id[1]) ||
               ((id[2] || right.id[2]) && id[2] == right.id[2]) ||
               ((id[3] || right.id[3]) && id[3] == right.id[3]);
    }

    public override int GetHashCode ()
    {
        int rightAdd = (id[0] ? 1 : 0) + (id[1] ? 1 : 0) + (id[2] ? 1 : 0) + (id[3] ? 1 : 0);
        return (int)(idName.Length * 3.14) ^ id.Length / 2 + rightAdd;
    }

    public override bool Equals (object obj)
    {
        if (ReferenceEquals(this, obj) || GetType() == obj.GetType())
        {
            return true;
        }

        if (ReferenceEquals(obj, null))
        {
            return false;
        }

        CollisionInfo colInfo = (CollisionInfo)obj;
        return this.Contains(colInfo);
    }

    /// <summary>
    /// Returns a nicely formatted string form of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString ()
    {
        return idName;
    }

    #endregion Public Methods

    #region Operators

    /// <summary>
    /// Replaces false bits with true bits only when available
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static CollisionInfo operator + (CollisionInfo left, CollisionInfo right)
    {
        return new CollisionInfo(left.id[0] || right.id[0],
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
    public static CollisionInfo operator - (CollisionInfo left, CollisionInfo right)
    {
        return new CollisionInfo(!(!left.id[0] || !right.id[0]), !(!left.id[1] || !right.id[1]), !(!left.id[2] || !right.id[2]), !(!left.id[3] || !right.id[3]));
    }

    /// <summary>
    /// Returns true if the left contains a part of right
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator == (CollisionInfo left, CollisionInfo right)
    {
        return left.Contains(right);
    }

    /// <summary>
    /// Returns true if the left dosen't contain a part of right
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator != (CollisionInfo left, CollisionInfo right)
    {
        // Don't remove the parenthises, it's the only thing keeping the code from going bongers
        return !left.Contains(right);
    }

    #endregion Operators

    #region Static Variables

    // 0, 0, 0, 1
    public static readonly CollisionInfo Right = new CollisionInfo(false, false, false, true);

    // 0, 0, 1, 0
    public static readonly CollisionInfo Left = new CollisionInfo(false, false, true, false);

    // 0, 1, 0, 0
    public static readonly CollisionInfo Above = new CollisionInfo(false, true, false, false);

    // 1, 0, 0, 0
    public static readonly CollisionInfo Below = new CollisionInfo(true, false, false, false);

    // 0, 0, 0, 0
    public static readonly CollisionInfo None = new CollisionInfo(false, false, false, false);

    #endregion Static Variables
}