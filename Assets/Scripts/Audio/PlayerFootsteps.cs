using UnityEngine;
using PlayerControls;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    public float stepInterval = 0.4f;

    private AudioSource audioSource;
    private float timer;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.volume = 0.7f;
    }

    void Update()
    {
        if (PlayerController.Instance == null) return;
        if (PlayerController.Instance.MoveDirection == 0f)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= stepInterval)
        {
            timer = 0f;
            PlayRandom();
        }
    }

    void PlayRandom()
    {
        if (footstepSounds.Length == 0) return;
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.PlayOneShot(clip);
    }
}