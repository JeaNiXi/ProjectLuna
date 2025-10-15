using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

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

        private void Start()
        {
            InitializeResourceSystem();
            Debug.Log(ResourceManager.Instance.GetResource(ResourceList.Resources.OakWood));
        }

        #region Initialization
        private void InitializeResourceSystem()
        {
            ResourceManager.Instance.InitializeResourceSystem();
        }
        #endregion
    }
}