using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceManager", menuName = "Scriptable Objects/Resources/ResourceManager")]
public class ResourceManagerSO : ScriptableObject
{
    [Header("Все Ресурсы:")]

    public List<ResourceCategorySO> CategoriesList;
}
