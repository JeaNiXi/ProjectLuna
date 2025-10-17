using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct ResourceGatheringSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        Debug.Log("R Gathering System Created");
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //foreach(var resource in SystemAPI.Query<RefRO<ResourceManagerComponent>>())
        //{
        //    Debug.Log($"Обновление ресурса ID {resource.ValueRO.ResourceBlobRef.Value.IDHashes}: время сбора {resource.ValueRO.ResourceBlobRef.Value.BaseGatheringTimes[0]}");
        //}
    }
    public void UpdateOnce(ref SystemState state)
    {
        foreach (var resource in SystemAPI.Query<RefRO<ResourceManagerComponent>>())
        {
            Debug.Log($"Обновление ресурса ID {resource.ValueRO.ResourceBlobRef.Value.IDHashes}: время сбора {resource.ValueRO.ResourceBlobRef.Value.BaseGatheringTimes[0]}");
        }
    }
}
