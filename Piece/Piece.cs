using Godot;
using System.Collections.Generic;

namespace BallmontGame.Core
{
    public partial class Piece : Node2D
    {
        public ChessColor Color { get; private set; }
        public ChessPiece Type { get; private set; }

        public Square Square { get; set; }
        public Square VisibleSquare
        {
            get => ServerToken.Square;
        }

        public PieceUI ServerToken { get; private set; }
        public PieceUI UserToken { get; private set; }
        public PieceUI OppToken { get; private set; }

        public const float MOVE_SPEED = 2f;

        private Board board;

        public void InitializeFromFenChar(char c, Board board)
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
            
            ServerToken = GetNode<PieceUI>("ServerToken");
            // UserToken = GetNode<PieceUI>("UserToken");
            // OppToken = GetNode<PieceUI>("OppToken");

            ServerToken.Initialize(this);
            // UserToken.Initialize(this);
            // OppToken.Initialize(this);

            this.board = board;
            board.AddChild(this);
        }

        public void SetTokenToSquare(Square square)
        {
            ServerToken.Square = square;
            ServerToken.GlobalPosition = square.GetGlobalCenter();
        }

        public List<Square> GetPath(Square start, Square end, bool includeStart = false)
        {
            return Type switch
            {
                ChessPiece.King => Pathfinding.GetPathKing(start, end, includeStart),
                ChessPiece.Queen => Pathfinding.GetPathQueen(start, end, includeStart),
                ChessPiece.Rook => Pathfinding.GetPathRook(start, end, includeStart),
                ChessPiece.Bishop => Pathfinding.GetPathBishop(start, end, includeStart),
                ChessPiece.Knight => Pathfinding.GetPathKnight(start, end, includeStart),
                ChessPiece.Pawn => Pathfinding.GetPathPawn(start, end, Color, includeStart),
                _ => throw new System.ArgumentException("Invalid chess piece passed to path")
            };
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