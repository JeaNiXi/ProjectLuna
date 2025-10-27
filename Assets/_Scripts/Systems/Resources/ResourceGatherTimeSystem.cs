using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct ResourceGatherTimeSystem : ISystem
{
    ResourceManagerComponent managerComponent;
    BlobAssetReference<ResourceBlob> blobRef;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceManagerComponent>();
        //state.RequireForUpdate<ResourceRuntimeBridgeCompleted>();
        state.RequireForUpdate<ResourceGatherTimeFlag>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Debug.Log("Updating Now");
        var buffer = SystemAPI.GetSingletonBuffer<ResourceGatherTimeBuffer>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        if (!blobRef.IsCreated)
            if (SystemAPI.TryGetSingleton<ResourceManagerComponent>(out managerComponent) && managerComponent.ResourceBlobRef.IsCreated)
                blobRef = managerComponent.ResourceBlobRef;
            else
            {
                ecb.Dispose();
                return;
            }

        foreach (var (flag, entity) in SystemAPI.Query<RefRO<ResourceGatherTimeFlag>>().WithEntityAccess())
        {
            int blobIndex;
            foreach (var (refData, blobEntity) in SystemAPI.Query<RefRO<ResourceReferenceData>>().WithEntityAccess())
            {
                if (flag.ValueRO.ID == refData.ValueRO.IdInBlob)
                {
                    blobIndex = refData.ValueRO.Index;
                    buffer.Add(new ResourceGatherTimeBuffer
                    {
                        ID = flag.ValueRO.ID,
                        GatheringTimeChangedValue = blobRef.Value.BaseGatheringTime[blobIndex] + flag.ValueRO.GatheringTimeChangeValue,
                    });
                    var value = flag.ValueRO.GatheringTimeChangeValue;
                    Debug.Log($"[ResourceGatherSpeedSystem] Entity {entity.Index} — GatheringSpeedChangeValue = + {value}");
                }
            }

            ecb.RemoveComponent<ResourceGatherTimeFlag>(entity);
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    public void OnDestroy(ref SystemState state)
    {

    }
}
