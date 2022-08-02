using UnityEngine;
using System;

namespace Ru1t3rl.Events
{
    [Serializable]
    public class ActionField<EventArgs>
    {
        [SerializeField]
        private UnityEngine.Object obj, gameObject, component;

        [SerializeField]
        private int selectedMethodIndex = 0, selectedComponentIndex = 0;

        [SerializeField]
        private string methodName;

        public static implicit operator System.Action<EventArgs>(ActionField<EventArgs> actionField)
        {
            return (eventargs) => { actionField.component.GetType().GetMethod(actionField.methodName).Invoke(actionField.component, new object[] { eventargs }); };
        }
    }
}