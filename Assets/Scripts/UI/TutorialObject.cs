using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial Information")]
public class TutorialObject : ScriptableObject
{


    [Header("ID")]
    public string TUTORIAL_ID;
    public Sprite portrait;
    [TextArea(5, 10)] public string[] kotorilines;
    public Vector3[] focusPosition;
    public Vector2[] focusSize;
}
