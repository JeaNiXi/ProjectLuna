using System.ComponentModel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceGatherTimeBuffer : IBufferElementData
{
    public FixedString128Bytes ID;
    public float NewGatheringTime;
}
