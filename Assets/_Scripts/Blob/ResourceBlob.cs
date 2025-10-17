using Unity.Entities;
using UnityEngine;

public struct ResourceBlob
{
    public BlobArray<int> IDHashes;
    public BlobArray<float> BaseGatheringTimes;
}
