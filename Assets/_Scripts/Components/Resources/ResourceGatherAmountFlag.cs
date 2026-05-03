using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceGatherAmountFlag : IComponentData
{
    public FixedString128Bytes ID;
    public int ResourceLevel;
    public float BaseGatheringAmount;
    public float GatheringAmountMultiplayer;
    public float BaseUpgradeCost;
    public float UpgradeCostMultiplayer;
}
