using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class ResourceRuntimeAmountData
{
    public FixedString128Bytes ID;
    public float Amount;
}

[CreateAssetMenu(fileName = "ResourceRuntimeAmount", menuName = "Scriptable Objects/Resources/ResourceRuntimeAmount")]
public class ResourceRuntimeAmountSO : ScriptableObject
{
    public Dictionary<string, ResourceRuntimeAmountData> AmountData = new Dictionary<string, ResourceRuntimeAmountData>();
    public List<ResourceRuntimeAmountData> AmountList = new List<ResourceRuntimeAmountData>();

    public void InitializeDictionary(ResourceManagerSO data)
    {
        AmountData.Clear();
        AmountList.Clear();
        foreach (var category in data.CategoriesList)
        {
            var currentCategory = category;
            foreach (var type in currentCategory.TypeList)
            {
                var currentType = type;
                foreach (var resource in currentType.ResourceList)
                {
                    AmountData.Add(resource.ID, new ResourceRuntimeAmountData
                    {
                        ID = resource.ID,
                        Amount = 0f
                    });
                    if (AmountData.TryGetValue(resource.ID, out var value))
                    {
                        AmountList.Add(value);
                    }
                }
            }
        }

    }
    public void AddNewAmount(FixedString128Bytes id, float amount)
    {
        if (AmountData.ContainsKey(id.ToString()))
        {
            AmountData[id.ToString()].Amount += amount;
        }
        RebuidList();
    }
    public void RemoveAmount(FixedString128Bytes id, float amount)
    {
        if(AmountData.ContainsKey(id.ToString()))
        {
            AmountData[id.ToString()].Amount -= amount;
        }
    }
    public ResourceRuntimeAmountData GetData(string id)
    {
        if (AmountData.TryGetValue(id, out var value)) { return value; }
        MainDebug.E0002DataNotFoundInUIBridge(MainDebug.ErrorSeverity.Error, id);
        return null;
    }
    private void RebuidList()
    {
        AmountList.Clear();
        foreach (var item in AmountData)
        {
            AmountList.Add(item.Value);
        }
    }
}
