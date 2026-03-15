using System;
using System.Collections.Generic;
using EchoMina.Original;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;

namespace QuickLoad
{
    public class QuickLoader : MonoBehaviour
    {
        // ReSharper disable once UnassignedField.Global
        public static QuickLoader Instance;

        [SerializeField] private List<Transform> checkpoints = new List<Transform>();
        [SerializeField] private Transform quickSavePoint;
        [SerializeField] private int loadIndex;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);
        }

        [Button]
        public void LoadCheckpoint()
        {
            PlayerController.Instance.transform.position = checkpoints[loadIndex].position;
            PlayerController.Instance.transform.rotation = checkpoints[loadIndex].rotation;

            if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
            {
                OriginalEchoMina.Instance.ToggleRecording(PlayerController.Instance.transform.position, PlayerController.Instance.transform.rotation);
            }
            else if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Playing)
            {
                OriginalEchoMina.Instance.TogglePlaying();
            }
        }

        public void QuickLoadCheckpoint()
        {
            PlayerController.Instance.transform.position = quickSavePoint.position;
            PlayerController.Instance.transform.rotation = quickSavePoint.rotation;
        }

        public void SaveCheckpoint()
        {
            quickSavePoint.position = PlayerController.Instance.transform.position;
            quickSavePoint.rotation = PlayerController.Instance.transform.rotation;
        }

        public void SetCheckpoint(int index)
        {
            loadIndex = index;
        }

        private void OnDrawGizmos()
        {
            if (quickSavePoint == null) return;
            if (checkpoints.Count == 0) return;

            Color  prevColor = Gizmos.color;

            Gizmos.color = Color.blueViolet;
            Gizmos.DrawSphere(quickSavePoint.position, 0.2f);

            if (checkpoints.Count == 0)
            {
                Gizmos.color = prevColor;
                return;
            }

            Gizmos.color = Color.gold;
            Gizmos.DrawSphere(checkpoints[loadIndex].position, 0.2f);

            Gizmos.color = prevColor;
        }
    }
}