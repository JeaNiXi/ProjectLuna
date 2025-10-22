using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIResourcePanel : VisualElement
{
    public Label Label;
    public Button Button;
    public ProgressBar ProgressBar;
    public VisualElement ImageContainer;

    public UIResourcePanel()
    {
        var RPTemplate = Resources.Load<VisualTreeAsset>("UI/ResourcePanel");
        if (RPTemplate != null)
        {
            RPTemplate.CloneTree(this);
            ImageContainer = this.Q<VisualElement>("ImageContainer");
            Label = this.Q<Label>("ResourceName");
            Button = this.Q<Button>("CollectionButton");
            ProgressBar = this.Q<ProgressBar>("ProgressBar");

            AddToClassList("resource-panel");

            Button.clicked += OnButtonClicked;
        }
        else
        {
            Debug.Log("Resource Template 404. HELP Feliciac!!!");
        }
    }

    public void OnButtonClicked()
    {
        Debug.Log("Button clicked for " +  Label.text);
    }
}
