using Godot;
using Godot.Collections;

namespace ProjectBallard.Core
{
    public abstract partial class Command : Node, IGDSerialize
    {
        protected bool stopExecution = false;

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public abstract void Dispatch();

        public virtual void Cancel()
        {
            stopExecution = true;
            QueueFree();
        }

        public abstract Dictionary<string, Variant> Serialize();
        public abstract void Deserialize(Dictionary<string, Variant> data);

        [Signal]
        public delegate void CommandExecutionCompletedEventHandler();
    }
}