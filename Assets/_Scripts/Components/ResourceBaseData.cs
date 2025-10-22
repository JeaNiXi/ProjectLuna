using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceBaseData : IComponentData
{
    public FixedString128Bytes ID;
    public float BaseGatheringTime;
    public float BaseGatheringAmount;
    public byte ResourceCategory;
    public byte ResourceType;
}
