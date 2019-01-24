﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ray.Core.Event;
using Ray.Core.Serialization;
using Ray.Core.State;

namespace Ray.Core
{
    public abstract class ReplicaGrain<K, E, S, B, W> : FollowGrain<K, E, S, B, W>
        where E : IEventBase<K>
        where S : class, IState<K, B>, new()
        where B : ISnapshot<K>, new()
        where W : IBytesWrapper
    {
        public ReplicaGrain(ILogger logger) : base(logger)
        {
        }
        protected IEventHandler<K, E, S, B> EventHandler { get; private set; }
        public override Task OnActivateAsync()
        {
            EventHandler = ServiceProvider.GetService<IEventHandler<K, E, S, B>>();
            return base.OnActivateAsync();
        }
        /// <summary>
        /// true:表示快照需要在Rep里做异步保存,false:表示快照在主Actor里保存
        /// </summary>
        protected override bool SaveSnapshot => false;
        protected override ValueTask OnEventDelivered(IEvent<K, E> @event)
        {
            EventHandler.Apply(State, @event);
            return Consts.ValueTaskDone;
        }
    }
}
