using System;
using System.Collections.Generic;
using EchoMina.Original;
using MainMenu;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable InconsistentNaming

namespace Utility
{
    [RequireComponent(typeof(BoxCollider))]
    public class Stairs : MonoBehaviour
    {
        [Tag] public List<string> Whitelist;
        public Transform TeleportPoint;
        public UnityEvent OnTeleport;
        [Required("You need to hook up a screen transitioner here")]
        public ScreenTransitioner Transitioner;
        private string interactorName;

        BoxCollider boxCollider;

        private void OnValidate()
        {
            if (boxCollider != null) return;

            boxCollider = GetComponent<BoxCollider>();
            {
                boxCollider.isTrigger = true;
            }
        }

        private void Start()
        {
            if (Transitioner != null)
            {
                Transitioner.EndFadeIn += TeleportObject;
            }
        }

        private void OnDestroy()
        {
            if (Transitioner != null)
            {
                Transitioner.EndFadeIn -= TeleportObject;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Whitelist.Contains(other.tag))
            {
                Debug.Log(other.transform.name + " has entered the stairs");

                if (other.tag == "Player")
                {
                    interactorName = "Player";
                }
                else if (other.tag == "EchoMina")
                {
                    interactorName = "EchoMina";
                }
                
                if (Transitioner != null)
                {
                    Transitioner.DoFadeIn();
                }
            }
        }

        private void TeleportObject()
        {
            OnTeleport?.Invoke();
            
            if (interactorName == "Player")
            {
                PlayerController.Instance.transform.position = TeleportPoint.position;
            }
            else if (interactorName == "EchoMina")
            {
                OriginalEchoMina.Instance.transform.position = TeleportPoint.position;
            }
            
            if (Transitioner != null)
            {
                Transitioner.DoFadeOut();
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