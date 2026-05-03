using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceIsGatheringFlag : IComponentData
{
    public FixedString128Bytes ID;
    public int Level;
    public float BaseAmount;
    public float ProductionMultiplayer;
}
