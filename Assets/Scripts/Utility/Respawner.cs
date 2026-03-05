using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Utility
{
    public class Respawner : MonoBehaviour
    {
        public Transform RespawnPoint;
        [Tag] public List<string> Whitelist = new List<string>();

        private void OnTriggerEnter(Collider other)
        {
            if (Whitelist.Contains(other.tag))
            {
                CharacterController characterController = gameObject.GetComponent<CharacterController>();
                if (characterController)
                {
                    characterController.enabled = false;
                    transform.position = RespawnPoint.position;
                    transform.rotation = RespawnPoint.rotation;
                    characterController.enabled = true;
                }
                else
                {
                    transform.position = RespawnPoint.position;
                    transform.rotation = RespawnPoint.rotation;
                }
            }
        }
    }
}
