using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CombinedResourceData
{
    public FixedString128Bytes ID;
    public ResourceSO ResourceSO;
    public ResourceRuntimeData ResourceRuntimeData;

    public CombinedResourceData(FixedString128Bytes id, ResourceSO so, ResourceRuntimeData data)
    {
        ID = id;
        ResourceSO = so;
        ResourceRuntimeData = data;
    }
}
public class ResourcePageController : IPageController
{
    private VisualElement page;
    private VisualTreeAsset resourcePanelAsset;
    private ResourceManagerSO data;
    private ResourceRuntimeBridgeSO uiBridge;
    private TreeView treeView;
    private ListView listView;

    private World world;
    private EntityManager entityManager;

    private List<TreeViewItemData<string>> treeData;
    private Dictionary<string, List<ResourceSO>> listData;
    private Dictionary<ResourceSO, string> runtimeListDataIndex;

    float debugfloat = 0;


    public void InitializePage(VisualElement page, ScriptableObject data)
    {
        this.page = page;
        this.data = data as ResourceManagerSO;
        if (this.data == null)
            MainDebug.E0001WrongScriptableObjectCast(MainDebug.ErrorSeverity.Error, typeof(ResourceManagerSO).Name, data.GetType().Name);
        treeView = page.Q<TreeView>("treeView");
        listView = page.Q<ListView>("listView");

        world = World.DefaultGameObjectInjectionWorld;
        entityManager = world.EntityManager;

        resourcePanelAsset = Resources.Load<VisualTreeAsset>("UI/Panels/UIResourcePanel");
        uiBridge = Resources.Load<ResourceRuntimeBridgeSO>("Resource/ResourceRuntimeBridge");
        if (uiBridge != null)
            Debug.Log("UI Bridge Loaded Successfuly! ");

        uiBridge.OnDataUpdated += HandleDataChange;

        InitializeData(this.data);
        InitializeTreeView();
    }

    private void InitializeData(ResourceManagerSO data)
    {
        treeData = new List<TreeViewItemData<string>>();
        listData = new Dictionary<string, List<ResourceSO>>();
        runtimeListDataIndex = new Dictionary<ResourceSO, string>();
        int index = 0;

        foreach (ResourceCategorySO category in data.CategoriesList)
        {
            List<TreeViewItemData<string>> treeNodes = new List<TreeViewItemData<string>>();
            foreach (ResourceTypeSO type in category.TypeList)
            {
                index++;
                treeNodes.Add(new TreeViewItemData<string>(index, type.ResourceTypeName));

                listData[type.ResourceTypeName] = type.ResourceList
                    .Where(r => r != null)
                    .ToList();
                foreach (ResourceSO resource in type.ResourceList)
                {
                    runtimeListDataIndex.Add(resource, resource.ID);
                }
            }
            index++;
            treeData.Add(new TreeViewItemData<string>(index, category.CategoryName, treeNodes));
        }
    }
    private void InitializeTreeView()
    {
        treeView.makeItem = () => new Label()
        {
            style =
                {

            }
        };
        treeView.bindItem = (element, index) =>
        {
            string item = treeView.GetItemDataForIndex<string>(index);
            (element as Label).text = item;
        };
        treeView.SetRootItems(treeData);
        treeView.selectionType = SelectionType.Single;
        treeView.selectionChanged += selections =>
        {
            string selectedItem = selections.FirstOrDefault() as string;
            if (selectedItem != null && listData.ContainsKey(selectedItem))
            {
                InitializeListView(selectedItem);
                UpdateListView();
            }
        };
    }

    private void InitializeListView(string selectedType)
    {
        if (listData.TryGetValue(selectedType, out var resList))
        {
            List<CombinedResourceData> combinedResourceDataList = new List<CombinedResourceData>();
            foreach (var res in resList)
            {
                combinedResourceDataList.Add(new CombinedResourceData(res.ID, res, uiBridge.GetData(res.ID)));
            }
            listView.itemsSource = combinedResourceDataList;
        }
    }
    private void UpdateListView()
    {
        listView.makeItem = () =>
        {
            TemplateContainer ve = resourcePanelAsset.CloneTree();
            return ve;
        };
        listView.bindItem = (element, index) =>
        {
            List<CombinedResourceData> combinedResourceData = listView.itemsSource as List<CombinedResourceData>;

            Label nameLabel = element.Q<Label>("resource-name-label");
            Label currentGatheringTime = element.Q<Label>("current-gathering-time");
            Button changeFloatButton = element.Q<Button>("add-float");

            nameLabel.text = combinedResourceData[index].ResourceSO.ResourceNameKey;
            currentGatheringTime.text = combinedResourceData[index].ResourceRuntimeData.GatheringTime.ToString();

            if(changeFloatButton.userData is Action oldHandler)
            {
                changeFloatButton.clicked -= oldHandler;
            }

            Action handler = () =>
            {
                Debug.Log("Button clicked!");
                debugfloat += 2;
                var ent = entityManager.CreateEntity();
                entityManager.AddComponentData(ent, new ResourceGatherTimeFlag
                {
                    ID = combinedResourceData[index].ID,
                    GatheringTimeChangeValue = debugfloat,
                    ModifierID = "test"
                });
            };
            changeFloatButton.userData = handler;
            changeFloatButton.clicked += handler;
        };
    }
    private void HandleDataChange(string id)
    {
        List<CombinedResourceData> items = listView.itemsSource as List<CombinedResourceData>;
        if (items == null)
            return;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == id)
            {
                items[i].ResourceRuntimeData = uiBridge.GetData(id);
                listView.RefreshItems();
                break;
            }
        }
    }
    public void ShowPage()
    {
        page.style.display = DisplayStyle.Flex;
    }
    public void HidePage()
    {
        page.style.display = DisplayStyle.None;
    }
}
