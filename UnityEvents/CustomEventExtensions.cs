using System.Collections.Generic;
using UnityEngine.Events;

namespace Ru1t3rl.Events
{
    public static class CustomEventExtensions
    {
        /// <summary>Is there an event with the name in the list?</summary>
        /// <param name="key">The name of the event</param>
        /// <returns>True if the event exists, false otherwise</returns>
        public static bool ContainsKey(this List<CustomEvent> events, string key)
        {
            for (int iEvent = 0; iEvent < events.Count; iEvent++)
            {
                if (events[iEvent].name == key)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsEvent(this List<CustomEvent> events, UnityEvent unityEvent)
        {
            for (int iEvent = 0; iEvent < events.Count; iEvent++)
            {
                if (events[iEvent].unityEvent.GetHashCode() == unityEvent.GetHashCode())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Get event by name</summary>
        /// <param name="key">The name of the event</param>
        /// <returns>The event if the event exists, null otherwise</returns>
        public static CustomEvent GetEvent(this List<CustomEvent> events, string key)
        {
            for (int iEvent = 0; iEvent < events.Count; iEvent++)
            {
                if (events[iEvent].name == key)
                {
                    return events[iEvent];
                }
            }

            return null;
        }
    }
}