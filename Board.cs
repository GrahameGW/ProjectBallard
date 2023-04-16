using Godot;
using System.Collections.Generic;

namespace BallmontGame.Core
{
    public partial class Board : Control
    {
        [Export] PackedScene packedSquare;
        [Export] PackedScene packedPiece;
        [Export] float borderWidth;
        
        public Square[] Squares { get; private set; }
        public List<Piece> Pieces { get; private set; }
        public int Rows { get; private set; }
        public int Cols { get; private set; } 

        private const string VALID_FEN_PIECES = "pPrRnNbBqQkK";
        private const string DEFAULT_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 0";

        private GridContainer squareGrid;
        private Vector2 squareSize;

        [Signal]
        public delegate void BoardResetEventHandler();


        public void Initialize(int rows, int cols)
        {
            // Top left to bottom right
            Rows = rows;
            Cols = cols;
            Pieces = new();
            Squares = new Square[rows * cols];

            squareGrid = GetNode<GridContainer>("Board");
            squareGrid.Columns = Rows;
            var spacing = squareGrid.GetThemeConstant("h_separation");
            var width = Size.X - 2 * borderWidth - (Cols - 1) * spacing;
            var sqrWidth = width / Cols;
            squareSize = new Vector2(sqrWidth, sqrWidth);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var color = (ChessColor)(x + y & 1);
                    var square = packedSquare.Instantiate<Square>();
                    square.Initialize(color, new Vector2I(x, y), this, sqrWidth);
                    Squares[x + y * Cols] = square;
                    squareGrid.AddChild(square);
                }
            }
        }

        public void SetUpBoardForChess()
        {
            if (CheckValidFen(DEFAULT_FEN))
            {
                Pieces.Clear();
                SetUpPiecesFromFen(DEFAULT_FEN);
                EmitSignal(SignalName.BoardReset);
            }
        }

        private void SetUpPiecesFromFen(string fen)
        {
            var parts = fen.Split(' ');
            var coord = Vector2I.Zero;

            foreach (char c in parts[0])
            {
                if (c == '/')
                {
                    coord.Y += 1;
                    coord.X = 0;
                }
                else if (char.IsDigit(c))
                {
                    coord.X += c;
                }
                else
                {
                    var piece = packedPiece.Instantiate<Piece>();
                    piece.InitializeFromFenChar(c);
                    Squares[coord.X + coord.Y * Cols].Piece = piece;
                    Pieces.Add(piece);
                    coord.X += 1;
                }
            }
        }

        private static bool CheckValidFen(string fen)
        {
            int squares = 0;
            int rows = 1;
            foreach (char c in fen)
            {
                if (VALID_FEN_PIECES.Contains(c))
                {
                    squares++;
                }
                else if (c == '/')
                {
                    rows++;
                }
                else if (char.IsDigit(c))
                {
                    squares += c - '0';
                }
                else
                {
                    if (squares != 64 || rows != 8)
                    {
                        GD.PushWarning($"{fen} is not a valid FEN string. Check failed at char[{squares}] (row {rows}): '{c}'");
                    }
                    break;
                }
            }

            return squares == 64 && rows == 8;
        }
    }
}