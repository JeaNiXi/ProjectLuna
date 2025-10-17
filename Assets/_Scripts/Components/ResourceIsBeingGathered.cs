using Unity.Entities;
using UnityEngine;

public struct ResourceIsBeingGathered : IComponentData
{
    public float GatheringProgress;
    public Entity GathererEntity;
}
