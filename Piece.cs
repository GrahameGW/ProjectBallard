using Godot;

namespace BallmontGame.Core
{
    public partial class Piece : Area2D
    {
        public ChessColor Color { get; private set; }
        public ChessPiece Type { get; private set; }

        public Square Square { get; set; }

        public const float MOVE_SPEED = 2f;


        public void InitializeFromFenChar(char c)
        {
            Color = char.IsUpper(c) ? ChessColor.White : ChessColor.Black;
            Type = char.ToUpper(c) switch
            {
                'P' => ChessPiece.Pawn,
                'N' => ChessPiece.Knight,
                'B' => ChessPiece.Bishop,
                'R' => ChessPiece.Rook,
                'Q' => ChessPiece.Queen,
                'K' or _ => ChessPiece.King,
            };
            GetNode<PieceSprite>("PieceSprite").Initialize(this);
        }
    }

    public enum ChessPiece
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }
}