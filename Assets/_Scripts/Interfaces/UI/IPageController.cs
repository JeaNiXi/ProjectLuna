using UnityEngine;
using UnityEngine.UIElements;

public interface IPageController
{
    void InitializePage(VisualElement page, ScriptableObject data);
    void ShowPage();
    void HidePage();
    void UpdateUI();
}
