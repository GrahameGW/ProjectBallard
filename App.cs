using Godot;

namespace BallmontGame.Core
{
    public partial class App : Node
    {
        [Export]
        private PackedScene packedGame;

        private Game gameInstance;

        public void StartSingleplayerGame(int playerColor)
        {
            GD.Print("Starting singleplayer game...");
            var color = (ChessColor)playerColor;

            gameInstance?.QueueFree();
            gameInstance = packedGame.Instantiate<Game>();
            gameInstance.Initialize(color, false);
            AddChild(gameInstance);

            foreach (var node in this.GetTypedChildren<CanvasLayer>())
            {
                node.Hide();
            }

            gameInstance.StartGame();
        }
    }
}