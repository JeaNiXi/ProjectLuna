using Unity.Entities;
using UnityEngine;

public class TimeControlWorker
{
    public void CreateTimeToggleRequestComponent(EntityManager entityManager)
    {
        entityManager.CreateEntity(typeof(TimeToggleRequest));
    }
}
