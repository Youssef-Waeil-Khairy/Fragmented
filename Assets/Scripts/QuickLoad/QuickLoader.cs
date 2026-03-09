using System.Collections.Generic;
using EchoMina.Original;
using PlayerControls;
using UnityEngine;

namespace QuickLoad
{
    public class QuickLoader : MonoBehaviour
    {
        public static QuickLoader Instance;

        [SerializeField] private List<Transform> checkpoints = new List<Transform>();
        [SerializeField] private Transform quickSavePoint;
        [SerializeField] private int loadIndex = 0;

        public void LoadCheckpoint()
        {
            PlayerController.Instance.Controller.enabled = false;
            PlayerController.Instance.Controller.transform.position = checkpoints[loadIndex].position;
            PlayerController.Instance.Controller.transform.rotation = checkpoints[loadIndex].rotation;
            PlayerController.Instance.Controller.enabled = true;

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
            checkpoints[loadIndex].position = PlayerController.Instance.Controller.transform.position;
            checkpoints[loadIndex].rotation = PlayerController.Instance.Controller.transform.rotation;
        }

        public void SetCheckpoint(int index)
        {
            loadIndex = index;
        }
    }
}