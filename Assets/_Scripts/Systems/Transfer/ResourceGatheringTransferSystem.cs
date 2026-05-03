using Unity.Entities;
using UnityEngine;

public partial class ResourceGatheringTransferSystem : SystemBase
{
    private ResourceRuntimeAmountSO amountBridgeSO;

    protected override void OnCreate()
    {

    }

    protected override void OnUpdate()
    {
        if (amountBridgeSO == null)
            if (ResourceBridge.Instance == null)
                return;
            else
                amountBridgeSO = ResourceBridge.Instance.ResourceRuntimeAmountSO;
        CheckForAmountBuffer();
        CheckRemoveGatherAmountCostBuffer();
    }
    private void CheckForAmountBuffer()
    {
        DynamicBuffer<ResourceIsGatheringBuffer> gatheringAmountBuffer = SystemAPI.GetSingletonBuffer<ResourceIsGatheringBuffer>();
        if (gatheringAmountBuffer.Length == 0)
            return;
        foreach (var element in gatheringAmountBuffer)
        {
            amountBridgeSO.AddNewAmount(element.ID, element.AmountToAdd);
        }
        gatheringAmountBuffer.Clear();
    }
    private void CheckRemoveGatherAmountCostBuffer()
    {
        DynamicBuffer<ResourceRemoveGatherAmountCostBuffer> costBuffer = SystemAPI.GetSingletonBuffer<ResourceRemoveGatherAmountCostBuffer>();
        if (costBuffer.Length == 0) return;
        foreach (var cost in costBuffer)
        {
            amountBridgeSO.RemoveAmount(cost.ID, cost.ResourceUpgradeCost);
        }
        costBuffer.Clear();
    }
    protected override void OnDestroy()
    {

    }
}
