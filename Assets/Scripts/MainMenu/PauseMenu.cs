using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using NaughtyAttributes;
using PlayerControls;
using RoomControls;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Foldout("Panel")] public RectTransform pausePanel;

    [Foldout("Buttons")] public Button resumeButton;
    [Foldout("Buttons")] public Button settingsButton;
    [Foldout("Buttons")] public Button quitButton;

    public LevelSwitcher SettingsSwitcher;
    [Foldout("Scene Names")] public string mainMenuSceneName = "MainMenu";
    [Foldout("Scene Names")] public string settingsSceneName = "Settings";

    [Foldout("Animation")]
    public float slideSpeed = 0.5f;

    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isPaused = false;
    private bool isAnimating = false;

    [Foldout("Snapshots")] public bool ShouldSnapshot;
    [Foldout("Snapshots")] public bool ShouldLoadSnapShot;
    [Foldout("Snapshots")][Scene] public int CurrentScene;
    [Foldout("Snapshots")] public Vector3 CurrentPosition;
    [Foldout("Snapshots")] public Quaternion CurrentRotation;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;

        if (pausePanel != null)
        {
            Canvas parentCanvas = pausePanel.GetComponentInParent<Canvas>();
            if (parentCanvas != null) DontDestroyOnLoad(parentCanvas.gameObject);

            hiddenPosition = new Vector2(-pausePanel.rect.width - 100, 0);
            shownPosition = new Vector2(0, 0);
            pausePanel.anchoredPosition = hiddenPosition;
            pausePanel.gameObject.SetActive(false);
        }
    }
    private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (ShouldLoadSnapShot)
        {
            LoadSnapshot();
        }
    }

    void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(Quit);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == mainMenuSceneName) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame && !isAnimating)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    void Pause()
    {
        if (pausePanel == null)
        {
            Debug.LogError("PausePanel is null! Canvas was destroyed on scene change.");
            return;
        }

        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.gameObject.SetActive(true);
        StartCoroutine(SlidePanel(hiddenPosition, shownPosition));
        Debug.Log("Game Paused");
    }

    public void Resume()
    {
        if (isAnimating) return;
        StartCoroutine(SlideOutAndResume());
        Debug.Log("Game Resumed");
    }

    void OpenSettings()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SettingsSwitcher.GoToSettingsScene();

        Debug.Log("Opening Settings");
    }

    public bool IsMainMenu()
    {
        return SceneManager.GetActiveScene().name == mainMenuSceneName;
    }
    public bool IsSettingsScene()
    {
        return SceneManager.GetActiveScene().name == settingsSceneName;
    }

    public void TakeSnapshot()
    {
        if (IsMainMenu())
        {
            return;
        }

        CurrentPosition = PlayerController.Instance.transform.position;
        CurrentRotation = PlayerController.Instance.transform.rotation;
    }

    public void LoadSnapshot()
    {
        if (PlayerController.Instance == null)
        {
            return;
        }

        PlayerController.Instance.transform.position = CurrentPosition;
        PlayerController.Instance.transform.rotation = CurrentRotation;
    }

    void Quit()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
        Debug.Log("Quitting to Main Menu");
    }

    IEnumerator SlidePanel(Vector2 from, Vector2 to)
    {
        isAnimating = true;
        float elapsed = 0f;
        while (elapsed < slideSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideSpeed);
            pausePanel.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }
        pausePanel.anchoredPosition = to;
        isAnimating = false;
    }

    IEnumerator SlideOutAndResume()
    {
        yield return StartCoroutine(SlidePanel(shownPosition, hiddenPosition));
        pausePanel.gameObject.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }
}