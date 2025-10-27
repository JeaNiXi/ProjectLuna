using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resources/Resource")]
public class ResourceSO : ScriptableObject
{
    public string ID; // Is converted to FixedString128Bytes in Baker!!!

    [Header("Основные данные:")]

    public string ResourceNameKey;
    public string Description;

    public Sprite Icon;

    public ResourceCategories.ResourceCategoriesList ResourceCategory;

    public ResourceTypes.ResourceTypesList ResourceType;

    public int resourceLevel;

    public float BaseGatheringTime;
    public float BaseGatheringAmount;

    public float GatherAmountMultiplayerPerUpgrade;
}
