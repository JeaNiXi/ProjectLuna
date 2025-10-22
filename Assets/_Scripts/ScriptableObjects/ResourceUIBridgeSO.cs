using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceUIBridge", menuName = "Scriptable Objects/Resources/ResourceUIBridge")]
public class ResourceUIBridgeSO : ScriptableObject
{
    public Dictionary<string, ResourceDynamicDataStruct> DynamicDataStruct = new Dictionary<string, ResourceDynamicDataStruct>();
}

[Serializable]
public struct ResourceDynamicDataStruct
{
    public float GatheringSpeed;
}
