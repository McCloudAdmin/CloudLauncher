using System;
using System.Collections.Generic;
using System.Linq;
using CloudLauncher.utils;

namespace CloudLauncher.plugins.Events
{
    /// <summary>
    /// Event manager implementation for handling plugin events
    /// </summary>
    public class EventManager : IEventManager
    {
        private readonly Dictionary<Type, List<EventSubscription>> _subscriptions = new Dictionary<Type, List<EventSubscription>>();
        private readonly object _lock = new object();

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            Subscribe(handler, 0);
        }

        public void Subscribe<T>(Action<T> handler, int priority) where T : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions[eventType] = new List<EventSubscription>();
                }

                var subscription = new EventSubscription
                {
                    Handler = evt => handler((T)evt),
                    Priority = priority,
                    EventType = eventType
                };

                _subscriptions[eventType].Add(subscription);
                
                // Sort by priority (higher numbers first)
                _subscriptions[eventType] = _subscriptions[eventType]
                    .OrderByDescending(s => s.Priority)
                    .ToList();

                Logger.Debug($"Subscribed to event {eventType.Name} with priority {priority}");
            }
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var eventType = typeof(T);
                if (_subscriptions.ContainsKey(eventType))
                {
                    var toRemove = _subscriptions[eventType]
                        .Where(s => s.Handler.Method.Equals(handler.Method) && s.Handler.Target.Equals(handler.Target))
                        .ToList();

                    foreach (var subscription in toRemove)
                    {
                        _subscriptions[eventType].Remove(subscription);
                    }

                    if (_subscriptions[eventType].Count == 0)
                    {
                        _subscriptions.Remove(eventType);
                    }

                    Logger.Debug($"Unsubscribed from event {eventType.Name}");
                }
            }
        }

        public void Publish<T>(T eventData) where T : IEvent
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            List<EventSubscription> subscriptions = null;
            lock (_lock)
            {
                var eventType = typeof(T);
                if (_subscriptions.ContainsKey(eventType))
                {
                    subscriptions = new List<EventSubscription>(_subscriptions[eventType]);
                }
            }

            if (subscriptions != null && subscriptions.Count > 0)
            {
                Logger.Debug($"Publishing event {typeof(T).Name} to {subscriptions.Count} subscribers");

                foreach (var subscription in subscriptions)
                {
                    try
                    {
                        subscription.Handler(eventData);
                        
                        // If the event was handled and it's cancellable, we might want to stop processing
                        if (eventData.IsHandled && eventData is ICancellableEvent cancellable && cancellable.IsCancelled)
                        {
                            Logger.Debug($"Event {typeof(T).Name} was cancelled, stopping further processing");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error handling event {typeof(T).Name}: {ex.Message}");
                        // Continue processing other handlers even if one fails
                    }
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _subscriptions.Clear();
                Logger.Info("All event subscriptions cleared");
            }
        }

        /// <summary>
        /// Get subscription count for a specific event type
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <returns>Number of subscriptions</returns>
        public int GetSubscriptionCount<T>() where T : IEvent
        {
            lock (_lock)
            {
                var eventType = typeof(T);
                return _subscriptions.ContainsKey(eventType) ? _subscriptions[eventType].Count : 0;
            }
        }

        /// <summary>
        /// Get subscription count for all events
        /// </summary>
        /// <returns>Total number of subscriptions</returns>
        public int GetTotalSubscriptionCount()
        {
            lock (_lock)
            {
                return _subscriptions.Values.Sum(list => list.Count);
            }
        }

        /// <summary>
        /// Get all event types that have subscriptions
        /// </summary>
        /// <returns>List of event types</returns>
        public List<Type> GetSubscribedEventTypes()
        {
            lock (_lock)
            {
                return new List<Type>(_subscriptions.Keys);
            }
        }

        /// <summary>
        /// Remove all subscriptions for a specific event type
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        public void ClearSubscriptions<T>() where T : IEvent
        {
            lock (_lock)
            {
                var eventType = typeof(T);
                if (_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions.Remove(eventType);
                    Logger.Debug($"Cleared all subscriptions for event {eventType.Name}");
                }
            }
        }
    }

    /// <summary>
    /// Internal event subscription wrapper
    /// </summary>
    internal class EventSubscription
    {
        public Action<IEvent> Handler { get; set; }
        public int Priority { get; set; }
        public Type EventType { get; set; }
    }
} 