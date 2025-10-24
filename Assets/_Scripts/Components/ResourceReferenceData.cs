using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceReferenceData : IComponentData
{
    public FixedString128Bytes IdInBlob;
    public int Index;
}
