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
    public float Lifetime;

    [Tooltip("Size of the particle.")]
    public float Size;
}
