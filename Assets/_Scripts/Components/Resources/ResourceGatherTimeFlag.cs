using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceGatherTimeFlag : IComponentData
{
    public FixedString128Bytes ID;
    public float GatheringTimeChangeValue;
    public FixedString128Bytes ModifierID;
}
