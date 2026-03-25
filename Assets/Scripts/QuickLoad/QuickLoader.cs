using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EchoMina.Original;
using Interactables;
using NaughtyAttributes;
using PlayerControls;
using UnityEngine;

namespace QuickLoad
{
    public class QuickLoader : MonoBehaviour
    {
        // ReSharper disable once UnassignedField.Global
        public static QuickLoader Instance;

        public delegate void OnCheckpointSave();
        public delegate void OnCheckpointLoad();
        public event OnCheckpointSave CheckpointSave;
        public event OnCheckpointLoad CheckpointLoad;

        [SerializeField] private List<Transform> checkpoints = new List<Transform>();
        [SerializeField] private Transform quickSavePoint;
        [SerializeField] private int loadIndex;
        private IRecordable[] recordables;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);
        }

        [Button]
        public void LoadCheckpoint()
        {
            if (CheckpointLoad != null) CheckpointLoad();

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
            if (CheckpointLoad != null) CheckpointLoad();

            PlayerController.Instance.transform.position = quickSavePoint.position;
            PlayerController.Instance.transform.rotation = quickSavePoint.rotation;

            if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Recording)
            {
                OriginalEchoMina.Instance.ToggleRecording(PlayerController.Instance.transform.position, PlayerController.Instance.transform.rotation);
            }
            else if (OriginalEchoMina.Instance.State is OriginalEchoMina.EchoState.Playing)
            {
                OriginalEchoMina.Instance.TogglePlaying();
            }
        }

        public void SaveCheckpoint()
        {
            if (CheckpointSave != null) CheckpointSave();

            quickSavePoint.position = PlayerController.Instance.transform.position;
            quickSavePoint.rotation = PlayerController.Instance.transform.rotation;
        }

        public void SetCheckpoint(int index)
        {
            if (CheckpointSave != null) CheckpointSave();

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
            if (checkpoints[loadIndex] != null) Gizmos.DrawSphere(checkpoints[loadIndex].position, 0.2f);

            Gizmos.color = Color.brown;
            for (int i = 0; i < checkpoints.Count; i++)
            {
                if (i == loadIndex) continue;

                Gizmos.DrawSphere(checkpoints[i].position, 0.2f);
            }

            Gizmos.color = prevColor;
        }
    }
}