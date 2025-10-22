using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourcePageController : IPageController
{
    private VisualElement page;
    private VisualTreeAsset resourcePanelAsset;
    private ResourceManagerSO data;
    //private ResourceUIBridgeSO uiBridge;
    private TreeView treeView;
    private ListView listView;

    private List<TreeViewItemData<string>> treeData;
    private Dictionary<string, List<ResourceSO>> listData;
    public void InitializePage(VisualElement page, ScriptableObject data)
    {
        this.page = page;
        this.data = data as ResourceManagerSO;
        //uiBridge = Resources.Load<ResourceUIBridgeSO>("UI/ResourceUIBridge");
        if (this.data == null)
            MainDebug.E0001WrongScriptableObjectCast(MainDebug.ErrorSeverity.Error, typeof(ResourceManagerSO).Name, data.GetType().Name);
        treeView = page.Q<TreeView>("treeView");
        listView = page.Q<ListView>("listView");
        resourcePanelAsset = Resources.Load<VisualTreeAsset>("UI/Panels/UIResourcePanel");

        InitializeData(this.data);
        InitializeTreeView();
        InitializeListView();
    }
    private void InitializeData(ResourceManagerSO data)
    {
        treeData = new List<TreeViewItemData<string>>();
        listData = new Dictionary<string, List<ResourceSO>>();
        int index = 0;

        foreach (var category in data.CategoriesList)
        {
            var treeNodes = new List<TreeViewItemData<string>>();
            foreach (var type in category.TypeList)
            {
                index++;
                treeNodes.Add(new TreeViewItemData<string>(index, type.ResourceTypeName));

                listData[type.ResourceTypeName] = type.ResourceList
                    .Where(r => r != null)
                    .ToList();
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
                UpdateListView(selectedItem);
            }
        };
        SetDefaultSelection();
    }
    private void InitializeListView()
    {
        listView.makeItem = () =>
        {
            TemplateContainer ve = resourcePanelAsset.CloneTree();
            return ve;
        };
        listView.bindItem = (element, index) =>
        {
            List<ResourceSO> resources = listView.itemsSource as List<ResourceSO>;
            ResourceSO resource = resources[index];

            //ResourceDynamicDataStruct dynamicData = uiBridge.ResourceDynamicData.Find(d => d.ID == resource.ID);

            Label nameLabel = element.Q<Label>("resource-name-label");
            //Label currentGatheringSpeed = element.Q<Label>("current-gathering-speed");

            nameLabel.text = resource.ResourceNameKey;
            //currentGatheringSpeed.text = dynamicData.ID != null ? "Скорость сборааа !!! = " + dynamicData.GatheringSpeed : "нет скорости сбора";
        };
    }
    private void UpdateListView(string typeName)
    {
        if (listData.TryGetValue(typeName, out var resources))
        {
            listView.itemsSource = resources;
            listView.Rebuild();
        }
    }
    private void SetDefaultSelection()
    {
        treeView.SetSelectionById(0);
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
