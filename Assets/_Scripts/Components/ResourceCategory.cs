using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct ResourceCategory : IComponentData
{
    public FixedString128Bytes CategoryName;
}
