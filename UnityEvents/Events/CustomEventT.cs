using UnityEngine.Events;

namespace Ru1t3rl.Events
{
    [System.Serializable]
    public class CustomEvent<T>
    {
        public string name;
        public UnityEvent<T> unityEvent;

        public CustomEvent(string name)
        {
            this.name = name;
            unityEvent = new UnityEvent<T>();
        }

        /// <summary>Add a listener to the unityevent</summary>
        /// <param name="listener">The listener to add</param>
        public void AddListener(System.Action<T> listener)
        {
            unityEvent.AddListener(listener.Invoke);
        }

        /// <summary>Remove a listener from the unityevent</summary>
        /// <param name="listener">The listener to remove</param>
        public void RemoveListener(System.Action<T> listener)
        {
            unityEvent.RemoveListener(listener.Invoke);
        }

        /// <summary>Invoke the unityevent</summary>
        public void Invoke(T args)
        {
            unityEvent.Invoke(args);
        }
    }
}