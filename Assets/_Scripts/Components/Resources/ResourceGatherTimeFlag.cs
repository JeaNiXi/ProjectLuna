using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceGatherTimeFlag : IComponentData
{
    public FixedString128Bytes ID;
    public float CurrentGatheringTime;
    public float GatheringTimeMultiplayer;
}
