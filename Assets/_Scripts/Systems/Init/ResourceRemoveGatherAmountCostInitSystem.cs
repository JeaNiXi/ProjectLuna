using Unity.Entities;
using UnityEngine;

public partial struct ResourceRemoveGatherAmountCostInitSystem : ISystem
{
  public void OnCreate(ref SystemState state)
    {
        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddBuffer<ResourceRemoveGatherAmountCostBuffer>(entity);
    }
}