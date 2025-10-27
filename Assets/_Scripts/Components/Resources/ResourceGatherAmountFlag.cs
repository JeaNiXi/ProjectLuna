using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceGatherAmountFlag : IComponentData
{
    public FixedString128Bytes ID;
    public int ResourceLevel;
    public float CurrentGatheringAmount;
    public float GatheringAmountMultiplayer;
}
