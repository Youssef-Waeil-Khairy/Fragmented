using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("Panel")]
    public RectTransform pausePanel;

    [Header("Buttons")]
    public Button resumeButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";
    public string settingsSceneName = "Settings";

    [Header("Animation")]
    public float slideSpeed = 0.5f;

    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isPaused = false;
    private bool isAnimating = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

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
        SceneManager.LoadScene(settingsSceneName);
        Debug.Log("Opening Settings");
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