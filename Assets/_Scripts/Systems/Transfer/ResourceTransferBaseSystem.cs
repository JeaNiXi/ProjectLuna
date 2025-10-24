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
        {
            if (ResourceBridge.Instance == null)
                return;
            else
                bridgeSO = ResourceBridge.Instance.ResourceRuntimeBridgeSO;
        }
        var buffer = SystemAPI.GetSingletonBuffer<ResourceGatherTimeBuffer>();
        if (buffer.Length == 0)
            return;

        foreach (var change in buffer)
        {
            bridgeSO.SetData(change.ID, new ResourceRuntimeData
            {
                ID = change.ID,
                GatheringTime = change.GatheringTimeChangedValue,
            });
        }
        buffer.Clear();
    }

    protected override void OnDestroy()
    {

    }
}
