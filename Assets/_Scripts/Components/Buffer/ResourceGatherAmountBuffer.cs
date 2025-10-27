using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceGatherAmountBuffer : IBufferElementData
{
    public FixedString128Bytes ID;
    public int NewResourceLevel;
    public float NewGatheringAmount;
}
