using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ConstructionSet", menuName = "Core/Construction", order = 1)]
public class ConstructionSet : ScriptableObject
{
    public List<GameObject> prefabs = new List<GameObject>();
}
