namespace MalbersAnimations.Events
{
    /// <summary>Simple Event Raiser On Disable</summary>
    public class UnityEventExit : UnityUtils
    {
        public UnityEngine.Events.UnityEvent OnDisableEvent;
        public void OnDisable() => OnDisableEvent.Invoke();
    }
}