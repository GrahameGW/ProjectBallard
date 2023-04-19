using Godot;

namespace BallmontGame.Core
{
    public partial class PieceUI : Area2D
    {
        public TokenOwner OwnedBy { get; private set; }
        public Piece Piece { get; private set; }
        public Square Square {
            get => _square;
            set
            {
                Piece.Board.UpdateTokenPosition(this, _square, value);
                _square = value;
            }
        }

        public const int HELD_Z_INDEX = 50;
        public const int DEFAULT_Z_INDEX = 25;

        private PieceSprite sprite;
        private bool playerControlled;
        private bool isHeld;
        private Square _square = null;


        public void Initialize(Piece piece, TokenOwner owner)
        {
            Piece = piece;
            Square = piece.Square;
            OwnedBy = owner;
            sprite = GetNode<PieceSprite>("PieceSprite");
            sprite.Initialize(piece);
        }

        private void OnInputEvent(Node _vp, InputEvent input, long _si)
        {
            if (input.IsActionPressed("click"))
            {
                isHeld = true;
                ZIndex = HELD_Z_INDEX;
            }

            if (input.IsActionReleased("click") && isHeld)
            {
                isHeld = false;
                ZIndex = DEFAULT_Z_INDEX;
                GlobalPosition = Square.GetGlobalCenter();

                var destination = Square.Board.GetSquare(GetGlobalMousePosition());
                Square.Board.RequestPieceMove(Piece, Square, destination);
            }
        }

        public override void _Process(double delta)
        {
            if (isHeld)
            {
                GlobalPosition = GetGlobalMousePosition();
            }
        }

        private void OnSquareRectChanged()
        {
            GlobalPosition = Square.GetGlobalCenter();
        }
    }

    public enum TokenOwner
    {
        Server,
        User,
        Opponent
    }
}