using Resources;
using UnityEngine;

namespace Managers
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [SerializeField] private ResourceDataSO _resourceDataSO;
        private ResourceData _resourceData = new ResourceData();

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        #region Initialization
        public void InitializeResourceSystem()
        {
            LoadResources(_resourceDataSO);
        }
        #endregion
        #region Resource Loaders
        private void LoadResources()
        {
            ResourceData resourceData = new ResourceData();
            _resourceData = resourceData;
        }
        private void LoadResources(ResourceDataSO resourceDataSO)
        {
            ResourceData resourceData = new ResourceData(resourceDataSO);
            _resourceData = resourceData;
        }
        #endregion
        #region GetResource
        public int GetResource(ResourceTypes.ResourceType resourceType)
        {
            return 0;
        }
        public int GetResource(ResourceList.Resources resource)
        {
            return _resourceData.GetResourceAmount(resource);
        }
        #endregion
    }
}
