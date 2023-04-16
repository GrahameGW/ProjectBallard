using Godot;
using System;

namespace BallmontGame.Core
{
    public partial class Player : Node
    {
        public ChessColor Color { get; private set; }
        public int PeerId { get; private set; }

        private Game game;


        public Player(ChessColor color, Game instance, int peerId = 0)
        {
            Color = color;
            PeerId = peerId;
            Name = peerId == 0 ? $"{Enum.GetName(typeof(ChessColor), color)} Player" : peerId.ToString();
            game = instance;
        }
    }
}