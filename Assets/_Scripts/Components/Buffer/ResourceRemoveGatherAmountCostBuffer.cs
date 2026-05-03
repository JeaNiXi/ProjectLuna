using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceRemoveGatherAmountCostBuffer : IBufferElementData
{
    public FixedString128Bytes ID;
    public float ResourceUpgradeCost;
}
