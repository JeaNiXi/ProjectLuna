using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingsPageController : IPageController
{
    private VisualElement page;
    private BuildingManagerSO data;
    public void InitializePage(VisualElement page, ScriptableObject data)
    {
        this.page = page;
        this.data = data as BuildingManagerSO;
        if (this.data == null)
            throw new ArgumentException($"Wrong Scriptable Object! Expected: {typeof(BuildingManagerSO).Name}, but got {data.GetType().Name}");
    }
    public void ShowPage()
    {
        page.style.display = DisplayStyle.Flex;
    }
    public void HidePage()
    {
        page.style.display = DisplayStyle.None;
    }
    public void UpdateUI()
    {

    }
}
