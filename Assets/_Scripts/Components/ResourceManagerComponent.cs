using NUnit.Framework;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ResourceManagerComponent : IComponentData
{
    public BlobAssetReference<ResourceBlob> ResourceBlobRef;

}
