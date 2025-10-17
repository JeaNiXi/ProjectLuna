using Unity.Collections;
using Unity.Entities;
using UnityEngine;

class ResourceManagerAuthoring : MonoBehaviour
{
    public ResourceManagerSO ResourceManagerSOAuthoring;
}

class ResourceManagerBaker : Baker<ResourceManagerAuthoring>
{
    public override void Bake(ResourceManagerAuthoring authoring)
    {
        var builder = new BlobBuilder(Allocator.Temp);
        ref ResourceBlob resourceBlob = ref builder.ConstructRoot<ResourceBlob>();

        int count = authoring.ResourceManagerSOAuthoring.Resources.Count;

        BlobBuilderArray<int> IDHashBuilder = builder.Allocate(ref resourceBlob.IDHashes, count);
        BlobBuilderArray<float> GatheringTimesBuilder = builder.Allocate(ref resourceBlob.BaseGatheringTimes, count);

        for (int i = 0; i < count; i++)
        {
            IDHashBuilder[i] = i;
            GatheringTimesBuilder[i] = authoring.ResourceManagerSOAuthoring.Resources[i].BaseGatheringTime; // Добавить проверки типа: idArray[i] = (r != null) ? r.ID.GetHashCode() : 0;
        }

        var result = builder.CreateBlobAssetReference<ResourceBlob>(Allocator.Persistent);
        builder.Dispose();

        AddBlobAsset<ResourceBlob>(ref result, out var hash);
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new ResourceManagerComponent()
        {
            ResourceBlobRef = result,
        });
    }
}
