using System.Collections.Generic;
using EchoMina.Original;
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

        public void SaveCheckpoint()
        {
            checkpoints[loadIndex].position = PlayerController.Instance.transform.position;
            checkpoints[loadIndex].rotation = PlayerController.Instance.transform.rotation;
        }

        public void SetCheckpoint(int index)
        {
            loadIndex = index;
        }
    }
}