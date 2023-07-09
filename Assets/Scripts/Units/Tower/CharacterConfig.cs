using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Character Config")]
public class CharacterConfig : ScriptableObject
{

    //Main Body
    [Header("Main Body")]
    [SerializeField] public RuntimeAnimatorController myAnimController;
    [SerializeField] public GameObject characterBody;
    [SerializeField] public string mainColorHex;
    [SerializeField] public string subColorHex;
    [SerializeField] public string hairID;
    [SerializeField] public string mouthID;
    [SerializeField] public string eyeID;




}
