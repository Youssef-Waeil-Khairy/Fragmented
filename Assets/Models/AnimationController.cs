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

        float move = PlayerController.Instance.MoveDirection;
        float turn = PlayerController.Instance.TurnDirection;

        animator.SetBool("isWalking", move != 0f);
        animator.SetBool("isTurningLeft", turn != 0f);
        animator.SetBool("isTurningRight", turn != 0f);


    }
}