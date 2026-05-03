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
    public float UpgradeCost;

    public void SetUpdateStatusFalse()
    {
        IsUpdated = false;
    }
}


[CreateAssetMenu(fileName = "ResourceRuntimeBridge", menuName = "Scriptable Objects/Resources/ResourceRuntimeBridge")]
public class ResourceRuntimeBridgeSO : ScriptableObject
{
    public Dictionary<string, ResourceRuntimeData> DynamicData = new Dictionary<string, ResourceRuntimeData>();
    public List<ResourceRuntimeData> DataList = new List<ResourceRuntimeData>();

    public void InitializeDictionary(ResourceManagerSO data)
    {
        DynamicData.Clear();
        DataList.Clear();
        foreach (var category in data.CategoriesList)
        {
            var currentCategory = category;
            foreach (var type in currentCategory.TypeList)
            {
                var currentType = type;
                foreach (var resource in currentType.ResourceList)
                {
                    DynamicData.Add(resource.ID, new ResourceRuntimeData
                    {
                        IsUpdated = false,
                        ID = resource.ID,
                        ResourceLevel = resource.resourceLevel,
                        GatheringAmount = resource.BaseGatheringAmount,
                        GatheringTime = resource.BaseGatheringTime,
                        UpgradeCost = resource.BaseUpgradeCost,
                    });
                    if (DynamicData.TryGetValue(resource.ID, out var value))
                    {
                        DataList.Add(value);
                    }
                }
            }
        }
    }
    public ResourceRuntimeData GetData(string id)
    {
        if (DynamicData.TryGetValue(id, out var value)) { return value; }
        ;
        {
            MainDebug.E0002DataNotFoundInUIBridge(MainDebug.ErrorSeverity.Error, id);
            return null;
        }
    }
    public void SetNewData(FixedString128Bytes id, int resourceLevel, float gatheringAmount, float upgradeCost)
    {
        if (DynamicData.ContainsKey(id.ToString()))
        {
            DynamicData[id.ToString()] = new ResourceRuntimeData
            {
                IsUpdated = true,
                ID = id,
                ResourceLevel = resourceLevel,
                GatheringAmount = gatheringAmount,
                GatheringTime = DynamicData[id.ToString()].GatheringTime,
                UpgradeCost = upgradeCost,
            };
            RebuildList();
        }
    }
    public void SetNewData(FixedString128Bytes id, float gatheringTime)
    {
        if (DynamicData.ContainsKey(id.ToString()))
        {
            DynamicData[id.ToString()] = new ResourceRuntimeData
            {
                IsUpdated = true,
                ID = id,
                ResourceLevel = DynamicData[id.ToString()].ResourceLevel,
                GatheringAmount = DynamicData[id.ToString()].GatheringAmount,
                GatheringTime = gatheringTime,
            };
            RebuildList();
        }
    }
    public void SetStatusUpdateFalse(FixedString128Bytes id)
    {
        if (DynamicData.ContainsKey(id.ToString()))
        {
            DynamicData[id.ToString()].SetUpdateStatusFalse();
        }
    }
    private void RebuildList()
    {
        DataList.Clear();
        foreach (var item in DynamicData)
        {
            DataList.Add(item.Value);
        }
    }
}