using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class ScreenTransitioner : MonoBehaviour
    {
        public enum TransitionType
        {
            FadeIn,
            FadeOut,
            DelayIn,
            DelayOut,
            None
        }

        public Image TransitionImage;
        [CurveRange(0, 0, 1, 1)] public AnimationCurve TransitionCurveIn;
        [CurveRange(0, 0, 1, 1)]public AnimationCurve TransitionCurveOut;
        public float FadeInTime, FadeOutTime;
        public float FadeTime;
        public TransitionType Transition = TransitionType.FadeOut;
        public float FadeDelayIn, FadeDelayOut;


        private void Update()
        {
            switch (Transition)
            {
                case TransitionType.FadeIn:
                    FadeTime += Time.deltaTime;
                    float timeIn = FadeTime / FadeInTime;
                    float alphaIn = TransitionCurveIn.Evaluate(timeIn);
                    TransitionImage.color = new Color(1, 1, 1, alphaIn);
                    if (FadeTime >= FadeInTime)
                    {
                        Transition = TransitionType.None;
                        FadeTime = 0f;
                    }
                    break;
                case TransitionType.FadeOut:
                    FadeTime += Time.deltaTime;
                    float timeOut = FadeTime / FadeOutTime;
                    float alphaOut = TransitionCurveOut.Evaluate(timeOut);
                    TransitionImage.color = new Color(1, 1, 1, alphaOut);
                    if (FadeTime >= FadeOutTime)
                    {
                        Transition = TransitionType.None;
                        FadeTime = 0f;
                    }
                    break;
                case TransitionType.None:
                    break;
            }
        }

        [Button]
        public void FadeIn()
        {
            Transition = TransitionType.FadeIn;
            FadeTime = 0f;
        }

        [Button]
        public void FadeOut()
        {
            Transition = TransitionType.FadeOut;
            FadeTime = 0f;
        }

    }
}