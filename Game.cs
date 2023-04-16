using Godot;

namespace BallmontGame.Core
{
    public partial class Game : Control
    {
        public Player User { get; private set; }
        public Player Opponent { get; private set; }
        public Board Board { get; private set; }
        public bool IsMultiplayer { get; private set; }


        public void Initialize(ChessColor userColor, bool multiplayerGame = false)
        {
            IsMultiplayer = multiplayerGame;
            Name = "ChessGame";

            User = CreateUser(userColor, multiplayerGame);
            Opponent = CreateOpponent(userColor.InvertColor(), multiplayerGame);
            AddChild(User);
            AddChild(Opponent);

            Board = GetNode<Board>("ChessBoard");
            Board.Initialize(8, 8);
        }

        public void StartGame()
        {
            Board.SetUpBoardForChess();
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
    }
}