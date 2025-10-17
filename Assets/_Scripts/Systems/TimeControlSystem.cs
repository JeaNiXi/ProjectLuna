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
        Entity _mainTimeState = state.EntityManager.CreateEntity(typeof(TimeState));
        state.EntityManager.SetComponentData<TimeState>(_mainTimeState, new TimeState
        {
            IsRunning = true
        });
    }
    public void OnDestroy(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(!SystemAPI.HasSingleton<TimeToggleRequest>())
            { return; }
        Entity toggleEntity = SystemAPI.GetSingletonEntity<TimeToggleRequest>();

        TimeState timeState = SystemAPI.GetSingleton<TimeState>();
        timeState.IsRunning = !timeState.IsRunning;
        SystemAPI.SetSingleton<TimeState>(timeState);

        state.EntityManager.DestroyEntity(toggleEntity);
    }
}
