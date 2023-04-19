using Godot;
using System.Collections.Generic;
using System;


namespace BallmontGame.Core
{
    public partial class Player : Node
    {
        public ChessColor Color { get; private set; }
        public int PeerId { get; private set; }
        public readonly Dictionary<Piece, Square> PieceData = new();
        public readonly List<Command> Commands;

        private Game game;
        private Board board;


        [Signal]
        public delegate void DispatchedCommandEventHandler(Command command);
        [Signal]
        public delegate void AddedCommandEventHandler(Command command);

        public Player(ChessColor color, Game instance, int peerId = 0)
        {
            Color = color;
            PeerId = peerId;
            Name = peerId == 0 ? $"{Enum.GetName(typeof(ChessColor), color)} Player" : peerId.ToString();
            game = instance;
            board = game.Board;
            board.BoardReset += OnBoardReset;
            Commands = new();
        }

        private void OnBoardReset()
        {
            PieceData.Clear();
            foreach (var piece in game.Board.Pieces)
            {
                PieceData[piece] = piece.Square;
            }
        }

        public Godot.Collections.Array<Command> CommandsAsGDArray()
        {
            return new Godot.Collections.Array<Command>(Commands);
        }

        public void EnqueueCommand(Command command)
        {
            Commands.Add(command);
            EmitSignal(SignalName.AddedCommand, command);
        }

        public void DispatchNextCommand()
        {
            var command = Commands.Pop();
            if (command == null) { return; }
            command.Dispatch();
            EmitSignal(SignalName.DispatchedCommand, command);
        }
    }
}