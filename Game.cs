using Godot;

namespace ProjectBallard.Core
{
    public partial class Game : Control
    {
        public Player User { get; private set; }
        public Player Opponent { get; private set; }
        public Board Board { get; private set; }
        public bool IsMultiplayer { get; private set; }

        public Player VisiblePlayer { get; private set; }

        [Signal]
        public delegate void VisiblePlayerChangedEventHandler(Player player);
        [Signal]
        public delegate void UserControlSwappedEventHandler();


        public void Initialize(ChessColor userColor, bool multiplayerGame = false)
        {
            IsMultiplayer = multiplayerGame;
            Name = "ChessGame";

            var commandUi = GetNode<CommandUI>("CommandUI");
            commandUi.Initialize(this);

            User = CreateUser(userColor, multiplayerGame);
            Opponent = CreateOpponent(userColor.InvertColor(), multiplayerGame);
            AddChild(User);
            AddChild(Opponent);

            Board = GetNode<Board>("ChessBoard");
            Board.Initialize(8, 8, this);
            Board.PieceCaptured += OnPieceCaptured;
        }

        public void StartGame()
        {
            // invert first so initialization is smoother
            if (User.Color == ChessColor.Black)
            {
                Board.Invert();
            }
            Board.SetUpBoardForChess();
            ToggleVisiblePlayer(User);
        }

        private Player CreateUser(ChessColor color, bool multiplayer)
        {
            return multiplayer ?
                new Player(color, this, Multiplayer.GetUniqueId()) :
                new Player(color, this);
        }

        private Player CreateOpponent(ChessColor color, bool multiplayer)
        {
            return multiplayer ?
                new Player(color, this, Multiplayer.GetPeers()[0]) :
                new Player(color, this);  
        }

        private void OnPieceCaptured(Piece captive, Piece captor)
        {
            if (captive.Type == ChessPiece.King)
            {
                GD.Print($"Game over! {captor.Color} wins!");
            }
        }
        public void ToggleVisiblePlayer(int index)
        {
            VisiblePlayer = index switch
            {
                2 => Opponent,
                1 => User,
                0 or _ => null
            };

            EmitSignal(SignalName.VisiblePlayerChanged, VisiblePlayer);
        }
        private void ToggleVisiblePlayer(Player player)
        {
            VisiblePlayer = player;
            EmitSignal(SignalName.VisiblePlayerChanged, VisiblePlayer);
        }

        #region Debug


        public void SwapPlayer()
        {
            (Opponent, User) = (User, Opponent);
        }

        public void SwapPieceTokens()
        {
            foreach (var piece in Board.Pieces)
            {
                piece.SwapUserOppTokens();
            }
        }
        #endregion
    }
}