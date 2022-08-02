using UnityEngine;
using System;
using System.Collections.Generic;
using Ru1t3rl.Extensions.Events;

namespace Ru1t3rl.Events
{
    [System.Serializable]
    public class CustomEvent
    {
        [SerializeField]
        private bool useInstanceID = true;
        public bool UseInstanceID => useInstanceID;

        [SerializeField]
        private string _id = string.Empty;
        public string id { get => _id + (UseInstanceID ? this.GetHashCode().ToString() : ""); protected set => _id = value; }

        [SerializeField]
        private List<ActionField> actionFields = new List<ActionField>();
        private List<Action> actions = new List<Action>();

        public List<Action> Actions
        {
            get
            {
                if (actionFields == null)
                {
                    actionFields = new List<ActionField>();
                }
                if (actions == null)
                {
                    actions = new List<Action>();
                }

                List<Action> totalActions = new List<Action>();
                totalActions.AddRange(actions);
                totalActions.AddRange(actionFields.ToActionList());
                return totalActions;
            }
        }

        public CustomEvent() { }
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
        public void Invoke()
        {
            for (int iAction = actionFields.Count; iAction-- > 0;)
            {
                try
                {
                    ((Action)actionFields[iAction])?.Invoke();
                }
                catch (MissingReferenceException) { actions.RemoveAt(iAction); }
            }

            for (int iAction = actions.Count; iAction-- > 0;)
            {
                try
                {
                    actions[iAction]?.Invoke();
                }
                catch (MissingReferenceException) { actions.RemoveAt(iAction); }
                catch (StackOverflowException) { }
            }
        }

        /// <summary>Add a method which will be fired once the event gets invoked</summary>
        /// <param name="action">The method you want to execute when the event gets fired</param>
        public void AddListener(Action action)
        {
            actions.Add(action);
        }

        /// <summary>Add a event which will be fired once the event gets invoked</summary>
        /// <param name="customEvent">The event you want to execute when the event gets fired</param>
        public void AddListener(CustomEvent customEvent)
        {
            actions.Add(customEvent.Invoke);
        }

        /// <summary>Remove a method from the list of listeners</summary>
        /// <param name="action">The method you want to stop from listening</param>
        public void RemoveListener(Action action)
        {
            actions.Remove(action);
        }

        /// <summary>Remove an event from the list of listeners</summary>
        /// <param name="customEvent">The event you want to stop from listening</param>
        public void RemoveListener(CustomEvent customEvent)
        {
            actions.Remove(customEvent.Invoke);
        }

        public bool IsListening(Action action) => actions.Contains(action);
        public bool IsListening(CustomEvent customEvent) => actions.Contains(customEvent.Invoke);

        #region Operator Overloading
        public static CustomEvent operator +(CustomEvent customEvent, Action action)
        {
            customEvent.AddListener(action);
            return customEvent;
        }
        public static CustomEvent operator +(CustomEvent customEvent1, CustomEvent customEvent2)
        {
            customEvent1.AddListener(customEvent2);
            return customEvent1;
        }

        public static CustomEvent operator -(CustomEvent customEvent, Action action)
        {
            customEvent.RemoveListener(action);
            return customEvent;
        }
        public static CustomEvent operator -(CustomEvent customEvent1, CustomEvent customEvent2)
        {
            customEvent1.RemoveListener(customEvent2);
            return customEvent1;
        }
        #endregion

        #region Casting
        /// <summary>Convert a System.Action to a CustomEvent</summary>
        public static implicit operator CustomEvent(Action action)
        {
            CustomEvent customEvent = new CustomEvent();
            customEvent.AddListener(action);
            return customEvent;
        }

        /// <summary>Convert CustomEvent to a System.Action</summary>
        public static implicit operator Action(CustomEvent customEvent)
        {
            Action action = delegate { };

            for (int iAction = 0; iAction < customEvent.actions.Count; iAction++)
            {
                action += customEvent.actions[iAction];
            }

            return action;
        }
        #endregion
    }
}