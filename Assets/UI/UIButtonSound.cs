using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private AudioSource audioSource;
    private Animator animator;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.Play("Highlighted");
        audioSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.Play("Normal");
    }
}