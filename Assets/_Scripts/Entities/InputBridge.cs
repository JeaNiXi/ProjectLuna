using Unity.Core;
using Unity.Entities;
using UnityEngine;

public class InputBridge : MonoBehaviour
{
    public static InputBridge Instance;
    private World _world;
    private EntityManager _entityManager;
    private TimeControlWorker _timeControlWorker;

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
        _world = World.DefaultGameObjectInjectionWorld;
        _entityManager = _world.EntityManager;
    }
    private void InitializeWorkers()
    {
        _timeControlWorker = new TimeControlWorker();
    }

    public void ToggleTimeState()
    {
        _timeControlWorker.CreateTimeToggleRequestComponent(_entityManager);
    }
}
