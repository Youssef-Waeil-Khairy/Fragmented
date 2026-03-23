using System;
using System.Collections.Generic;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;
using UnityEngine.Events;

namespace Utility
{
    [RequireComponent(typeof(BoxCollider))]
    public class Stairs : MonoBehaviour
    {
        public Transform TeleportPoint;
        [Tag] public List<string> Whitelist;
        public UnityEvent OnTeleport;

        BoxCollider boxCollider;

        private void OnValidate()
        {
            if (boxCollider != null) return;

            boxCollider = GetComponent<BoxCollider>();
            {
                boxCollider.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Whitelist.Contains(other.tag))
            {
                other.transform.position = TeleportPoint.position;
                OnTeleport?.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            if (TeleportPoint)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(TeleportPoint.position, .2f);
            }
        }
    }
}