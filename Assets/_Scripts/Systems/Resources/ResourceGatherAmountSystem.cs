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
        var costBuffer = SystemAPI.GetSingletonBuffer<ResourceRemoveGatherAmountCostBuffer>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (flag, entity) in SystemAPI.Query<RefRO<ResourceGatherAmountFlag>>().WithEntityAccess())
        {
            Debug.Log("ResourceGatherAmountSystem is RUNNING");
            costBuffer.Add(new ResourceRemoveGatherAmountCostBuffer
            {
                ID = flag.ValueRO.ID,
                ResourceUpgradeCost = GetCurrentUpgradeCost(flag.ValueRO.BaseUpgradeCost, flag.ValueRO.UpgradeCostMultiplayer, flag.ValueRO.ResourceLevel),
            });
            buffer.Add(new ResourceGatherAmountBuffer
            {
                ID = flag.ValueRO.ID,
                NewResourceLevel = GetNewResourceLevel(flag.ValueRO.ResourceLevel),
                NewGatheringAmount = GetGatherAmountUpgrade(flag.ValueRO.ResourceLevel, flag.ValueRO.BaseGatheringAmount, flag.ValueRO.GatheringAmountMultiplayer),
                NewUpgradeCost = GetNextUpgradeCost(flag.ValueRO.BaseUpgradeCost, flag.ValueRO.UpgradeCostMultiplayer, flag.ValueRO.ResourceLevel),
            });
            Debug.Log($"Upgraded Resource :{flag.ValueRO.ID.ToString()}, level = {GetNewResourceLevel(flag.ValueRO.ResourceLevel)}, new amount: {GetGatherAmountUpgrade(flag.ValueRO.ResourceLevel, flag.ValueRO.BaseGatheringAmount, flag.ValueRO.GatheringAmountMultiplayer)}");
            ecb.RemoveComponent<ResourceGatherAmountFlag>(entity);
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    private float GetGatherAmountUpgrade(int level, float baseGatherinAmount, float multiplayer) => Mathf.Round(baseGatherinAmount * Mathf.Pow(multiplayer, ++level - 1));
    private float GetNextUpgradeCost(float baseCost, float costGrowth, int level) => Mathf.Round(baseCost * Mathf.Pow(costGrowth, ++level - 1));
    private float GetCurrentUpgradeCost(float baseCost, float costGrowth, int level) => Mathf.Round(baseCost * Mathf.Pow(costGrowth, level - 1));

    private int GetNewResourceLevel(int oldLevel) => ++oldLevel;
    public void OnDestroy(ref SystemState state)
    {

    }
}
