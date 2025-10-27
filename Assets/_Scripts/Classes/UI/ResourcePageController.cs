using Mono.Cecil;
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
    public bool isUpgradable;

    public CombinedResourceData(FixedString128Bytes id, ResourceSO so, ResourceRuntimeData data, bool isUpgradable)
    {
        ID = id;
        ResourceSO = so;
        ResourceRuntimeData = data;
        this.isUpgradable = isUpgradable;
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
    private List<CombinedResourceData> currentCombinedResourceDataList;

    private Dictionary<string, List<ResourceSO>> listData;
    private Dictionary<ResourceSO, string> runtimeListDataIndex;

    private float currentTime = 0;
    private float updateTimer = 0.5f;

    private bool ListViewHasBeenUpdated;

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

        InitializeData(this.data);
        InitializeTreeView();
    }

    private void InitializeData(ResourceManagerSO data)
    {
        treeData = new List<TreeViewItemData<string>>();
        listData = new Dictionary<string, List<ResourceSO>>();
        runtimeListDataIndex = new Dictionary<ResourceSO, string>();
        currentCombinedResourceDataList = new List<CombinedResourceData>();
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
        Font font = Resources.Load<Font>("Fonts/Excalifont-Regular");
        treeView.makeItem = () => new Label()
        {
            //<ui:Button text="Улучшить" name="upgrade-resource-main-building" style="-unity-font-definition: resource(&apos;Fonts/Excalifont-Regular&apos;);" />
            style =
            {
                unityFontDefinition= new StyleFontDefinition(font)
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
                currentTime = 0;
                InitializeListView(selectedItem);
            }
        };
    }

    private void InitializeListView(string selectedType)
    {
        ListViewHasBeenUpdated = false;
        if (listData.TryGetValue(selectedType, out var resList))
        {
            List<CombinedResourceData> combinedResourceDataList = new List<CombinedResourceData>();
            foreach (var res in resList)
            {
                combinedResourceDataList.Add(new CombinedResourceData(res.ID, res, uiBridge.GetData(res.ID), CanBeUpdated(res.ID.ToString())));
            }
            listView.itemsSource = combinedResourceDataList;
            currentCombinedResourceDataList = combinedResourceDataList;
        }
        UpdateListView();
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
            Label descriptionLabel = element.Q<Label>("description-label");
            Label currentGatheringTime = element.Q<Label>("current-gathering-time");
            Label currentGatheringAmount = element.Q<Label>("current-gathering-amount");
            Button upgradeResourceMainBuilding = element.Q<Button>("upgrade-resource-main-building");


            nameLabel.text = combinedResourceData[index].ResourceSO.ResourceNameKey;
            descriptionLabel.text = combinedResourceData[index].ResourceSO.Description;
            currentGatheringTime.text = combinedResourceData[index].ResourceRuntimeData.GatheringTime.ToString();
            currentGatheringAmount.text = combinedResourceData[index].ResourceRuntimeData.GatheringAmount.ToString();

            if (upgradeResourceMainBuilding.userData is Action oldHandler)
            {
                upgradeResourceMainBuilding.clicked -= oldHandler;
            }

            Action handler = () =>
            {
                Debug.Log("upgrade-resource-main-building Button Clicked!");
                var ent = entityManager.CreateEntity();
                entityManager.AddComponentData(ent, new ResourceGatherAmountFlag
                {
                    ID = combinedResourceData[index].ID,
                    ResourceLevel = combinedResourceData[index].ResourceRuntimeData.ResourceLevel,
                    CurrentGatheringAmount = combinedResourceData[index].ResourceRuntimeData.GatheringAmount,
                    GatheringAmountMultiplayer = combinedResourceData[index].ResourceSO.GatherAmountMultiplayerPerUpgrade,
                });
            };
            upgradeResourceMainBuilding.userData = handler;
            upgradeResourceMainBuilding.clicked += handler;
            upgradeResourceMainBuilding.SetEnabled(combinedResourceData[index].isUpgradable);
        };
        Debug.Log("Updated List View");
        ListViewHasBeenUpdated = true;
    }
    public void UpdateUI()
    {
        currentTime += Time.deltaTime;
        if (ListViewHasBeenUpdated && currentTime > updateTimer)
        {
            Debug.Log("Updating UI");
            List<CombinedResourceData> items = currentCombinedResourceDataList;
            for (int i = 0; i < items.Count; i++)
            {
                if (uiBridge.DynamicDataStruct.TryGetValue(items[i].ID.ToString(), out var newData) && newData.IsUpdated)
                {
                    CombinedResourceData tmpData = new CombinedResourceData(
                        uiBridge.DynamicDataStruct[items[i].ID.ToString()].ID,
                        items[i].ResourceSO,
                        uiBridge.DynamicDataStruct[items[i].ID.ToString()],
                        CanBeUpdated(items[i].ID.ToString()));
                    items[i] = tmpData;
                    uiBridge.SetStatusUpdateFalse(items[i].ID);
                }
            }
            currentCombinedResourceDataList = listView.itemsSource as List<CombinedResourceData>;
            listView.RefreshItems();
            currentTime = 0;
        }
    }
    private bool CanBeUpdated(string id) // Need to add level to check.
    {
        if (uiBridge.DynamicDataStruct.TryGetValue(id, out var newData))
        {
            if (newData.GatheringAmount >= 30f)
                return true;
            else
                return false;
        }
        else
            return false;
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
