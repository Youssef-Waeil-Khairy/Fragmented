using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayFromMainMenu
{
    private const string MAIN_MENU_PATH = "Assets/Scenes/MainMenu.unity";

    private static string previousScenePath;
    private static bool shouldReturnToPreviousScene;

    static PlayFromMainMenu()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    // =========================
    // ALT + P HOTKEY
    // =========================
    [MenuItem("Tools/Play From MainMenu &p")]
    public static void PlayFromHotkey()
    {
        StartPlayFromMainMenu();
    }

    // =========================
    // SHIFT + PLAY BUTTON
    // =========================
    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (!IsShiftHeld())
                return;

            StartPlayFromMainMenu();
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            ReturnToPreviousSceneIfNeeded();
        }
    }

    private static void StartPlayFromMainMenu()
    {
        if (EditorApplication.isPlaying)
            return;

        string currentScene = SceneManager.GetActiveScene().path;

        if (currentScene == MAIN_MENU_PATH)
        {
            EditorApplication.isPlaying = true;
            return;
        }

        previousScenePath = currentScene;
        shouldReturnToPreviousScene = true;

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            shouldReturnToPreviousScene = false;
            return;
        }

        EditorSceneManager.OpenScene(MAIN_MENU_PATH);

        EditorApplication.isPlaying = true;
    }

    private static void ReturnToPreviousSceneIfNeeded()
    {
        if (!shouldReturnToPreviousScene)
            return;

        shouldReturnToPreviousScene = false;

        if (!string.IsNullOrEmpty(previousScenePath))
        {
            EditorSceneManager.OpenScene(previousScenePath);
        }
    }

    private static bool IsShiftHeld()
    {
        return Input.GetKey(KeyCode.LeftShift) ||
               Input.GetKey(KeyCode.RightShift);
    }
}