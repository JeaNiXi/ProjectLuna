using Resources;
using UnityEngine;


[CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resources/Resource")]
public class ResourceSO : ScriptableObject
{
    public ResourceTypes.ResourceType resourceType;
}
