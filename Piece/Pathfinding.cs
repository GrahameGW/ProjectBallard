using Godot;
using System.Collections.Generic;

namespace ProjectBallard.Core
{
    public static class Pathfinding
    {

        public static List<Square> GetPath(this Piece piece, Square start, Square end, bool includeStart = false)
        {
            return piece.Type switch
            {
                ChessPiece.King => GetPathKing(start, end, includeStart),
                ChessPiece.Queen => GetPathQueen(start, end, includeStart),
                ChessPiece.Rook => GetPathRook(start, end, includeStart),
                ChessPiece.Bishop => GetPathBishop(start, end, includeStart),
                ChessPiece.Knight => GetPathKnight(start, end, includeStart),
                ChessPiece.Pawn => GetPathPawn(start, end, piece.Color, includeStart),
                _ => null
            };
        }

        public static List<Square> GetPathKing(Square start, Square end, bool includeStart)
        {
            if (Mathf.Abs(start.XY.X - end.XY.X) + Mathf.Abs(start.XY.Y - end.XY.Y) > 1)
            {
                return null;
            }
            else if (includeStart)
            {
                return new List<Square> { start, end };
            }
            else
            {
                return new List<Square> { end };
            }
        }

        public static List<Square> GetPathQueen(Square start, Square end, bool includeStart)
        {
            if (start.XY.X == end.XY.X || start.XY.Y == end.XY.Y)
            {
                return GetPathRook(start, end, includeStart);
            }
            else if (Mathf.Abs(start.XY.X - end.XY.X) == Mathf.Abs(start.XY.Y - end.XY.Y))
            {
                return GetPathBishop(start, end, includeStart);
            }
            else
            {
                return null;
            }
        }

        public static List<Square> GetPathRook(Square start, Square end, bool includeStart)
        {
            if (start.XY.X != end.XY.X && start.XY.Y != end.XY.Y)
            {
                return null;
            }

            var path = includeStart ? new List<Square> { start } : new List<Square>();
            var board = start.Board;

            if (start.XY.X == end.XY.X)
            {
                int steps = Mathf.Abs(end.XY.Y - start.XY.Y);
                int sign = Mathf.Sign(end.XY.Y - start.XY.Y);

                for (int i = 1; i <= steps; i++)
                {
                    path.Add(board.GetSquare(new Vector2I(start.XY.X, start.XY.Y + i * sign)));
                }

                return path;
            }
            else
            {
                int steps = Mathf.Abs(end.XY.X - start.XY.X);
                int sign = Mathf.Sign(end.XY.X - start.XY.X);

                for (int i = 1; i <= steps; i++)
                {
                    path.Add(board.GetSquare(new Vector2I(start.XY.X + i * sign, start.XY.Y)));
                }

                return path;
            }
        }

        public static List<Square> GetPathBishop(Square start, Square end, bool includeStart)
        {
            if (Mathf.Abs(start.XY.X - end.XY.X) != Mathf.Abs(start.XY.Y - end.XY.Y))
            {
                return null;
            }

            var path = includeStart ? new List<Square> { start } : new List<Square>();
            var board = start.Board;

            var signX = Mathf.Sign(end.XY.X - start.XY.X);
            var signY = Mathf.Sign(end.XY.Y - start.XY.Y);
            var steps = Mathf.Abs(end.XY.X - start.XY.X);

            for (int i = 1; i <= steps; i++)
            {
                path.Add(board.GetSquare(new Vector2I(start.XY.X + i * signX, start.XY.Y + i * signY)));
            }

            return path;
        }

        public static List<Square> GetPathKnight(Square start, Square end, bool includeStart)
        {
            var path = includeStart ? new List<Square> { start } : new List<Square>();
            var board = start.Board;
            var x = end.XY.X - start.XY.X;
            var y = end.XY.Y - start.XY.Y;

            if (Mathf.Abs(x) == 2 && Mathf.Abs(y) == 1)
            {
                path.Add(board.GetSquare(new Vector2I(start.XY.X + x, start.XY.Y)));
                path.Add(end);
                return path;
            }

            if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 2)
            {
                path.Add(board.GetSquare(new Vector2I(start.XY.X, start.XY.Y + y)));
                path.Add(end);
                return path;
            }

            return null;
        }

        public static List<Square> GetPathPawn(Square start, Square end, ChessColor color, bool includeStart)
        {
            if (end.XY.X != start.XY.X)
            {
                return null;
            }

            var path = includeStart ? new List<Square> { start } : new List<Square>();

            // Double jump on first move
            if (Mathf.Abs(end.XY.Y - start.XY.Y) >= 2)
            {
                if (color == ChessColor.White && start.XY.Y == 6 && end.XY.Y == 4)
                {
                    path.Add(start.Board.GetSquare(new Vector2I(end.XY.X, 5)));
                }
                else if (color == ChessColor.Black && start.XY.Y == 1 && end.XY.Y == 3)
                {
                    path.Add(start.Board.GetSquare(new Vector2I(end.XY.X, 2)));
                }
                else
                {
                    return null;
                }
            }

            path.Add(end);
            return path;
        }
    }
}