using UnityEngine;

namespace Ru1t3rl.Utilities
{
    public abstract class UnitySingleton<T> : MonoBehaviour where T : UnityEngine.Component
    {
        private static T instance;
        public static T Instance => instance ??= GameObject.FindObjectOfType<T>() ?? new GameObject(typeof(T).ToString()).AddComponent<T>();

        protected virtual void Awake()
        {
            if (Instance.GetHashCode() != this.GetHashCode())
            {
                Debug.LogWarning($"[{this.GetType()}] There are multiple instance of type {this.GetType()} in the scene. Please make sure there is only one!\n" +
                                 $"To prevent any errors I have been destroyed, my parent was {gameObject.name}");
                Destroy(this);
            }
        }
    }
}