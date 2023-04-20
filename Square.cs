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
        
        public Piece Piece { get; set; }

        const int ASCII_LOWER_A = 97;


        public void Initialize(ChessColor color, Vector2I xy, Vector2 size, Board board)
        {
            XY = xy;
            Board = board;
            Name = $"{(char)(ASCII_LOWER_A + xy.X)}{board.Rows - xy.Y}";
            Color = color == ChessColor.White ? lightColor : darkColor;
            CustomMinimumSize = size;
            Size = size;
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