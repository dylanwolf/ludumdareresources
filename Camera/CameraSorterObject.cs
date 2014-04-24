using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
// Defines an object whose Z-index will be adjusted based on it's bounding box's Y coordinate
// You can use this component to change which "layer" the object shows up on
public class CameraSorterObject : MonoBehaviour
{
    public int Layer = 0;

    [NonSerialized]
    public float YSort = 0;
}
