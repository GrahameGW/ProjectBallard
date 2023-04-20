using Godot;

namespace ProjectBallard.Core
{
    public partial class MoveCommandCard : CommandCard
    {
        private MoveCommand command;
        
        public override void Initialize(Command command)
        {
            var moveCmd = command as MoveCommand ?? 
                throw new System.ArgumentException("Wrong command type passed to MoveCommandCard");

            GetNode<Label>("StartSquareLabel").Text = moveCmd.Start.Name;
            GetNode<Label>("EndSquareLabel").Text = moveCmd.End.Name;
            GetNode<PieceSprite>("PieceSprite").Initialize(moveCmd.Piece);
            this.command = moveCmd;
            moveCmd.CommandExecutionCompleted += OnCommandExecutionComplete;
        }
    }
}