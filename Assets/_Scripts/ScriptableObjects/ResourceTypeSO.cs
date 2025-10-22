using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceList", menuName = "Scriptable Objects/Resources/ResourceList")]
public class ResourceTypeSO : ScriptableObject
{
    public string ResourceTypeName;
    public List<ResourceSO> ResourceList;
}
