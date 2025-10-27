using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public partial struct ResourceGatherAmountSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceGatherAmountBuffer>();
        state.RequireForUpdate<ResourceGatherAmountFlag>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Debug.Log("[ResourceGatherAmountSystem]: Upgrading Building: ");
        var buffer = SystemAPI.GetSingletonBuffer<ResourceGatherAmountBuffer>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (flag, entity) in SystemAPI.Query<RefRO<ResourceGatherAmountFlag>>().WithEntityAccess())
        {
            buffer.Add(new ResourceGatherAmountBuffer
            {
                ID = flag.ValueRO.ID,
                NewResourceLevel = GetNewResourceLevel(flag.ValueRO.ResourceLevel),
                NewGatheringAmount = GetGatherAmountUpgradeFloat(flag.ValueRO.CurrentGatheringAmount, flag.ValueRO.GatheringAmountMultiplayer),
            });
            Debug.Log($"Upgraded Resource :{flag.ValueRO.ID.ToString()}, old amount: {flag.ValueRO.CurrentGatheringAmount}, new amount: {GetGatherAmountUpgradeFloat(flag.ValueRO.CurrentGatheringAmount, flag.ValueRO.GatheringAmountMultiplayer)}");
            ecb.RemoveComponent<ResourceGatherAmountFlag>(entity);
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    private float GetGatherAmountUpgradeFloat(float currentGatherAmount, float multiplayer) => multiplayer * currentGatherAmount;
    private int GetNewResourceLevel(int oldLevel) => oldLevel++;
    public void OnDestroy(ref SystemState state)
    {

    }
}
