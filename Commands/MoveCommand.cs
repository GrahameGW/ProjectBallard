using Godot;

namespace BallmontGame.Core
{
    public partial class MoveCommand : Command
    {
        public Piece Piece { get; private set; }
        public Square Start { get; private set; }
        public Square End { get; private set; }

        public MoveCommand(Piece piece, Square start, Square end) 
        {
            Piece = piece;
            Start = start;
            End = end;
        }

        public override void Dispatch(Player dispatcher)
        {
            GD.Print($"Dispatching move from {Start.Name} to {End.Name}");
            var actualPath = Piece.GetPath(Piece.Square, End);
            var displayPath = Piece.GetPath(Start, End);

            if (actualPath == null)
            {
                GD.Print("Server path not possible");
            }
            else
            {
                GD.Print("Starting travel for server piece");
                Piece.Square = End;
            }
            if (displayPath == null)
            {
                GD.Print("Display path not possible");
            }
            else
            {
                GD.Print("Starting travel for display piece");
                //dispatcher.PieceData[Piece] = End;
            }
        }
    }
}