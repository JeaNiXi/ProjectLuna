using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceBlob
{
    public BlobArray<FixedString128Bytes> ID;
    public BlobArray<byte> ResourceType;
    public BlobArray<byte> ResourceCategory;
    public BlobArray<float> BaseGatheringTime;
    public BlobArray<float> BaseGatheringAmount;
}
