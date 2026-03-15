using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class ClickEffect : MonoBehaviour
{
    public GameObject clickEffectPrefab;
    public Canvas canvas;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            SpawnEffect(mousePos);
        }
    }

    void SpawnEffect(Vector2 mousePos)
    {
        GameObject effect = Instantiate(clickEffectPrefab, canvas.transform);
        RectTransform rt = effect.GetComponent<RectTransform>();

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            mousePos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out pos
        );

        rt.localPosition = pos;
        StartCoroutine(AnimateEffect(effect));
    }

    IEnumerator AnimateEffect(GameObject effect)
    {
        float duration = 0.4f;
        float elapsed = 0f;
        Image img = effect.GetComponent<Image>();
        RectTransform rt = effect.GetComponent<RectTransform>();
        Vector3 startScale = rt.localScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rt.localScale = Vector3.Lerp(startScale, startScale * 1.5f, t);
            Color c = img.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            img.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(effect);
    }
}