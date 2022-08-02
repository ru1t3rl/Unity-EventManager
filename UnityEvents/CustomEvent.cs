using UnityEngine.Events;

namespace Ru1t3rl.Events
{
    [System.Serializable]
    public class CustomEvent
    {
        public string name;
        public UnityEvent unityEvent;

        public CustomEvent(string name)
        {
            this.name = name;
            unityEvent = new UnityEvent();
        }

        /// <summary>Add a listener to the unityevent</summary>
        /// <param name="listener">The listener to add</param>
        public void AddListener(System.Action listener)
        {
            unityEvent.AddListener(listener.Invoke);
        }

        /// <summary>Remove a listener from the unityevent</summary>
        /// <param name="listener">The listener to remove</param>
        public void RemoveListener(System.Action listener)
        {
            unityEvent.RemoveListener(listener.Invoke);
        }

        /// <summary>Invoke the unityevent</summary>
        public void Invoke()
        {
            unityEvent.Invoke();
        }
    }
}