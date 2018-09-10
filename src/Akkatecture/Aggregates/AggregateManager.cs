﻿// The MIT License (MIT) // Copyright (c) 2018 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture // Permission is hereby granted, free of charge, to any
// person obtaining a copy of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions: // The
// above copyright notice and this permission notice shall be included in all copies or substantial
// portions of the Software. // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
    public abstract class AggregateManager<TAggregate, TIdentity, TCommand> : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : class, ICommand<TAggregate, TIdentity>
    {
        public AggregateManagerSettings Settings { get; }

        protected AggregateManager()
        {
            this.Logger = Context.GetLogger();
            this.Settings = new AggregateManagerSettings(Context.System.Settings.Config);

            this.Receive<Terminated>(this.Terminate);

            if(this.Settings.AutoDispatchOnReceive)
            {
                this.Receive<TCommand>(this.Dispatch);
            }

            if(this.Settings.HandleDeadLetters)
            {
                Context.System.EventStream.Subscribe(this.Self, typeof(DeadLetter));
                this.Receive(this.DeadLetterHandler);
            }
        }

        protected Func<DeadLetter, bool> DeadLetterHandler => this.Handle;
        protected ILoggingAdapter Logger { get; set; }

        protected virtual IActorRef CreateAggregate(TIdentity aggregateId)
        {
            var aggregateRef = Context.ActorOf(Props.Create<TAggregate>(aggregateId), aggregateId.Value);
            Context.Watch(aggregateRef);
            return aggregateRef;
        }

        protected virtual bool Dispatch(TCommand command)
        {
            this.Logger.Info($"{this.GetType().PrettyPrint()} received {command.GetType().PrettyPrint()}");

            var aggregateRef = this.FindOrCreate(command.AggregateId);

            aggregateRef.Forward(command);

            return true;
        }

        protected virtual IActorRef FindOrCreate(TIdentity aggregateId)
        {
            var aggregate = Context.Child(aggregateId);

            if(Equals(aggregate, ActorRefs.Nobody))
            {
                aggregate = this.CreateAggregate(aggregateId);
            }

            return aggregate;
        }

        protected bool Handle(DeadLetter deadLetter)
        {
            if(deadLetter.Message is TCommand &&
                (deadLetter.Message as TCommand).AggregateId.GetType() == typeof(TIdentity))
            {
                var command = deadLetter.Message as TCommand;

                this.ReDispatch(command);
            }

            return true;
        }

        protected virtual bool ReDispatch(TCommand command)
        {
            this.Logger.Info($"{this.GetType().PrettyPrint()} as dead letter {command.GetType().PrettyPrint()}");

            var aggregateRef = this.FindOrCreate(command.AggregateId);

            aggregateRef.Tell(command);

            return true;
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 3000,
                localOnlyDecider: x =>
                {
                    this.Logger.Warning($"[{this.GetType().PrettyPrint()}] Exception={x.ToString()} to be decided.");
                    return Directive.Restart;
                });
        }

        protected virtual bool Terminate(Terminated message)
        {
            this.Logger.Warning($"{typeof(TAggregate).PrettyPrint()}: {message.ActorRef.Path} has terminated.");
            Context.Unwatch(message.ActorRef);
            return true;
        }
    }
}
