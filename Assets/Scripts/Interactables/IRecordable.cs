using EchoMina.Original;

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
        }

        /// <summary>
        /// Called before Echo Mina is destroyed to unsubscribe all the methods that were subscribed in BindRecordable
        /// </summary>
        public void UnbindRecordable()
        {
            OriginalEchoMina.Instance.StartRecording -= TakeSnapshot;
            OriginalEchoMina.Instance.StopRecording -= LoadSnapshot;
        }

        /// <summary>
        /// Take a snapshot of all the information when the recording starts
        /// </summary>
        public void TakeSnapshot();

        /// <summary>
        /// Resets all the information when the recording stops so that it returns o how it was when we started recording
        /// </summary>
        public void LoadSnapshot();
    }
}