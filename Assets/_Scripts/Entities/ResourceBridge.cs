using System;
using Unity.Entities;
using UnityEngine;

public class ResourceBridge : MonoBehaviour
{
    public static ResourceBridge Instance;

    public ResourceManagerSO ResourceManagerSO;
    public ResourceRuntimeBridgeSO ResourceRuntimeBridgeSO;
    private World world;
    private EntityManager entityManager;

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
        InitializeEntitySystems();
        InitializeResourceBridgeSO();
        CreateBridgeCompletedComponent();
    }
    private void InitializeEntitySystems()
    {
        world = World.DefaultGameObjectInjectionWorld;
        entityManager = world.EntityManager;
    }
    private void InitializeResourceBridgeSO()
    {
        ResourceRuntimeBridgeSO.InitializeDictionary(ResourceManagerSO);
    }
    private void CreateBridgeCompletedComponent()
    {
        Entity ResourceRuntimeEntity = entityManager.CreateEntity();
        entityManager.AddComponentData(ResourceRuntimeEntity, new ResourceRuntimeBridgeCompleted());
    }
}
