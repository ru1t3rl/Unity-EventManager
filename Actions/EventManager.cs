using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ru1t3rl.Events
{
    class EventManager
    {
#nullable enable
        private Dictionary<string, CustomEvent?> noParamEvents = new Dictionary<string, CustomEvent?>();
        private Dictionary<string, CustomEvent<EventArgs>?> withParamEvents = new Dictionary<string, CustomEvent<EventArgs>?>();

        #region Thread-safe singleton setup
        private static readonly object _lock = new object();

        private static EventManager? instance;
        public static EventManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new EventManager();
                    }
                    return instance;
                }
            }
        }
        #endregion

        #region Manage Events
        /// <summary>
        /// Creates a new empty event and adds it to one of the event dictionaries
        /// </summary>
        /// <param name="eventKey">The key that will be used to reference an event</param>
        /// <param name="hasParameters">Does the event pass parameters</param>
        public void AddEvent(string eventKey, bool hasParameters = false)
        {
            if (!hasParameters)
            {
                if (!Instance.noParamEvents.ContainsKey(eventKey))
                {
                    Instance.noParamEvents.Add(eventKey, new CustomEvent(eventKey));
                }
            }
            else
            {
                if (!Instance.withParamEvents.ContainsKey(eventKey))
                {
                    Instance.withParamEvents.Add(eventKey, new CustomEvent<EventArgs>(eventKey));
                }
            }
        }

        /// <summary>
        /// Adds an existing event to the events dictionary
        /// </summary>
        /// <param name="eventKey">The key that will be used to reference an event</param>
        /// <param name="action">The existing event to add to the dictionary</param>
        public void AddEvent(string eventKey, Action action)
        {
            if (Instance.noParamEvents.ContainsKey(eventKey))
            {
                Instance.noParamEvents[eventKey] += action;
            }
            else
            {
                Instance.noParamEvents.Add(eventKey, action);
            }
        }

        /// <summary>
        /// Adds an existing CustomEvent to the events dictionary
        /// </summary>
        /// <param name="customEvent">The custom event you want to add to the dictionary</param>        
        public void AddEvent(CustomEvent customEvent)
        {
            if (Instance.noParamEvents.ContainsKey(customEvent.id))
            {
                Instance.noParamEvents[customEvent.id]!.AddListener(customEvent);
            }
            else
            {
                Instance.noParamEvents.Add(customEvent.id, customEvent);
            }
        }

        /// <summary>
        /// Adds an existing event with paramters to the events dictionary
        /// </summary>
        /// <param name="eventKey">The key that will be used to reference an event</param>
        /// <param name="action">The existing event to add to the dictionary</param>
        public void AddEvent(string eventKey, Action<EventArgs> action)
        {
            if (Instance.withParamEvents.ContainsKey(eventKey))
            {
                Instance.withParamEvents[eventKey] += action;
            }
            else
            {
                Instance.withParamEvents.Add(eventKey, action);
            }
        }

        /// <summary>
        /// Adds an existing CustomEvent to the events dictionary
        /// </summary>
        /// <param name="customEvent">The custom event you want to add to the dictionary</param>        
        public void AddEvent(CustomEvent<EventArgs> customEvent)
        {
            if (Instance.noParamEvents.ContainsKey(customEvent.id))
            {
                Instance.withParamEvents[customEvent.id]!.AddListener(customEvent);
            }
            else
            {
                Instance.withParamEvents.Add(customEvent.id, customEvent);
            }
        }

        /// <summary>
        /// Remove an existing event from the dictionary
        /// </summary>
        /// <param name="eventKey"></param>
        public void RemoveEvent(string eventKey)
        {
            if (Instance.noParamEvents.ContainsKey(eventKey))
            {
                Instance.noParamEvents.Remove(eventKey);
            }
            else if (Instance.withParamEvents.ContainsKey(eventKey))
            {
                Instance.withParamEvents.Remove(eventKey);
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
            }
        }
        #endregion

        #region Manage Listeners
        /// <summary>
        /// Add a listener to an event, if the event doens't exist a new one will be created
        /// </summary>
        /// <param name="eventKey">The name of the event to subscribe to</param>
        /// <param name="method">The method that will be exectued when the event is invoked</param>
        public void AddListener(string eventKey, Action? method)
        {
            if (!Instance.noParamEvents.ContainsKey(eventKey))
            {
                Instance.noParamEvents.Add(eventKey, new CustomEvent(eventKey));
            }

            Instance.noParamEvents[eventKey] += method;
        }

        /// <summary>
        /// Add a listener to an event, if the event doens't exist a new one will be created
        /// </summary>
        /// <param name="eventKey">The name of the event to subscribe to</param>
        /// <param name="method">The method that will be exectued when the event is invoked</param>
        public void AddListener(string eventKey, Action<EventArgs> method)
        {
            if (!Instance.withParamEvents.ContainsKey(eventKey))
            {
                Instance.withParamEvents.Add(eventKey, new CustomEvent<EventArgs>(eventKey));
            }

            Instance.withParamEvents[eventKey] += method;
        }

        /// <summary>
        /// Add a listener to an event, if the event doens't exist a new one will be created
        /// </summary>
        /// <param name="eventKey">The name of the event to subscribe to</param>
        /// <param name="method">The method that will be exectued when the event is invoked</param>
        public void AddListener(CustomEvent eventKey, Action? method)
        {
            if (!Instance.noParamEvents.ContainsKey(eventKey.id))
            {
                Instance.noParamEvents.Add(eventKey.id, new CustomEvent(eventKey.id));
            }

            Instance.noParamEvents[eventKey.id] += method;
        }

        /// <summary>
        /// Add a listener to an event, if the event doens't exist a new one will be created
        /// </summary>
        /// <param name="eventKey">The name of the event to subscribe to</param>
        /// <param name="method">The method that will be exectued when the event is invoked</param>
        public void AddListener(CustomEvent<EventArgs> eventKey, Action<EventArgs> method)
        {
            if (!Instance.withParamEvents.ContainsKey(eventKey.id))
            {
                Instance.withParamEvents.Add(eventKey.id, new CustomEvent<EventArgs>(eventKey.id));
            }

            Instance.withParamEvents[eventKey.id] += method;
        }

        /// <summary>
        /// Remove a listener from an event
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <param name="method">The method which needs to be removed</param>
        public void RemoveListener(string eventKey, Action method)
        {
            if (!Instance.noParamEvents.ContainsKey(eventKey))
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
            }

            Instance.noParamEvents[eventKey] -= method;
        }

        /// <summary>
        /// Remove a listener from an event
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <param name="method">The method which needs to be removed</param>
        public void RemoveListener(string eventKey, Action<EventArgs> method)
        {
            if (!Instance.withParamEvents.ContainsKey(eventKey))
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
            }

            Instance.withParamEvents[eventKey] -= method;
        }

        /// <summary>
        /// Get a list of listeners from an event without paramters
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <returns>Returns a list of actions (The Listeners)</returns>
        public List<Action> GetNoParameterEventListeners(string eventKey)
        {
            if (!Instance.noParamEvents.ContainsKey(eventKey))
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
            }

            return Instance.noParamEvents[eventKey]!.Actions;
        }

        /// <summary>
        /// Get a list of listeners from an event with paramters
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <returns>Returns a list of actions with parameters (The Listeners)</returns>
        public List<Action<EventArgs>> GetParameterEventListeners(string eventKey)
        {
            if (!Instance.withParamEvents.ContainsKey(eventKey))
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
            }

            return Instance.withParamEvents[eventKey]!.Actions;
        }

        /// <summary>
        /// Returns true if the listener is already listening to 'customEven' event
        /// </summary>
        /// <param name="eventKey">The event reference to check</param>
        /// <param name="action">The listeners to check for</param>
        public bool IsListening(string eventKey, Action action) => Instance.noParamEvents.ContainsKey(eventKey) && Instance.noParamEvents[eventKey]!.Actions.Contains(action);

        /// <summary>
        /// Returns true if the listener is already listening to 'customEven' event
        /// </summary>
        /// <param name="customEvent">The event to check</param>
        /// <param name="action">The listeners to check for</param>
        public bool IsListening(CustomEvent customEvent, Action action) => Instance.noParamEvents.ContainsKey(customEvent.id) && Instance.noParamEvents[customEvent.id]!.Actions.Contains(action);

        /// <summary>
        /// Returns true if the listener is already listening to 'customEven' event
        /// </summary>
        /// <param name="eventKey">The event reference to check</param>
        /// <param name="action">The listeners to check for</param>
        public bool IsListening(string eventKey, Action<EventArgs> action) => Instance.withParamEvents.ContainsKey(eventKey) && Instance.withParamEvents[eventKey]!.Actions.Contains(action);

        /// <summary>
        /// Returns true if the listener is already listening to 'customEven' event
        /// </summary>
        /// <param name="customEvent">The event to check</param>
        /// <param name="action">The listeners to check for</param>
        public bool IsListening(CustomEvent<EventArgs> customEvent, Action<EventArgs> action) => Instance.withParamEvents.ContainsKey(customEvent.id) && Instance.withParamEvents[customEvent.id]!.Actions.Contains(action);
        #endregion

        #region Execution
        /// <summary>
        /// Invoke the event and execute it's listeners
        /// </summary>
        /// <param name="eventKey">The event name</param>
        public void Invoke(string eventKey, bool createNewIfNotFound = false)
        {
            if (Instance.noParamEvents.ContainsKey(eventKey))
            {
                Instance.noParamEvents[eventKey]?.Invoke();
            }
            else if (createNewIfNotFound)
            {
                Instance.AddEvent(eventKey);
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
            }
        }

        /// <summary>
        /// Invoke the event and execute it's listeners
        /// </summary>
        /// <param name="eventKey">The event name</param>
        /// <param name="args">The arguments to use</param>
        public void Invoke(string eventKey, EventArgs args, bool createNewIfNotFound = false)
        {
            if (Instance.withParamEvents.ContainsKey(eventKey))
            {
                Instance.withParamEvents[eventKey]?.Invoke(args);
            }
            else if (createNewIfNotFound)
            {
                Instance.AddEvent(eventKey);
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");

            }
        }
        #endregion

        #region Ohter
        /// <summary>
        /// Get an event (you won't be able to add or remove events using this!)
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <returns>An event</returns>
        public CustomEvent? GetEvent(string eventKey)
        {
            if (Instance.noParamEvents.ContainsKey(eventKey))
            {
                return Instance.noParamEvents[eventKey];
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
                return null;
            }
        }

        /// <summary>
        /// Get an event (you won't be able to add or remove events using this!)
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <returns>An event</returns>
        public CustomEvent<EventArgs>? GetParameterizedEvent(string eventKey)
        {
            if (Instance.withParamEvents.ContainsKey(eventKey))
            {
                return Instance.withParamEvents[eventKey];
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
                return null;
            }
        }

        /// <summary>
        /// Return true if either noParamEvents or withParamEvents conatins an event with your id
        /// </summary>
        /// <param name="eventId">The id of the event to check for</summary>
        public bool Contains(string eventId)
        {
            return noParamEvents.ContainsKey(eventId) || withParamEvents.ContainsKey(eventId);
        }
        #endregion
    }
}
