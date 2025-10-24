using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

[BurstCompile]
public partial struct ResourceInitialBuilder : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceManagerComponent>();
        Debug.Log("RM Found");
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity refs = SystemAPI.GetSingletonEntity<ResourceManagerComponent>();
        ResourceManagerComponent data = state.EntityManager.GetComponentData<ResourceManagerComponent>(refs);

        ref var blob = ref data.ResourceBlobRef.Value;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        for (int i = 0; i < blob.ID.Length; i++)
        {
            var newEntity = ecb.CreateEntity();
            ecb.AddComponent(newEntity, new ResourceReferenceData
            {
                IdInBlob = blob.ID[i],
                Index = i
            });
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        Debug.Log($"Создано {blob.ID.Length} сущностей из ResourceBlob.");

        state.Enabled = false;
    }
    public void OnDestroy(ref SystemState state)
    {

    }
}
