using Unity.Entities;
using UnityEngine;

public partial struct ResourceGatherAmountInitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddBuffer<ResourceGatherAmountBuffer>(entity);
    }
}
