using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;
using static UnityEditor.VersionControl.Message;

public static class MainDebug
{
    public enum ErrorSeverity
    {
        Info,
        Warning,
        Error,
        Critical,
    }
    public enum ErrorType
    {
        E0001WrongScriptableObjectCast,
        E0002DataNotFoundInUIBridge,
        E000XSomeOtherError
    }
    private static readonly string CriticalMessage = "CRITICAL ERROR! ";

    public static void E0001WrongScriptableObjectCast(ErrorSeverity severity, string SONeeded, string SOGot)
    {
        var message = $"Wrong Scriptable Object! Expected: {SONeeded}, but got {SOGot}";
        CreateMessage(severity, message);
    }
    public static void E0002DataNotFoundInUIBridge(ErrorSeverity severity, string idSearched)
    {
        var message = $"Data not found in UI Bridge! Expected: {idSearched}";
        CreateMessage(severity, message);
    }
    public static void E0002SomeOtherError(params string[] args)
    {

    }

    private static void CreateMessage(ErrorSeverity severity, string message)
    {
        switch (severity)
        {
            case ErrorSeverity.Info:
                Debug.Log(message);
                break;
            case ErrorSeverity.Warning:
                Debug.LogWarning(message);
                break;
            case ErrorSeverity.Error:
                Debug.LogError(message);
                break;
            case ErrorSeverity.Critical:
                Debug.LogError($"{CriticalMessage}{message}");
                break;
            default:
                Debug.Log("Default Error Message");
                break;
        }
    }
}
