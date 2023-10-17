using System;
using System.Collections.Generic;
using System.Linq;

namespace mDownloader.Helpers
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<object>> _handlers = new();
        private readonly object _lock = new object();

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            var eventType = typeof(TEvent);
            lock (_lock)
            {
                if (!_handlers.ContainsKey(eventType))
                {
                    _handlers[eventType] = new List<object>();
                }
                _handlers[eventType].Add(handler);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            var eventType = typeof(TEvent);
            lock (_lock)
            {
                if (_handlers.ContainsKey(eventType))
                {
                    _handlers[eventType].Remove(handler);
                    if (_handlers[eventType].Count == 0)
                    {
                        _handlers.Remove(eventType);
                    }
                }
            }
        }

        public void Publish<TEvent>(TEvent @event)
        {
            var eventType = typeof(TEvent);
            List<object> handlers;
            lock (_lock)
            {
                if (!_handlers.ContainsKey(eventType))
                {
                    return;
                }
                // 使用 ToList 创建 handlers 的副本，以防止在迭代过程中集合被修改
                handlers = _handlers[eventType].ToList();
            }

            foreach (var handler in handlers.Cast<Action<TEvent>>())
            {
                handler(@event);
            }
        }
    }

    public interface IEventAggregator
    {
        void Subscribe<TEvent>(Action<TEvent> handler);
        void Unsubscribe<TEvent>(Action<TEvent> handler);
        void Publish<TEvent>(TEvent @event);
    }
}
