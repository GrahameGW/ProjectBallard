using Godot;
using System;
using System.Collections.Generic;

namespace BallmontGame.Core
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
            var card = cardMap[command];
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
    }
}