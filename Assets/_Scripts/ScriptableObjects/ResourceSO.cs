using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resources/Resource")]
public class ResourceSO : ScriptableObject
{
    public string ID;

    [Header("Основные данные:")]

    public string ResourceNameKey;
    public ResourceCategories.ResourceCategoriesList ResourceCategory;
    public ResourceTypes.ResourceTypesList ResourceType;
    public float BaseGatheringTime;
    public float BaseGatheringAmount;
}
