using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceCategory", menuName = "Scriptable Objects/Resources/ResourceCategory")]
public class ResourceCategorySO : ScriptableObject
{
    public string CategoryName;
    public List<ResourceTypeSO> TypeList;
}
