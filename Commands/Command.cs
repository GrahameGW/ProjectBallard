using Godot;

namespace BallmontGame.Core
{
    public abstract partial class Command : Node
    {
        protected bool stopExecution = false;
        
        public abstract void Dispatch();
        public virtual void Cancel()
        {
            stopExecution = true;
            QueueFree();
        }

        [Signal]
        public delegate void CommandExecutionCompletedEventHandler();
    }
}