using System;
using NaughtyAttributes;
using UnityEngine;

namespace QuickLoad
{
    [RequireComponent(typeof(BoxCollider))]
    public class CheckpointSetter : MonoBehaviour
    {
        [SerializeField] private int loadIndex = -1;
        [Tag] [SerializeField] private string allowedTag = "Player";
        private BoxCollider boxCollider;

        private void OnValidate()
        {
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider>();
                boxCollider.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(allowedTag))
            {
                QuickLoader.Instance.SetCheckpoint(loadIndex);
            }
        }
    }
}