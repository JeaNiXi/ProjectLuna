using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public partial class ResourceTransferBaseSystem : SystemBase
{
    private ResourceRuntimeBridgeSO bridgeSO;

    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        if (bridgeSO == null)
            if (ResourceBridge.Instance == null)
                return;
            else
                bridgeSO = ResourceBridge.Instance.ResourceRuntimeBridgeSO;
        CheckGatherAmountBuffer();
        CheckGatherTimeBuffer();
    }
    private void CheckGatherAmountBuffer()
    {
        DynamicBuffer<ResourceGatherAmountBuffer> amountBufferComponent = SystemAPI.GetSingletonBuffer<ResourceGatherAmountBuffer>();
        if (amountBufferComponent.Length == 0)
            return;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var change in amountBufferComponent)
        {
            bridgeSO.SetNewData(change.ID, change.NewResourceLevel, change.NewGatheringAmount, change.NewUpgradeCost);


            foreach (var (levelBuffer, entity) in SystemAPI.Query<DynamicBuffer<ResourceRealtimeLevelBuffer>>().WithEntityAccess())
            {
                for (int i = 0; i < levelBuffer.Length; i++)
                {
                    if (levelBuffer[i].ID == change.ID)
                    {
                        var newBufferData = new NativeList<ResourceRealtimeLevelBuffer>(levelBuffer.Length, Allocator.Temp);
                        for (int j = 0; j < levelBuffer.Length; j++)
                        {
                            var old = levelBuffer[j];
                            if (old.ID == change.ID)
                            {
                                old.Level = change.NewResourceLevel;
                            }
                            newBufferData.Add(old);
                        }
                        ecb.SetBuffer<ResourceRealtimeLevelBuffer>(entity).CopyFrom(newBufferData.AsArray());
                        newBufferData.Dispose();
                        break;
                    }
                }
            }
            Debug.Log($"ID = {change.ID}, new Level = {change.NewResourceLevel}, new amount = {change.NewGatheringAmount}, new cost = {change.NewUpgradeCost}");

        }

        var bufferEntity = SystemAPI.GetSingletonEntity<ResourceGatherAmountBuffer>();
        ecb.SetBuffer<ResourceGatherAmountBuffer>(bufferEntity).Clear();
        ecb.Playback(World.EntityManager);
        ecb.Dispose();
    }



    private void CheckGatherTimeBuffer()
    {
        DynamicBuffer<ResourceGatherTimeBuffer> timeBuffercomponent = SystemAPI.GetSingletonBuffer<ResourceGatherTimeBuffer>();
        if (timeBuffercomponent.Length == 0)
            return;
        foreach (var change in timeBuffercomponent)
        {
            bridgeSO.SetNewData(change.ID, change.NewGatheringTime);
        }
        timeBuffercomponent.Clear();
    }

    protected override void OnDestroy()
    {

    }
}
