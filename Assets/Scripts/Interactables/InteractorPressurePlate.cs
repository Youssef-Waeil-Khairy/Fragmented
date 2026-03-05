using System.Collections.Generic;
using EchoMina.Original;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Utility;
// ReSharper disable InconsistentNaming

namespace Interactables
{
    public class InteractorPressurePlate : MonoBehaviour, IBindable
    {
        [SerializeField] private bool _isInteracting = false;
        [Tag][SerializeField] private List<string> _whitelistTags = new List<string>() {"Player"};
        [SerializeField] private List<string> _blacklistTags = new List<string>() {};
        public UnityEvent OnBeginInteraction, OnEndInteraction;

        [BoxGroup("Snapshot")] [SerializeField] private Transform snapshotTransformParent;
        [BoxGroup("Snapshot")] [SerializeField] private Transform snapshotTransform;

        private void OnTriggerEnter(Collider other)
        {
            if (_whitelistTags.Contains(other.tag) && !_isInteracting)
            {
                Debug.Log(other.gameObject.name + " has stepped on " + gameObject.name);
                OnBeginInteraction?.Invoke();
                _isInteracting = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isInteracting)
            {
                Debug.Log(other.gameObject.name + " has stepped off " + gameObject.name);

                OnEndInteraction?.Invoke();
                _isInteracting = false;
            }
        }

        private void PlaybackStopped()
        {
            if (!_isInteracting) return;
            
            OnEndInteraction?.Invoke();
            _isInteracting = false;
        }
        public void BindObject()
        {
            OriginalEchoMina.Instance.StopPlayback += PlaybackStopped;
            OriginalEchoMina.Instance.StopRecording += PlaybackStopped;
        }
        public void UnBindObject()
        {
            OriginalEchoMina.Instance.StopPlayback -= PlaybackStopped;
            OriginalEchoMina.Instance.StopRecording -= PlaybackStopped;
        }
    }
}