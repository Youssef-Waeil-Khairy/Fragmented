using System;
using EchoMina.Original;
using QuickLoad;
using Unity.VisualScripting.FullSerializer;

namespace Interactables
{
    public interface IRecordable
    {
        /// <summary>
        /// Calls the object to subscribe to Echo Mina's recording start and stop delegates
        /// </summary>
        public void BindRecordable()
        {
            OriginalEchoMina.Instance.StartRecording += TakeSnapshot;
            OriginalEchoMina.Instance.StopRecording += LoadSnapshot;
            QuickLoader.Instance.CheckpointSave += TakeCheckpointSnapshot;
            QuickLoader.Instance.CheckpointLoad += LoadCheckpointSnapshot;
        }

        /// <summary>
        /// Called before Echo Mina is destroyed to unsubscribe all the methods that were subscribed in BindRecordable
        /// </summary>
        public void UnbindRecordable()
        {
            OriginalEchoMina.Instance.StartRecording -= TakeSnapshot;
            OriginalEchoMina.Instance.StopRecording -= LoadSnapshot;
            QuickLoader.Instance.CheckpointSave -= TakeCheckpointSnapshot;
            QuickLoader.Instance.CheckpointLoad -= LoadCheckpointSnapshot;
        }

        /// <summary>
        /// Take a snapshot of all the information when the recording starts
        /// </summary>
        public void TakeSnapshot();

        /// <summary>
        /// Takes a snapshot of the object's state for the checkpoint/quick save
        /// </summary>
        public void TakeCheckpointSnapshot();

        /// <summary>
        /// Resets all the information when the recording stops so that it returns o how it was when we started recording
        /// </summary>
        public void LoadSnapshot();

        /// <summary>
        /// Loads the snapshot for the checkpoint or quick save/load
        /// </summary>
        public void LoadCheckpointSnapshot();
    }
}