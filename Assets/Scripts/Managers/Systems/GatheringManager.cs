using Resources;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class GatheringManager : MonoBehaviour
{
    public ProgressBar gatheringProgressBar;

    [SerializeField] UIDocument MainUIDocument;
    private VisualElement VERoot;
    private int val = 0;
    private enum woodGatherState
    {
        GETHERING,
        IDLE
    }
    private woodGatherState currentState = woodGatherState.IDLE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VERoot = MainUIDocument.rootVisualElement;
        var startGatheringButton = VERoot.Q<Button>("getWoodBtn");
        var progressBar = VERoot.Q<ProgressBar>("woodPB");
        var textField = VERoot.Q<TextField>("woodTextAmount");

        startGatheringButton.RegisterCallback<ClickEvent>(evt => OnStartGatheringGuttonClicked(evt, progressBar, textField));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnStartGatheringGuttonClicked(ClickEvent evt, ProgressBar psb, TextField txt)
    {
        if (currentState == woodGatherState.IDLE)
        {
            Debug.Log("Start Gathering");
            currentState = woodGatherState.GETHERING;
            UpdateProgressBar(psb, txt);
        }
        else
        {
            currentState = woodGatherState.IDLE;
            Debug.Log("Stop Gathering");
        }
    }

    private void UpdateProgressBar(ProgressBar psb, TextField txt)
    {
        psb.schedule.Execute(() =>
        {
            if (psb.value <= 100f)
                psb.value += 2f;
            else
            {
                psb.value = 0f;
                UpdateWood(txt);
            }
        }).Every(75).Until(() => currentState == woodGatherState.IDLE);//.Until(() => psb.value >= 100f).;
        Debug.Log("ended");
    }
    private void UpdateWood(TextField txt)
    {
        val++;
        txt.value = val.ToString();
    }
}
