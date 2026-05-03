using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceIsGatheringBuffer : IBufferElementData
{
    public FixedString128Bytes ID;
    public int CurrentLevel;
    public float AmountToAdd;
}
