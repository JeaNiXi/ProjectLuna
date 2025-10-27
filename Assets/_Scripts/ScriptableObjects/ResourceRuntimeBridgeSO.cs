using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class ResourceRuntimeData
{
    public bool IsUpdated;
    public FixedString128Bytes ID;
    public int ResourceLevel;
    public float GatheringAmount;
    public float GatheringTime;

    public void SetUpdateStatusFalse()
    {
        IsUpdated = false;
    }
}


[CreateAssetMenu(fileName = "ResourceRuntimeBridge", menuName = "Scriptable Objects/Resources/ResourceRuntimeBridge")]
public class ResourceRuntimeBridgeSO : ScriptableObject
{

    public event Action<string> OnDataUpdated;

    public Dictionary<string, ResourceRuntimeData> DynamicDataStruct = new Dictionary<string, ResourceRuntimeData>();
    public List<ResourceRuntimeData> DataList = new List<ResourceRuntimeData>();

    public void InitializeDictionary(ResourceManagerSO data)
    {
        DynamicDataStruct.Clear();
        DataList.Clear();
        foreach (var category in data.CategoriesList)
        {
            var currentCategory = category;
            foreach (var type in currentCategory.TypeList)
            {
                var currentType = type;
                foreach (var resource in currentType.ResourceList)
                {
                    DynamicDataStruct.Add(resource.ID, new ResourceRuntimeData
                    {
                        IsUpdated = false,
                        ID = resource.ID,
                        ResourceLevel = resource.resourceLevel,
                        GatheringAmount = resource.BaseGatheringAmount,
                        GatheringTime = resource.BaseGatheringTime
                    });
                    if (DynamicDataStruct.TryGetValue(resource.ID, out var value))
                    {
                        DataList.Add(value);
                    }
                }
            }
        }
    }
    public ResourceRuntimeData GetData(string id)
    {
        if (DynamicDataStruct.TryGetValue(id, out var value)) { return value; }
        ;
        {
            MainDebug.E0002DataNotFoundInUIBridge(MainDebug.ErrorSeverity.Error, id);
            return null;
        }
    }
    public void SetNewData(FixedString128Bytes id, int resourceLevel, float gatheringAmount)
    {
        if (DynamicDataStruct.ContainsKey(id.ToString()))
        {
            DynamicDataStruct[id.ToString()] = new ResourceRuntimeData
            {
                IsUpdated = true,
                ID = id,
                ResourceLevel = resourceLevel,
                GatheringAmount = gatheringAmount,
                GatheringTime = DynamicDataStruct[id.ToString()].GatheringTime,
            };
            RebuildList();
        }
    }
    public void SetStatusUpdateFalse(FixedString128Bytes id)
    {
        if(DynamicDataStruct.ContainsKey(id.ToString()))
        {
            DynamicDataStruct[id.ToString()].SetUpdateStatusFalse();
        }
    }
    public void SetData(FixedString128Bytes id, ResourceRuntimeData data)
    {
        if (DynamicDataStruct.ContainsKey(id.ToString()))
        {
            DynamicDataStruct[id.ToString()] = data;
            RebuildList();
        }
        OnDataUpdated?.Invoke(id.ToString());
    }
    private void RebuildList()
    {
        DataList.Clear();
        foreach (var item in DynamicDataStruct)
        {
            DataList.Add(item.Value);
        }
    }
}


