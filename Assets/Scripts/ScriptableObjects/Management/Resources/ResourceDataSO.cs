using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
    [CreateAssetMenu(fileName = "ResourceData", menuName = "Scriptable Objects/Management/Resources/ResourceData")]
    public class ResourceDataSO : ScriptableObject
    {
        public int OakWoodAmount;
    }
}