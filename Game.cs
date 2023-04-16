using Godot;

namespace BallmontGame.Core
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


        public void Initialize(ChessColor userColor, bool multiplayerGame = false)
        {
            IsMultiplayer = multiplayerGame;
            Name = "ChessGame";

            Board = GetNode<Board>("ChessBoard");
            Board.Initialize(8, 8, this);

            var commandUi = GetNode<CommandUI>("CommandUI");
            commandUi.Initialize(this);

            User = CreateUser(userColor, multiplayerGame);
            Opponent = CreateOpponent(userColor.InvertColor(), multiplayerGame);
            AddChild(User);
            AddChild(Opponent);
        }

        public void StartGame()
        {
            Board.SetUpBoardForChess();
            VisiblePlayer = User;
            EmitSignal(SignalName.VisiblePlayerChanged, VisiblePlayer);
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
    }
}