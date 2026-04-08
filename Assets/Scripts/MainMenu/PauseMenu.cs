using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    [Header("Panel")]
    public RectTransform pausePanel;

    [Header("Buttons")]
    public Button resumeButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Settings Panel")]
    public GameObject seetingsPanel;

    [Header("Animation")]
    public float slideSpeed = 0.5f;
    public string mainMenuSceneName = "MainMenu";

    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isPaused = false;
    private bool isAnimating = false;

    void Awake()
    {
      if (instance != null && instance != this)
      {
            Destroy(gameObject);
            return;
      }

      instance = this;
        DontDestroyOnLoad(gameObject);
    }




    void Start()
    {
        hiddenPosition = new Vector2(-pausePanel.rect.width - 100, 0);
        shownPosition = new Vector2(0, 0);

        pausePanel.anchoredPosition = hiddenPosition;
        pausePanel.gameObject.SetActive(false);

        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(Quit);

    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == mainMenuSceneName) 
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isAnimating) 
        {
            if (seetingsPanel != null && seetingsPanel.activeSelf) 
            {
                seetingsPanel.SetActive(false);
                return;
            }

            if (isPaused) 
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.gameObject.SetActive(true);

        StartCoroutine(SlidePanel(hiddenPosition, shownPosition));

        Debug.Log("game paused");
        
    }

    void Resume()
    {
        if (isAnimating) return;

        StartCoroutine(SlideOutandResume());

        Debug.Log("game resumed");
    }

    void OpenSettings()
    {
        if (seetingsPanel == null)
        {
            Debug.LogError("seetings pannel not assigned in inspector");
            return;
        }

        seetingsPanel.SetActive(true);
        Debug.Log("settings opend");
    }

    void Quit()
    {
        Debug.Log("quiting");

        Time.timeScale = 1f;

        isPaused = false;

        SceneManager.LoadScene("MainMenu");
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

    IEnumerator SlideOutandResume() 
    {
        yield return StartCoroutine(SlidePanel(shownPosition, hiddenPosition));
        pausePanel.gameObject.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }


}
