using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointType
{
    [Tooltip("Tag of an object you want the particle to change on hit.")]
    public string TagName;

    [Tooltip("Colour of the particle.")]
    [ColorUsage(true, true)]
    public Color Color;

    [Tooltip("Lifetime of the particle.")]
    public bool useDefaultGradient;

    [Tooltip("Size of the particle.")]
    public float Size;

    [Tooltip("Bool that shows if that tag is being scanned right now.")]
    public bool isBeingScanned;
}
