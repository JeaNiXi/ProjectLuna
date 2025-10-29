using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UIDocument MainUIDocument;

    private VisualElement RootVE;

    [SerializeField] private VisualTreeAsset resourcesMainAsset;
    [SerializeField] private VisualTreeAsset buildingsMainAsset;

    [SerializeField] private ResourceManagerSO resourceManagerSO;
    [SerializeField] private BuildingManagerSO buildingManagerSO;

    //private IPageController resourcePageControllerRef;
    //private ResourcePageController resourcePageController;

    private Button categoryResourceButton;
    private Button categoryBuildingsButton;

    private VisualElement mainView;

    private Dictionary<string, VisualElement> cachedPages;
    private Dictionary<string, IPageController> cachedPageControllers;

    private VisualElement currentPage;
    private IPageController currentController;
    private string currentPageName;

    private bool IsChangingPage = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        RootVE = MainUIDocument.rootVisualElement;
        mainView = RootVE.Q<VisualElement>("mainView");

        cachedPages = new Dictionary<string, VisualElement>(); // Создаём словарь со всеми страницами.
        cachedPageControllers = new Dictionary<string, IPageController>(); //Создаём словарь со всеми контроллерами для страниц.
    }
    private void Start()
    {
        InitializePagesDictionary();
        InitializeButtons();
        InitializeButtonEvents();
        //GetControllerRefs();
    }
    private void LateUpdate()
    {
        UpdateUI();
    }
    private void UpdateUI()
    {
        if (!IsChangingPage && currentController != null)
            currentController.UpdateUI();
    }

    private void InitializePagesDictionary()
    {
        CachePage("resources", resourcesMainAsset, new ResourcePageController(), resourceManagerSO);
        CachePage("buildings", buildingsMainAsset, new BuildingsPageController(), buildingManagerSO);
    }
    private void CachePage(string category, VisualTreeAsset asset, IPageController controller, ScriptableObject data)
    {
        VisualElement newPage = new VisualElement()
        {
            style =
            {
                flexGrow = 1,
                width = Length.Percent(100f),
                height=Length.Percent(100f),
            }
        };
        asset.CloneTree(newPage);

        controller.InitializePage(newPage, data);

        newPage.style.display = DisplayStyle.None;
        cachedPages.Add(category, newPage);
        cachedPageControllers.Add(category, controller);
        //switch (category)
        //{
        //    case "resources":
        //        resourcePageControllerRef = controller;
        //        break;
        //    case "buildings":
        //        break;
        //}
    }
    //private void GetControllerRefs()
    //{
    //    resourcePageController = (ResourcePageController)resourcePageControllerRef;
    //}
    private void InitializeButtons()
    {
        categoryResourceButton = RootVE.Q<Button>("resourceButton");
        categoryBuildingsButton = RootVE.Q<Button>("buildingsButton");
    }
    private void InitializeButtonEvents()
    {
        categoryResourceButton.clicked += () => ShowPage("resources");
        categoryBuildingsButton.clicked += () => ShowPage("buildings");
    }
    private void ShowPage(string newPage)
    {
        if (currentPage != null && currentController != null)
        {
            IsChangingPage = true;
            currentController.HidePage();
            Debug.Log("Hiding Page " + currentPageName);
        }
        if (cachedPages.TryGetValue(newPage, out var page) && cachedPageControllers.TryGetValue(newPage, out var newController))
        {
            mainView.Add(page);
            newController.ShowPage();
            currentPage = page;
            currentController = newController;
            currentPageName = newPage;
            Debug.Log("Showing Page " + newPage);
            IsChangingPage = false;
        }
    }

    private void OnDestroy()
    {
        if (categoryResourceButton != null)
        {
            categoryResourceButton.clicked -= () => ShowPage("resources");
        }
        if (categoryBuildingsButton != null)
        {
            categoryBuildingsButton.clicked -= () => ShowPage("buildings");
        }
    }
}
