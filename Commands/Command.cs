using Godot;

namespace BallmontGame.Core
{
    public abstract partial class Command : Node
    {
        public abstract void Dispatch(Player dispatcher);
    }
}