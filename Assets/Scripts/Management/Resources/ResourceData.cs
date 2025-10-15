using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
    public class ResourceData
    {
        private List<Resource> ResourcesList;
        private int _oakAmount;
        public int OakAmount
        {
            get { return _oakAmount; }
            set { _oakAmount = value; }
        }

        public ResourceData()
        {
            _oakAmount = 0;
        }

        public ResourceData(ResourceDataSO resourceDataSO)
        {
            _oakAmount = resourceDataSO.OakWoodAmount;
        }
        public int GetResourceAmount(Resource resource)
        {
            switch (resource.Id)
            {
                case 001:
                    return OakAmount;
                default: return 0;
            }
        }
        public int GetResourceAmount(ResourceList.Resources resource)
        {
            switch (resource)
            {
                case ResourceList.Resources.OakWood:
                    return OakAmount;
                default: return 0;
            }
        }
    }
}