using Godot;

namespace BallmontGame.Core
{
    public partial class PieceUI : Area2D
    {
        public Piece Piece { get; private set; }
        public Square Square {
            get => _square;
            set
            {
                if (_square != null)
                {
                    _square.ItemRectChanged -= OnSquareRectChanged;
                }
                _square = value;
                if (_square != null)
                {
                    _square.ItemRectChanged += OnSquareRectChanged;
                }
            }
        }

        public const int HELD_Z_INDEX = 50;
        public const int DEFAULT_Z_INDEX = 25;

        private PieceSprite sprite;
        private bool playerControlled;
        private bool isHeld;
        private Square _square;


        public void Initialize(Piece piece)
        {
            Piece = piece;
            Square = piece.Square;
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
                Square.Board.RequestPieceMove(Piece, destination);
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
}