using System;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable InconsistentNaming

namespace MainMenu
{
    public class ScreenTransitioner : MonoBehaviour
    {
        /// <summary>
        /// What type of transition to do
        /// </summary>
        public enum TransitionType
        {
            FadeIn, // Full opacity => full transparency
            FadeOut, // Full transparency => full opacity
            DelayIn, // Wait for the delay time, then full opacity => full transparency
            DelayOut, // Wait for the delay time, then full transparency => full opacity
            None
        }

        #region Events

        public delegate void OnBeginDelayIn();
        public delegate void OnEndDelayIn();
        public delegate void OnBeginFadeIn();
        public delegate void OnEndFadeIn();
        public delegate void OnBeginDelayOut();
        public delegate void OnEndDelayOut();
        public delegate void OnBeginFadeOut();
        public delegate void OnEndFadeOut();
        public event OnBeginDelayIn BeginDelayIn;
        public event OnEndDelayIn EndDelayIn;
        public event OnBeginFadeIn BeginFadeIn;
        public event OnEndFadeIn EndFadeIn;
        public event OnBeginDelayOut BeginDelayOut;
        public event OnEndDelayOut EndDelayOut;
        public event OnBeginFadeOut BeginFadeOut;
        public event OnEndFadeOut EndFadeOut;

  #endregion

        public static ScreenTransitioner Instance { get; private set; }

        [SerializeField][ReadOnly] private bool isTransitioning = false;
        public float FadeTime;
        [SerializeField][ReadOnly] private float delayTime = 0f;
        public float FadeInTime, FadeOutTime, FadeDelayIn, FadeDelayOut;
        public Image TransitionImage;
        [CurveRange(0, 0, 1, 1)] public AnimationCurve TransitionCurveIn;
        [CurveRange(0, 0, 1, 1)] public AnimationCurve TransitionCurveOut;
        public TransitionType Transition = TransitionType.FadeOut;

        [SerializeField] private float debugTime;
        [SerializeField] private bool debugBool;
        [Button("Fade In With Delay")]
        private void DebugFadeIn()
        {
            DoFadeIn(debugBool, debugTime);
        }
        [Button("Fade Out With Delay")]
        private void DebugFadeOut()
        {
            DoFadeOut(debugBool, debugTime);
        }

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);

            Transition = TransitionType.None;
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }
        private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            DoFadeOut(false, 1f);
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
        }

        private void Start()
        {
            if (Transition is not TransitionType.None) isTransitioning = true;
        }

        private void Update()
        {
            if (!isTransitioning) return;

            switch (Transition)
            {
                case TransitionType.FadeIn:
                    FadeIn(Time.deltaTime);
                    break;
                case TransitionType.FadeOut:
                    FadeOut(Time.deltaTime);
                    break;
                case TransitionType.DelayIn:
                    delayTime += Time.deltaTime;

                    if (delayTime >= FadeDelayIn)
                    {
                        EndDelayIn?.Invoke();
                        DoFadeIn();
                    }
                    break;
                case TransitionType.DelayOut:
                    delayTime += Time.deltaTime;

                    if (delayTime >= FadeDelayOut)
                    {
                        EndDelayOut?.Invoke();
                        DoFadeOut();
                    }
                    break;
                case TransitionType.None:
                    break;
            }
        }

        /// <summary>
        /// Starts fading the transition screen to full opacity without delay
        /// </summary>
        [Button]
        public void DoFadeIn()
        {
#if DEBUG
            Debug.Log("Begin Fade In");
#endif
            Transition = TransitionType.FadeIn;
            FadeTime = 0f;

            isTransitioning = true;

            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.ToggleInputEnabled(false);
            }

            BeginFadeIn?.Invoke();
        }
        /// <summary>
        /// Starts fading in the transition screen with a delay
        /// </summary>
        /// <param name="defaultDelay">Whether to use the default delay or the provided delay</param>
        /// <param name="delay">The time to wait before fading in</param>
        public void DoFadeIn(bool defaultDelay, float delay)
        {
#if DEBUG
            Debug.Log("Begin Fade In Delayed");
#endif
            Transition = TransitionType.DelayIn;
            FadeTime = 0f;
            delayTime = 0f;
            if (!defaultDelay) FadeDelayIn = delay;

            isTransitioning = true;

            BeginDelayIn?.Invoke();
        }

        /// <summary>
        /// Begins fading the transition screen out to full transparency
        /// </summary>
        [Button]
        public void DoFadeOut()
        {
#if DEBUG
            Debug.Log("Begin Fade Out");
#endif
            Transition = TransitionType.FadeOut;
            FadeTime = 0f;

            isTransitioning = true;

            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.ToggleInputEnabled(false);
            }

            BeginFadeOut?.Invoke();
        }

        /// <summary>
        /// Begins fading out the transition screen to full transparency after a delay
        /// </summary>
        /// <param name="defaultDelay">Whether to use the default delay or given delay</param>
        /// <param name="delay">How long to wait before starting the fade out process</param>
        public void DoFadeOut(bool defaultDelay, float delay)
        {
#if DEBUG
            Debug.Log("Begin Fade Out Delayed");
#endif
            Transition = TransitionType.DelayOut;
            FadeTime = 0f;
            delayTime = 0f;
            if (!defaultDelay) FadeDelayOut = delay;

            isTransitioning = true;

            BeginDelayOut?.Invoke();
        }

        /// <summary>
        /// Lerps the transition image's alpha from 0 => 1
        /// </summary>
        /// <param name="dt">delta time</param>
        private void FadeIn(float dt)
        {
            FadeTime += dt;
            float timeIn = FadeTime / FadeInTime;
            float alphaIn = TransitionCurveIn.Evaluate(timeIn);
            TransitionImage.color = new Color(1, 1, 1, alphaIn);
            if (FadeTime >= FadeInTime)
            {
                Transition = TransitionType.None;
                FadeTime = 0f;
                isTransitioning = false;

                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.ToggleInputEnabled(true);
                }
                Debug.Log("Fade In Finished");
                EndFadeIn?.Invoke();
            }
        }

        /// <summary>
        /// Lerps the transition image's alpha from 1 => 0
        /// </summary>
        /// <param name="dt">delta time</param>
        private void FadeOut(float dt)
        {
            FadeTime += dt;
            float timeOut = FadeTime / FadeOutTime;
            float alphaOut = TransitionCurveOut.Evaluate(timeOut);
            TransitionImage.color = new Color(1, 1, 1, alphaOut);
            if (FadeTime >= FadeOutTime)
            {
                Transition = TransitionType.None;

                FadeTime = 0f;

                isTransitioning = false;

                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.ToggleInputEnabled(true);
                }
                Debug.Log("Fade Out Finished");
                EndFadeOut?.Invoke();
            }
        }
    }
}