using Godot;

namespace ProjectBallard.Core
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

            GetNode("DebugHUD").Call("show");
            GetNode("DebugHUD/Visibility").Call("initialize", gameInstance);
            GetNode("DebugHUD/UserOppControl").Call("initialize", gameInstance);

            gameInstance.StartGame();
        }

        [Rpc(CallLocal = true)]
        public void StartMultiplayerGame(int serverColor)
        {
            GD.Print("Starting multiplayer game...");
            var color = Multiplayer.IsServer() ? (ChessColor)serverColor : (ChessColor)(serverColor ^ 1);
            GD.Print($"I am {color} | Server = {Multiplayer.IsServer()}");

            gameInstance?.QueueFree();
            gameInstance = packedGame.Instantiate<Game>();
            AddChild(gameInstance);
            gameInstance.Initialize(color, true);

            foreach (var node in this.GetTypedChildren<CanvasLayer>())
            {
                node.Hide();
            }

            GetNode("DebugHUD").Call("show");
            GetNode("DebugHUD/Visibility").Call("initialize", gameInstance);
            GetNode("DebugHUD/UserOppControl").Call("initialize", gameInstance);

            gameInstance.StartGame();
        }
    }
}