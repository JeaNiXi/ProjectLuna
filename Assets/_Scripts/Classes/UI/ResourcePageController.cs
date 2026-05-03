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
    public ResourceRuntimeAmountData ResourceRuntimeAmountData;
    public bool isUpgradable;

    public CombinedResourceData(FixedString128Bytes id, ResourceSO so, ResourceRuntimeData data, ResourceRuntimeAmountData amountData, bool isUpgradable)
    {
        ID = id;
        ResourceSO = so;
        ResourceRuntimeData = data;
        ResourceRuntimeAmountData = amountData;
        this.isUpgradable = isUpgradable;
    }
}
public class ResourcePageController : IPageController
{
    private VisualElement page;
    private VisualTreeAsset resourcePanelAsset;
    private ResourceManagerSO data;
    private ResourceRuntimeBridgeSO uiBridge;
    private ResourceRuntimeAmountSO uiAmountBridge;
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
        uiAmountBridge = Resources.Load<ResourceRuntimeAmountSO>("Resource/ResourceRuntimeAmount");
        if (uiAmountBridge != null)
            Debug.Log("UI Amount Bridge Loaded Successfuly! ");

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
                combinedResourceDataList.Add(new CombinedResourceData(res.ID, res, uiBridge.GetData(res.ID), uiAmountBridge.GetData(res.ID), CanBeUpdated(res.ID.ToString())));
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

            Label resourceLevelLabel = element.Q<Label>("resource-level-label");
            Label nameLabel = element.Q<Label>("resource-name-label");
            Label descriptionLabel = element.Q<Label>("description-label");
            Label currentGatheringTime = element.Q<Label>("current-gathering-time");
            Label currentGatheringAmount = element.Q<Label>("current-gathering-amount");
            Label resourceAmountLabel = element.Q<Label>("resource-amount-label");
            Label resourceUpgradeCostLabel = element.Q<Label>("resource-upgrade-cost");
            Button startGatheringButton = element.Q<Button>("start-gathering-button");
            Button upgradeResourceMainBuilding = element.Q<Button>("upgrade-resource-main-building");

            resourceLevelLabel.text = combinedResourceData[index].ResourceRuntimeData.ResourceLevel.ToString();
            nameLabel.text = combinedResourceData[index].ResourceSO.ResourceNameKey;
            descriptionLabel.text = combinedResourceData[index].ResourceSO.Description;
            currentGatheringTime.text = combinedResourceData[index].ResourceRuntimeData.GatheringTime.ToString();
            currentGatheringAmount.text = combinedResourceData[index].ResourceRuntimeData.GatheringAmount.ToString();
            resourceAmountLabel.text = combinedResourceData[index].ResourceRuntimeAmountData.Amount.ToString();
            resourceUpgradeCostLabel.text = combinedResourceData[index].ResourceRuntimeData.UpgradeCost.ToString();

