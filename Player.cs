using Godot;
using System.Collections.Generic;
using System;


namespace ProjectBallard.Core
{
    public partial class Player : Node
    {
        public ChessColor Color { get; private set; }
        public int PeerId { get; private set; }
        public readonly Dictionary<Piece, Square> PieceData = new();
        public readonly List<Command> Commands;

        private Game game;


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
            Commands = new();
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
            if (game.IsMultiplayer)
            {
                SyncAndDispatchCommand(command);
                //Rpc(nameof(command.Dispatch));
            }
            else
            {
                command.Dispatch();
            }
            EmitSignal(SignalName.DispatchedCommand, command);
        }

        private void SyncAndDispatchCommand(Command command)
        {
            var oppId = game.Opponent.PeerId;
            var serde = command.Serialize();
            var type = command switch
            {
                MoveCommand => "MOVE",
                _ => throw new NotImplementedException()
            };
            Rpc("CreateRemoteCommand", serde, type);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        private void CreateRemoteCommand(Godot.Collections.Dictionary<string, Variant> parameters, string typeString)
        {
            var command = typeString switch
            {
                "MOVE" => new MoveCommand(),
                _ => throw new NotImplementedException()
            };
            command.Deserialize(parameters);
            command.Dispatch();
            EmitSignal(SignalName.DispatchedCommand, command);
        }
    }
}