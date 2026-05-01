using Interactables;
using NaughtyAttributes;
using UnityEngine;
// ReSharper disable RedundantDefaultMemberInitializer

namespace Utility
{
    public class Rotator : MonoBehaviour, IRecordable
    {
        public Vector3 targetRotation;
        public Vector3 startingRotation;
        public Quaternion goalRotation;
        public Quaternion sourceRotation;
        public float speed = 0.1f;
        public float timeCount = 0f;
        public bool isRotating = false;
        [SerializeField][ReadOnly] private Quaternion snapshotRotation;
        [SerializeField][ReadOnly] private Quaternion checkpointSnapshotRotation;

        private void Start()
        {
            startingRotation = transform.eulerAngles;
            goalRotation = transform.rotation;
            sourceRotation = transform.rotation;
        }

        private void Update()
        {
            if (!isRotating) return;

            transform.rotation = Quaternion.Lerp(sourceRotation, goalRotation, timeCount *  speed);
            timeCount += Time.deltaTime;

            if (transform.rotation == goalRotation)
            {
                isRotating = false;
            }
        }

        [Button]
        public void ForwardRotate()
        {
            goalRotation = Quaternion.Euler(targetRotation);
            sourceRotation = transform.rotation;
            isRotating = true;
            timeCount = 0f;
        }

        [Button]
        public void BackwardRotate()
        {
            goalRotation = Quaternion.Euler(startingRotation);
            sourceRotation = transform.rotation;
            isRotating = true;
            timeCount = 0f;
        }

        public void TakeSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Take]</color></b> Snapshot: {gameObject.name}]");

            snapshotRotation = transform.rotation;
        }
        public void TakeCheckpointSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Take][Checkpoint]</color></b> Snapshot: {gameObject.name}]");

            checkpointSnapshotRotation = transform.rotation;
        }
        public void LoadSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Load]</color></b> Snapshot: {gameObject.name}]");

            transform.rotation = snapshotRotation;
            isRotating = false;
            timeCount = 0f;
        }
        public void LoadCheckpointSnapshot()
        {
            Debug.Log($"<b><color=brown>[Snapshot][Load][Checkpoint]</color></b> Snapshot: {gameObject.name}]");

            transform.rotation = checkpointSnapshotRotation;
            isRotating = false;
            timeCount = 0f;
        }
    }
}