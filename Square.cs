using Godot;

namespace BallmontGame.Core
{
    public partial class Square : ColorRect
    {
        [Export]
        private Color darkColor;
        [Export]
        private Color lightColor;

        public Board Board { get; private set; }
        public Vector2I XY { get; private set; }
        
        public Piece Piece 
        {
            get => _piece;
            set { 
                if (_piece != null)
                {
                    _piece.Square = null;
                    GD.PushWarning($"{value.Name} pushed {_piece.Name} out from {Name}");
                }
                _piece = value;
                value.Square = this;
                if (value.GetParent() == null)
                {
                    AddChild(value);
                }
                else
                {
                    value.Reparent(this);
                }
                value.Position = GetCenter();
            }
        }
        private Piece _piece;

        const int ASCII_LOWER_A = 97;


        public void Initialize(ChessColor color, Vector2I xy, Board board, float width)
        {
            XY = xy;
            Board = board;
            Name = $"{(char)(ASCII_LOWER_A + xy.X)}{board.Rows - xy.Y}";
            Color = color == ChessColor.White ? lightColor : darkColor;
            CustomMinimumSize = new Vector2(width, width);
            MouseFilter = MouseFilterEnum.Ignore;
        }

        public Vector2 GetCenter()
        {
            return GetRect().Size * 0.5f;
        }

        public Vector2 GetGlobalCenter()
        {
            return GetCenter() + GlobalPosition;
        }
    }
}