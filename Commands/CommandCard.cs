using Godot;

namespace ProjectBallard.Core
{
    public abstract partial class CommandCard : NinePatchRect
    {
        public abstract void Initialize(Command command);

        public virtual void OnCommandExecutionComplete()
        {
            QueueFree();
        }
    }
}