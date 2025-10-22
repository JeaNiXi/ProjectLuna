using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial struct ResourceInitialBuilder : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceManagerComponent>();
        EntityQuery query = state.GetEntityQuery(typeof(ResourceManagerComponent));
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
        foreach (Entity entity in entities)
        {
            ResourceManagerComponent resourceManagerComponent = state.EntityManager.GetComponentData<ResourceManagerComponent>(entity);
            ref ResourceBlob blob = ref resourceManagerComponent.ResourceBlobRef.Value;

            for (int i = 0; i < blob.ID.Length; i++)
            {
                var newEntity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(newEntity, new ResourceBaseData
                {
                    ID = blob.ID[i],
                    BaseGatheringTime = blob.BaseGatheringTime[i],
                    BaseGatheringAmount = blob.BaseGatheringAmount[i],
                    ResourceCategory = blob.ResourceCategory[i],
                    ResourceType = blob.ResourceType[i],
                });
            }
        }
        entities.Dispose();
    }
    public void OnUpdate(ref SystemState state)
    {

    }
    public void OnDestroy(ref SystemState state)
    {

    }
    //public struct ResourceBaseData : IComponentData
    //{
    //    public float BaseGatheringTime;
    //    public float BaseGatheringAmount;
    //}

    //public BlobArray<FixedString128Bytes> ID;
    //public BlobArray<byte> ResourceType;
    //public BlobArray<byte> ResourceCategory;
    //public BlobArray<float> BaseGatheringTime;
    //public BlobArray<float> BaseGatheringAmount;
    //public override void Bake(SpawnerAuthoring authoring)
    //{
    //    var entity = GetEntity(TransformUsageFlags.None);
    //    AddComponent(entity, new Spawner
    //    {
    //        // By default, each authoring GameObject turns into an Entity.
    //        // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
    //        Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
    //        SpawnPosition = authoring.transform.position,
    //        NextSpawnTime = 0.0f,
    //        SpawnRate = authoring.SpawnRate
    //    });
    //}
}