            if (startGatheringButton.userData is Action oldHandlerStartGathering)
            {
                startGatheringButton.clicked -= oldHandlerStartGathering;
            }
            if (upgradeResourceMainBuilding.userData is Action oldHandlerUpgradeBuilding)
            {
                upgradeResourceMainBuilding.clicked -= oldHandlerUpgradeBuilding;
            }
            Action startGatheringHandler = () =>
            {
                startGatheringButton.text = "working!";
                startGatheringButton.SetEnabled(false);
                Debug.Log("startGatheringButton Clicked!");
                var ent = entityManager.CreateEntity();
                entityManager.AddComponentData(ent, new ResourceIsGatheringFlag
                {
                    ID = combinedResourceData[index].ID,
                    Level = combinedResourceData[index].ResourceRuntimeData.ResourceLevel,
                    BaseAmount = combinedResourceData[index].ResourceSO.BaseGatheringAmount,
                    ProductionMultiplayer = combinedResourceData[index].ResourceSO.GatherAmountMultiplayerPerUpgrade,
                });
            };
            Action upgradeBuildingHandler = () =>
            {
                Debug.Log("upgrade-resource-main-building Button Clicked!");
                var ent = entityManager.CreateEntity();
                entityManager.AddComponentData(ent, new ResourceGatherAmountFlag
                {
                    ID = combinedResourceData[index].ID,
                    ResourceLevel = combinedResourceData[index].ResourceRuntimeData.ResourceLevel,
                    BaseGatheringAmount = combinedResourceData[index].ResourceSO.BaseGatheringAmount,
                    GatheringAmountMultiplayer = combinedResourceData[index].ResourceSO.GatherAmountMultiplayerPerUpgrade,
                    BaseUpgradeCost = combinedResourceData[index].ResourceSO.BaseUpgradeCost,
                    UpgradeCostMultiplayer = combinedResourceData[index].ResourceSO.UpgradeCostMultiplayer,
                });
                    //public FixedString128Bytes ID;
                    //public int ResourceLevel;
                    //public float BaseGatheringAmount;
                    //public float GatheringAmountMultiplayer;
                    //public float BaseUpgradeCost;
                    //public float UpgradeCostMultiplayer;
    //var testEnt = entityManager.CreateEntity();  //Пока не трогаем.
    //entityManager.AddComponentData(testEnt, new ResourceGatherTimeFlag
    //{
    //    ID = combinedResourceData[index].ID,
    //    CurrentGatheringTime = combinedResourceData[index].ResourceRuntimeData.GatheringTime,
    //    GatheringTimeMultiplayer = combinedResourceData[index].ResourceSO.GatherTimeMultiplayerPerUpgrade,
    //});
};
            startGatheringButton.userData = startGatheringHandler;
            startGatheringButton.clicked += startGatheringHandler;
            upgradeResourceMainBuilding.userData = upgradeBuildingHandler;
            upgradeResourceMainBuilding.clicked += upgradeBuildingHandler;
            upgradeResourceMainBuilding.SetEnabled(combinedResourceData[index].isUpgradable);
        };
        Debug.Log("Updated List View");
        ListViewHasBeenUpdated = true;
    }
    public void UpdateUI()
    {
        currentTime += Time.deltaTime;
        bool hasChanges = false;
        if (ListViewHasBeenUpdated && currentTime > updateTimer)
        {
            List<CombinedResourceData> items = currentCombinedResourceDataList;
            for (int i = 0; i < items.Count; i++)
            {
                if (uiBridge.DynamicData.TryGetValue(items[i].ID.ToString(), out var newData))// && newData.IsUpdated)
                {
                    CombinedResourceData tmpData = new CombinedResourceData(
                        uiBridge.DynamicData[items[i].ID.ToString()].ID,
                        items[i].ResourceSO,
                        uiBridge.DynamicData[items[i].ID.ToString()],
                        uiAmountBridge.AmountData[items[i].ID.ToString()],
                        CanBeUpdated(items[i].ID.ToString()));
                    items[i] = tmpData;
                    uiBridge.SetStatusUpdateFalse(items[i].ID);
                    //Debug.Log($"Can be updated = {CanBeUpdated(items[i].ID.ToString())}");
                    if (!hasChanges)
                        hasChanges = true;
                }
            }
            if (hasChanges)
            {
                listView.itemsSource = items;
                currentCombinedResourceDataList = listView.itemsSource as List<CombinedResourceData>;
            }
            listView.RefreshItems();
            //Debug.Log("Updated UI");
            currentTime = 0;
        }
    }
    private bool CanBeUpdated(string id)
    {
        if (uiAmountBridge.AmountData.TryGetValue(id, out var newData))
        {
            //Debug.Log($"New Date = {newData.Amount} and cost = {uiBridge.GetData(id.ToString()).UpgradeCost}");
            if (newData.Amount >= uiBridge.GetData(id.ToString()).UpgradeCost)
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
