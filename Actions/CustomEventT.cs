using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ru1t3rl.Events
{
    [System.Serializable]
    public class CustomEvent<EventArgs>
    {
        [SerializeField]
        private string _id = string.Empty;
        public string id { get => _id; protected set => _id = value; }

        [SerializeField]
        private bool useInstanceID = false;
        public bool UseInstanceID => useInstanceID;

        [SerializeField]
        private List<ActionField<EventArgs>> actionFields = new List<ActionField<EventArgs>>();
        private List<Action<EventArgs>> actions = new List<Action<EventArgs>>();

        public List<Action<EventArgs>> Actions
        {
            get
            {
                if (actionFields == null)
                {
                    actionFields = new List<ActionField<EventArgs>>();
                }
                if (actions == null)
                {
                    actions = new List<Action<EventArgs>>();
                }

                List<Action<EventArgs>> totalActions = new List<Action<EventArgs>>();
                totalActions.AddRange(actions);
                totalActions.AddRange(ToActionList(actionFields));
                return totalActions;
            }
        }

        public CustomEvent() { }

        /// <param name="id">id can be used to refrence the event in the manager</param>
        public CustomEvent(string id)
        {
            SetID(id);
        }

        /// <summary>Set the id/name of the event</summary>
        /// <param name="id">The id you want the event to have</param>
        public void SetID(string id)
        {
            this.id = id;
        }


        /// <summary>Execute the listeners</summary>
        /// <param name="value">A value that could possibly be used by it's listeners</param>
        public void Invoke(EventArgs value)
        {
            for (int iAction = actionFields.Count; iAction-- > 0;)
            {
                try
                {
                    ((Action<EventArgs>)actionFields[iAction])?.Invoke(value);
                }
                catch (MissingReferenceException) { actions.RemoveAt(iAction); }
            }

            for (int iAction = actions.Count; iAction-- > 0;)
            {
                try
                {
                    actions[iAction]?.Invoke(value);
                }
                catch (MissingReferenceException) { actions.RemoveAt(iAction); }
            }
        }

        /// <summary>Add a method which will be fired once the event gets invoked</summary>
        /// <param name="action">The method you want to execute when the event gets fired</param>
        public void AddListener(Action<EventArgs> action)
        {
            actions.Add(action);
        }

        /// <summary>Add a event which will be fired once the event gets invoked</summary>
        /// <param name="customEvent">The event you want to execute when the event gets fired</param>
        public void AddListener(CustomEvent<EventArgs> customEvent)
        {
            actions.Add(customEvent.Invoke);
        }

        /// <summary>Remove a method from the list of listeners</summary>
        /// <param name="action">The method you want to stop from listening</param>
        public void RemoveListener(Action<EventArgs> action)
        {
            actions.Remove(action);
        }

        /// <summary>Remove an event from the list of listeners</summary>
        /// <param name="customEvent">The event you want to stop from listening</param>
        public void RemoveListener(CustomEvent<EventArgs> customEvent)
        {
            actions.Remove(customEvent.Invoke);
        }

        #region Operator Overloading
        public static CustomEvent<EventArgs> operator +(CustomEvent<EventArgs> customEvent, Action<EventArgs> action)
        {
            customEvent.AddListener(action);
            return customEvent;
        }
        public static CustomEvent<EventArgs> operator +(CustomEvent<EventArgs> customEvent1, CustomEvent<EventArgs> customEvent2)
        {
            customEvent1.AddListener(customEvent2);
            return customEvent1;
        }

        public static CustomEvent<EventArgs> operator -(CustomEvent<EventArgs> customEvent, Action<EventArgs> action)
        {
            customEvent.RemoveListener(action);
            return customEvent;
        }
        public static CustomEvent<EventArgs> operator -(CustomEvent<EventArgs> customEvent1, CustomEvent<EventArgs> customEvent2)
        {
            customEvent1.RemoveListener(customEvent2);
            return customEvent1;
        }
        #endregion

        #region Casting
        /// <summary>Convert a System.Action<EventArgs> to a CustomEvent<EventArgs></summary>
        public static implicit operator CustomEvent<EventArgs>(Action<EventArgs> action)
        {
            CustomEvent<EventArgs> customEvent = new CustomEvent<EventArgs>();
            customEvent.AddListener(action);
            return customEvent;
        }

        /// <summary>Convert CustomEvent<EventArgs> to a System.Action<EventArgs></summary>
        public static implicit operator Action<EventArgs>(CustomEvent<EventArgs> customEvent)
        {
            Action<EventArgs> action = delegate { };

            for (int iAction = 0; iAction < customEvent.actions.Count; iAction++)
            {
                action += customEvent.actions[iAction];
            }

            return action;
        }

        public List<System.Action<EventArgs>> ToActionList(List<ActionField<EventArgs>> actionFields)
        {
            return actionFields.Select(action => (System.Action<EventArgs>)action).ToList();
        }
        #endregion
    }
}