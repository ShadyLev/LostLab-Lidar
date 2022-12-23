using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointType
{
    public string TagName;
    [ColorUsage(true, true)]
    public Color Color;
    public float Lifetime;
}
