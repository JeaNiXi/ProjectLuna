using Unity.Core;
using Unity.Entities;
using UnityEngine;

public class InputBridge : MonoBehaviour
{
    public static InputBridge Instance;
    private World world;
    private EntityManager entityManager;
    private TimeControlWorker timeControlWorker;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitializeECSConnection();
        InitializeWorkers();
    }
    private void InitializeECSConnection()
    {
        world = World.DefaultGameObjectInjectionWorld;
        entityManager = world.EntityManager;
    }
    private void InitializeWorkers()
    {
        timeControlWorker = new TimeControlWorker();
    }

    public void ToggleTimeState()
    {
        timeControlWorker.CreateTimeToggleRequestComponent(entityManager);
    }
}
