using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectBallard.Core
{
    public partial class CommandUI : Control
    {
        [Export] PackedScene moveCardPackedScene;

        private readonly Dictionary<Command, Node> cardMap = new();
        private Player currentPlayer;
        private Node queueContainer;
        private Control spacer;

        public void Initialize(Game game)
        {
            game.VisiblePlayerChanged += OnVisiblePlayerChanged;
            queueContainer = FindChild("Queue");
            spacer = queueContainer.GetNode<Control>("Spacer");
            spacer.CustomMinimumSize = new Vector2();
        }

        private void OnVisiblePlayerChanged(Player player)
        {
            if (currentPlayer != null)
            {
                currentPlayer.DispatchedCommand -= OnPlayerDispatchedCommand;
                currentPlayer.AddedCommand -= OnPlayerAddedCommand;
            }
            if (player != null)
            {
                player.DispatchedCommand += OnPlayerDispatchedCommand;
                player.AddedCommand += OnPlayerAddedCommand;
            }
            currentPlayer = player;
            RefreshCardDisplay();
        }

        private void OnPlayerDispatchedCommand(Command command)
        {
            // tween into process area
            cardMap.Remove(command);
        }

        private void OnPlayerAddedCommand(Command command)
        {
            CommandCard card = command switch
            {
                MoveCommand => moveCardPackedScene.Instantiate<CommandCard>(),
                _ => throw new NotImplementedException()
            };

            card.Initialize(command);
            queueContainer.AddChild(card);
            cardMap[command] = card;
        }

        private void RefreshCardDisplay()
        {
            ClearCardDisplay();
            if (currentPlayer == null) { return; }

            foreach (var command in currentPlayer.Commands)
            {
                OnPlayerAddedCommand(command);
            }

            queueContainer.MoveChild(spacer, queueContainer.GetChildCount() - 1);
            spacer.CustomMinimumSize = new Vector2();
        }

        private void ClearCardDisplay()
        {
            foreach (var card in cardMap.Values)
            {
                card.QueueFree();
            }
            cardMap.Clear();
        }

        private void OnPieceCaptured(Piece captured, Piece captor)
        {
            foreach (var cmd in cardMap.Keys.Cast<MoveCommand>())
            {
                if (cmd == null) { continue; }
                var card = cardMap[cmd];
                card.QueueFree();
                cardMap.Remove(cmd);
                cmd.Cancel();
            }
        }
    }
}