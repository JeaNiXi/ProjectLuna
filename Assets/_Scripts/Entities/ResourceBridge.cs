using System;
using Unity.Entities;
using UnityEngine;

public class ResourceBridge : MonoBehaviour
{
    public static ResourceBridge Instance;

    public ResourceManagerSO ResourceManagerSO;
    public ResourceRuntimeBridgeSO ResourceRuntimeBridgeSO;
    public ResourceRuntimeAmountSO ResourceRuntimeAmountSO;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitializeResourceBridgeSO();
        InitializeResourceRuntimeAmountSO();
    }
    private void InitializeResourceBridgeSO()
    {
        ResourceRuntimeBridgeSO.InitializeDictionary(ResourceManagerSO);
    }
    private void InitializeResourceRuntimeAmountSO()
    {
        ResourceRuntimeAmountSO.InitializeDictionary(ResourceManagerSO);
    }
}
