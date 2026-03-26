using System;
using System.Collections.Generic;
using EchoMina.Original;
using NaughtyAttributes;
using QuickLoad;
using UnityEngine;

namespace Utility
{
    public class Respawner : MonoBehaviour
    {
        [Tag] public List<string> Whitelist = new List<string>();
        [SerializeField] private bool IsOnMina;

        private void OnTriggerEnter(Collider other)
        {
            if (Whitelist.Contains(other.tag))
            {
                if (IsOnMina)
                {
                    QuickLoader.Instance.LoadCheckpoint();
                }
                else
                {
                    if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
                    {
                        OriginalEchoMina.Instance.Despawn();
                    }
                    else if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Playing)
                    {
                        OriginalEchoMina.Instance.TogglePlaying(0);
                    }
                }
            }
        }
    }
}