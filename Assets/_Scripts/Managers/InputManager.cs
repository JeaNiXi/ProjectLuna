using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public InputActionReference PauseAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }

        EnableAllActions();
    }
    #region Enable InputActions
    private void EnableAllActions()
    {
        PauseAction.action.performed += OnPauseKeyPressed;
        PauseAction.action.Enable();
    }
    #endregion
    #region InputMethods
    private void OnPauseKeyPressed(InputAction.CallbackContext ctx)
    {
        InputBridge.Instance.ToggleTimeState();
    }
    #endregion
}
