﻿using System;
using System.Threading.Tasks;

namespace EventWay.Core
{
    public class AggregateStore : IAggregateStore
    {
        private readonly IAggregateRepository _aggregateRepository;
        private readonly IAggregateTracking _aggregateTracking;
        private readonly IEventListener _eventListener;

        public AggregateStore(IAggregateRepository aggregateRepository, IEventListener eventListener, IAggregateTracking aggregateTracking = null)
        {
            if (aggregateRepository == null) throw new ArgumentNullException(nameof(aggregateRepository));
            if (eventListener == null) throw new ArgumentNullException(nameof(eventListener));

            _aggregateRepository = aggregateRepository;
            _aggregateTracking = aggregateTracking;
            _eventListener = eventListener;
        }

        public T GetById<T>(Guid aggregateId) where T : IAggregate
        {
            return _aggregateRepository.GetById<T>(aggregateId);
        }

        public async Task Save(IAggregate aggregate)
        {
            if (_aggregateTracking != null)
                _aggregateTracking.TrackEvents(aggregate);

            var orderedEvents = _aggregateRepository.Save(aggregate);

            foreach (var @event in orderedEvents)
                await _eventListener.Handle(@event);
        }
    }
}