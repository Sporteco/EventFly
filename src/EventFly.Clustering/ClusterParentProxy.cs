using Akka.Actor;
using EventFly.Messages;

namespace EventFly.Clustering
{
    public class ClusterParentProxy : ReceiveActor
    {
        private readonly IActorRef _child;
        
        public ClusterParentProxy(Props childProps, bool shouldUnsubscribe = true)
        {
            _child = Context.ActorOf(childProps);
            
            if(shouldUnsubscribe)
                _child.Tell(UnsubscribeFromAll.Instance);
            
            ReceiveAny(Forward);
        }

        public void Forward(object message)
        {
            _child.Forward(message);
        }
    }
}