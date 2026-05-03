using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct ResourceGatheringSystem : ISystem
{
    private float currentTime;
    private float updateInterval;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceRealtimeLevelBuffer>();
        state.RequireForUpdate<ResourceIsGatheringBuffer>();
        state.RequireForUpdate<ResourceIsGatheringFlag>();
        currentTime = 0;
        updateInterval = 1f;
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        currentTime += Time.deltaTime;
        if (currentTime < updateInterval)
            return;
        Debug.Log("[Started Gathering System]");
        var buffer = SystemAPI.GetSingletonBuffer<ResourceIsGatheringBuffer>();
        var realtimeLevelBuffer = SystemAPI.GetSingletonBuffer<ResourceRealtimeLevelBuffer>();

        foreach (var (flag, entity) in SystemAPI.Query<RefRO<ResourceIsGatheringFlag>>().WithEntityAccess())
        {
            bool exists = false;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].ID == flag.ValueRO.ID)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                realtimeLevelBuffer.Add(new ResourceRealtimeLevelBuffer
                {
                    ID = flag.ValueRO.ID,
                    Level = flag.ValueRO.Level,
                });
                buffer.Add(new ResourceIsGatheringBuffer
                {
                    ID = flag.ValueRO.ID,
                    CurrentLevel = GetLevelBufferData(ref state, flag.ValueRO.ID),
                    AmountToAdd = GetGatherAmount(GetLevelBufferData(ref state, flag.ValueRO.ID), flag.ValueRO.BaseAmount, flag.ValueRO.ProductionMultiplayer),
                });
                Debug.Log($"Added Resource : {GetGatherAmount(GetLevelBufferData(ref state, flag.ValueRO.ID), flag.ValueRO.BaseAmount, flag.ValueRO.ProductionMultiplayer)}");
            }
        }
        currentTime = 0;
    }
    [BurstCompile]
    private float GetGatherAmount(int level, float baseProduction, float multiplayer)
    {
        Debug.Log($"Base Prod = {baseProduction}, Multiplayer = {multiplayer}, Level = {level}");
        return Mathf.Round(baseProduction * Mathf.Pow(multiplayer, level - 1));
    }
    private int GetLevelBufferData(ref SystemState state, FixedString128Bytes id)
    {
        foreach (var data in SystemAPI.GetSingletonBuffer<ResourceRealtimeLevelBuffer>())
        {
            if (id == data.ID)
                return data.Level;
        }
        return 0;
    }
    public void OnDestroy(ref SystemState state)
    {

    }
}
