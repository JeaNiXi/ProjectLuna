using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceRealtimeLevelBuffer : IBufferElementData
{
    public FixedString128Bytes ID;
    public int Level;
}