using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
public partial struct TimeControlSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        if(!SystemAPI.HasSingleton<TimeState>())
        {
            Entity mainTimeState = state.EntityManager.CreateEntity(typeof(TimeState));
            state.EntityManager.SetComponentData<TimeState>(mainTimeState, new TimeState
            {
                IsRunning = true
            });
        }
    }
    public void OnDestroy(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(!SystemAPI.HasSingleton<TimeToggleRequest>())
            { return; }

        if(!SystemAPI.HasSingleton<TimeState>())
            { return; }

        ref TimeState timeState = ref SystemAPI.GetSingletonRW<TimeState>().ValueRW;
        timeState.IsRunning = !timeState.IsRunning;

        Entity toggleEntity = SystemAPI.GetSingletonEntity<TimeToggleRequest>();
        state.EntityManager.DestroyEntity(toggleEntity);
    }
}
