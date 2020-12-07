using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public enum WallType
    {
        down, left, right, up
    }

    public WallType wallType;
}
