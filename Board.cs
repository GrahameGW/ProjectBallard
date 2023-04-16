using Godot;
using System.Collections.Generic;
using System.Linq;

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
        private bool isInverted;

        private Game game;


        [Signal]
        public delegate void BoardResetEventHandler();
        [Signal]
        public delegate void BoardDisplayChangedEventHandler();


        public void Initialize(int rows, int cols, Game game)
        {
            // Top left to bottom right
            Rows = rows;
            Cols = cols;
            Pieces = new();
            Squares = new Square[rows * cols];

            squareGrid = GetNode<GridContainer>("Board");
            squareGrid.Columns = Cols;
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
                    squareGrid.AddChild(square);
                    square.Initialize(color, new Vector2I(x, y), squareSize, this);
                    Squares[x + y * Cols] = square;
                }
            }

            this.game = game;
            game.VisiblePlayerChanged += OnVisiblePlayerChanged;
        }

        public void SetUpBoardForChess()
        {
            if (CheckValidFen(DEFAULT_FEN))
            {
                Pieces.Clear();
                SetUpPiecesFromFen(DEFAULT_FEN);
                EmitSignal(SignalName.BoardReset);

                if (game.User.Color == ChessColor.Black)
                {
                    Invert();
                }
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
                    piece.InitializeFromFenChar(c, this);
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

        private static void DisplayPieces(Dictionary<Piece, Square> data)
        {
            foreach (var piece in data.Keys)
            {
                piece.SetTokenToSquare(data[piece]);
            }
        }
        private static void DisplayPieces(List<Piece> pieces)
        {
            foreach (var piece in pieces)
            {
                piece.SetTokenToSquare(piece.Square);
            }
        }

        public Square GetSquare(Vector2 globalMousePos)
        {
            var gridPos = squareGrid.GlobalPosition;
            if (globalMousePos.X < gridPos.X || globalMousePos.Y < gridPos.Y
                || globalMousePos.X > gridPos.X + squareGrid.Size.X
                || globalMousePos.Y > gridPos.Y + squareGrid.Size.Y)
            {
                GD.Print($"{globalMousePos} is not on the chess board");
                return null;
            }

            var boardCoords = new Vector2(globalMousePos.X - gridPos.X, globalMousePos.Y - gridPos.Y);
            boardCoords /= squareSize;
            var coords = new Vector2I(
                Mathf.FloorToInt(boardCoords.X),
                Mathf.FloorToInt(boardCoords.Y)
                );
            // get inverted coords if board is flipped
            if (isInverted)
            {
                coords = new Vector2I(
                    coords.X ^ (Cols - 1),
                    coords.Y ^ (Rows - 1)
                    );
            }

            return GetSquare(coords);
        }
        public Square GetSquare(Vector2I coordinates)
        {
            return Squares.FirstOrDefault(s => s.XY == coordinates);
        }

        public void Invert()
        {
            var boardGrid = GetNode<GridContainer>("Board");
            var nodes = new Node[boardGrid.GetChildCount()];
            var mask = nodes.Length - 1;
            // sort list
            for (int i = 0; i < nodes.Length; i++)
            {
                var sqr = boardGrid.GetChild(i);
                nodes[i ^ mask] = sqr;
            }
            // reassign nodes to display correctly
            for (int i = 0; i < nodes.Length; i++)
            {
                boardGrid.MoveChild(nodes[i], i);
            }

            isInverted = !isInverted;
        }

        public void RequestPieceMove(Piece piece, Square destination)
        {
            if (game.VisiblePlayer != game.User || piece.Color != game.VisiblePlayer.Color)
            {
                GD.Print("You don't control this piece. Move canceled");
                return;
            }

            var move = new MoveCommand(piece, piece.VisibleSquare, destination);
            game.VisiblePlayer.EnqueueCommand(move);
            //game.VisiblePlayer.DispatchNextCommand();
        }

        private void OnVisiblePlayerChanged(Player player)
        {
            if (player != null)
            {
                // players
                DisplayPieces(player.PieceData);
            }
            else
            {
                // server
                DisplayPieces(game.Board.Pieces);
            }
        }
    }
}