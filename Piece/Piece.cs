using Godot;
using System.Collections.Generic;

namespace ProjectBallard.Core
{
    public partial class Piece : Node2D
    {
        public ChessColor Color { get; private set; }
        public ChessPiece Type { get; private set; }

        public Square Square
        {
            get => _square;
            set
            {
                if (_square != null)
                {
                    _square.Piece = null;
                }

                _square = value;
                value.Piece = this;
                ServerToken.Square = value; // server token matches actual
            }
        }
        private Square _square;

        public PieceUI ServerToken { get; private set; }
        public PieceUI UserToken { get; private set; }
        public PieceUI OppToken { get; private set; }

        public const float MOVE_SPEED = 2f;

        public Board Board { get; private set; }

        public void InitializeFromFenChar(char c, Square square)
        {
            // do this before so initialization works right
            Board = square.Board;
            Board.AddChild(this);

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
            UserToken = GetNode<PieceUI>("UserToken");
            OppToken = GetNode<PieceUI>("OppToken");

            ServerToken.Initialize(this, TokenOwner.Server);
            UserToken.Initialize(this, TokenOwner.User);
            OppToken.Initialize(this, TokenOwner.Opponent);

            // do this afterwards so initialization works right
            Square = square;
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

        public void Capture(Piece capturedBy)
        {
            Board.CapturePiece(this, capturedBy);
            GD.Print("Piece captured!");
            QueueFree();
        }

        public void ShowUserToken()
        {
            ServerToken.Hide();
            OppToken.Hide();
            UserToken.Show();
        }
        public void ShowServerToken()
        {
            UserToken.Hide();
            OppToken.Hide();
            ServerToken.Show();
        }
        public void ShowOppToken()
        {
            ServerToken.Hide();
            UserToken.Hide();
            OppToken.Show();
        }

        public void SetAllToSquare(Square square, bool updatePosition = true)
        {
            UserToken.Square = OppToken.Square = square;
            Square = square;
            if (updatePosition)
            {
                ServerToken.GlobalPosition = UserToken.GlobalPosition
                    = OppToken.GlobalPosition = square.GetGlobalCenter();
            }
        }

        public PieceUI GetToken(TokenOwner owner)
        {
            return owner switch
            {
                TokenOwner.User => UserToken,
                TokenOwner.Opponent => OppToken,
                TokenOwner.Server or _ => ServerToken
            };
        }

        public void SwapUserOppTokens()
        {
            (UserToken, OppToken) = (OppToken, UserToken);
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