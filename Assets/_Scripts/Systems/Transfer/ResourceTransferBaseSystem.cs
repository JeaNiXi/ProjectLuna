using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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

        DynamicBuffer<ResourceGatherAmountBuffer> amountBufferComponent = SystemAPI.GetSingletonBuffer<ResourceGatherAmountBuffer>();
        if (amountBufferComponent.Length == 0)
            return;

        foreach (var change in amountBufferComponent)
        {
            bridgeSO.SetNewData(change.ID, change.NewResourceLevel, change.NewGatheringAmount);
        }
        amountBufferComponent.Clear();
    }

    protected override void OnDestroy()
    {

    }
}
