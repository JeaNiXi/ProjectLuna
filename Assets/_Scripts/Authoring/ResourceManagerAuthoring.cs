using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
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

        // Resource <= ResourceType <= ResourceCategory <= ResourceManager
        // OakTree <= Wood <= Natural <= AllResourceManager

        int allResourcesCount = authoring.ResourceManagerSOAuthoring
            .CategoriesList
            .SelectMany(c => c.TypeList)
            .SelectMany(t => t.ResourceList)
            .Count();

        BlobBuilderArray<FixedString128Bytes> ID = builder.Allocate(ref resourceBlob.ID, allResourcesCount);
        BlobBuilderArray<byte> ResourceCategory = builder.Allocate(ref resourceBlob.ResourceCategory, allResourcesCount);
        BlobBuilderArray<byte> ResourceType = builder.Allocate(ref resourceBlob.ResourceType, allResourcesCount);
        BlobBuilderArray<float> BaseGatheringTime = builder.Allocate(ref resourceBlob.BaseGatheringTime, allResourcesCount);
        BlobBuilderArray<float> BaseGatheringAmount = builder.Allocate(ref resourceBlob.BaseGatheringAmount, allResourcesCount);

        int index = 0;
        for (int i = 0; i < authoring.ResourceManagerSOAuthoring.CategoriesList.Count; i++)
        {
            for (int j = 0; j < authoring.ResourceManagerSOAuthoring.CategoriesList[i].TypeList.Count; j++)
            {
                for (int k = 0; k < authoring.ResourceManagerSOAuthoring.CategoriesList[i].TypeList[j].ResourceList.Count; k++)
                {
                    ResourceSO res = authoring.ResourceManagerSOAuthoring.CategoriesList[i].TypeList[j].ResourceList[k];
                    ID[index] = new FixedString128Bytes(res.ID);
                    ResourceCategory[index] = (byte)res.ResourceCategory;
                    ResourceType[index] = (byte)res.ResourceType;
                    BaseGatheringTime[index] = res.BaseGatheringTime;
                    BaseGatheringAmount[index] = res.BaseGatheringAmount;
                    index++;
                }
            }
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
