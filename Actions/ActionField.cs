using UnityEngine;

namespace Ru1t3rl.Events
{
    [System.Serializable]
    public class ActionField
    {
        [SerializeField]
        private Object obj, gameObject, component;

        [SerializeField]
        private int selectedMethodIndex = 0, selectedComponentIndex = 0;

        [SerializeField]
        private string methodName;

        public static implicit operator System.Action(ActionField actionField)
        {
            return () => { actionField.component.GetType().GetMethod(actionField.methodName).Invoke(actionField.component, new object[] { }); };
        }
    }
}