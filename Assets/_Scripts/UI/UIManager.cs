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

    private Button categoryResourceButton;
    private Button categoryBuildingsButton;

    private VisualElement mainView;

    private Dictionary<string, VisualElement> cachedPages;
    private Dictionary<string, IPageController> cachedPageControllers;

    private VisualElement currentPage;
    private IPageController currentController;
    private string currentPageName;


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
    }
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
            currentController.HidePage();
            Debug.Log("Hiding Page " + currentPageName);
            //mainView.Remove(currentPage); Использовать если надо удалить страницу. Пока не делаю.
        }
        if(cachedPages.TryGetValue(newPage, out var page) && cachedPageControllers.TryGetValue(newPage, out var newController))
        {
            mainView.Add(page);
            newController.ShowPage();
            currentPage = page;
            currentController = newController;
            currentPageName = newPage;
            Debug.Log("Showing Page " +  newPage);
        }
    }

    private void OnDestroy()
    {
        if (categoryResourceButton != null)
        {
            categoryResourceButton.clicked -= () => ShowPage("resources");
        }
        if(categoryBuildingsButton != null)
        {
            categoryBuildingsButton.clicked -= () => ShowPage("buildings");
        }
    }
    // КОД НИЖЕ НЕ ИСПОЛЬЗУЕТСЯ





    //private ListView WoodResourceList;
    /// <summary>
    const int itemCount = 4;
    List<string> WoodResourceList = new List<string>(itemCount);

    /// </summary>


    private void InitializeVisualElements()
    {
    }
    private void InitializeUIComponents()
    {
        //WoodResourceList = RootVE.Q<ListView>("WoodResourceList");
    }
    private void CreateList()
    {
        for (var i = 0; i < itemCount; i++)
            WoodResourceList.Add(i.ToString());


        Func<VisualElement> makeItem = () => new UIResourcePanel();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            var panel = e as UIResourcePanel;
            if (panel != null)
            {
                panel.Label.text = "Test Name";
                panel.ProgressBar.value = 0f;
                panel.Button.text = "Test Text";
            }
        };
        var listview = RootVE.Q<ListView>("WoodResourceList");
        if (listview != null)
        {
            listview.makeItem = makeItem;
            listview.bindItem = bindItem;
            listview.itemsSource = WoodResourceList;
        }
        else
        {
            Debug.Log("Damn List 404, Felicia Sumimaseee");
        }


        //listView.itemsChosen += (selectedItems) =>
        //{
        //    Debug.Log("Items chosen: " + string.Join(", ", selectedItems));
        //};

        //// Callback invoked when the user changes the selection inside the ListView
        //listView.selectedIndicesChanged += (selectedIndices) =>
        //{
        //    Debug.Log("Index selected: " + string.Join(", ", selectedIndices));

        //    // Note: selectedIndices can also be used to get the selected items from the itemsSource directly or
        //    // by using listView.viewController.GetItemForIndex(index).
        //};

        // Callback invoked when the user double clicks an item

    }
}
