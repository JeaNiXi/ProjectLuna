using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

[BurstCompile]
public partial struct ResourceGatherTimeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceGatherAmountBuffer>();
        state.RequireForUpdate<ResourceGatherTimeFlag>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Debug.Log("[ResourceGatherTimeSystem]: Changing Gather Time: ");
        var buffer = SystemAPI.GetSingletonBuffer<ResourceGatherTimeBuffer>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (flag, entity) in SystemAPI.Query<RefRO<ResourceGatherTimeFlag>>().WithEntityAccess())
        {
            buffer.Add(new ResourceGatherTimeBuffer
            {
                ID = flag.ValueRO.ID,
                NewGatheringTime = GetGatherTimeUpgrade(flag.ValueRO.CurrentGatheringTime, flag.ValueRO.GatheringTimeMultiplayer)
            });
            Debug.Log($"Upgraded Resource Gathering Time:{flag.ValueRO.ID.ToString()}, old amount: {flag.ValueRO.CurrentGatheringTime}, new amount: {GetGatherTimeUpgrade(flag.ValueRO.CurrentGatheringTime, flag.ValueRO.GatheringTimeMultiplayer)}");

            ecb.RemoveComponent<ResourceGatherTimeFlag>(entity);
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    private float GetGatherTimeUpgrade(float currentGatheringTime, float multiplayer) => currentGatheringTime * multiplayer;
    public void OnDestroy(ref SystemState state)
    {

    }
}
