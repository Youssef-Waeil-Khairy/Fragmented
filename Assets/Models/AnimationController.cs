using UnityEngine;
using PlayerControls;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (PlayerController.Instance == null || animator == null) return;

        bool isWalking = PlayerController.Instance.MoveDirection != 0f;
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("WalkingSpeed", PlayerController.Instance.MoveDirection);
    }
}