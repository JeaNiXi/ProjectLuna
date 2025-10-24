using System.Resources;
using Unity.Entities;
using UnityEngine;

public partial struct DebugAllResources : ISystem
{
    //EntityQuery query;
    //public void OnCreate(ref SystemState state)
    //{
    //    state.RequireForUpdate<ResourceManagerComponent>();
    //    query = state.GetEntityQuery(typeof(ResourceBaseData));
    //}
    //public void OnDestroy(ref SystemState state)
    //{

    //}
    //public void OnUpdate(ref SystemState state)
    //{
    //    var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
    //    foreach (var entity in entities)
    //    {
    //        var resourceManager = state.EntityManager.GetComponentData<ResourceManagerComponent>(entity);
    //        ref var blob = ref resourceManager.ResourceBlobRef.Value;
    //        Debug.Log(blob.ID.Length);
    //        for (int i = 0; i < blob.ID.Length; i++)
    //        {
    //            var category = (ResourceCategories.ResourceCategoriesList)blob.ResourceCategory[i];
    //            var type = (ResourceTypes.ResourceTypesList)blob.ResourceType[i];
    //            Debug.Log($"Resource #{i}: ID = {blob.ID[i]}, Category = {category}, Type = {type}, BaseTime = {blob.BaseGatheringTime[i]}, BaseAmount = {blob.BaseGatheringAmount[i]}");
    //        }
    //    }
    //    entities.Dispose();
    //    state.Enabled = false;
    //}
}
