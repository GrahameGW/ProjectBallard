using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBallard.Core
{
    public partial class MoveCommand : Command
    {
        public Piece Piece { get; private set; }
        public Square Start { get; private set; }
        public Square End { get; private set; }

        public MoveCommand() { }
        public MoveCommand(Piece piece, Square start, Square end) 
        {
            Piece = piece;
            Start = start;
            End = end;
        }

        private bool serverTravelComplete = false;
        private bool userTravelComplete = false;

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public override void Dispatch()
        {
            GD.Print($"Dispatching move from {Start.Name} to {End.Name}");
            var actualPath = Piece.GetPath(Piece.Square, End);
            var userPath = Piece.GetPath(Start, End);

#pragma warning disable CS4014 
            if (actualPath != null)
            {
                Travel(actualPath, TokenOwner.Server);
            }
            else
            {
                serverTravelComplete = true;
            }
            if (userPath != null)
            {
                Travel(userPath, TokenOwner.User);
            }
            else
            {
                userTravelComplete = true;
            }
#pragma warning restore CS4014 

            // end travel if bad move
            if (serverTravelComplete && userTravelComplete)
            {
                EmitSignal(SignalName.CommandExecutionCompleted);
                QueueFree();
            }
        }

        private async void Travel(List<Square> path, TokenOwner owner)
        {
            if (path.Count == 1 && path[0] == Start)
            {
                return;
            }

            var token = Piece.GetToken(owner);
            var board = Start.Board;

            for (int i = 0; i < path.Count; i++)
            {
                var next = path[i];
                var current = token.Square;
                var lerpSpeed = GetLerpSpeed(current.XY, next.XY);

                // travel to square edge
                for (double t = 0.0; t < 0.5; t += Piece.GetPhysicsProcessDeltaTime() * lerpSpeed)
                {
                    await LerpPiece(token, current, next, (float)t);
                }

                //wait for square to clear
                var nextPiece = board.GetTokenInSquare(next, owner);
                if (nextPiece?.Piece.Color == Piece.Color && nextPiece.Piece != token.Piece)
                {
                    GD.Print($"Waiting for square {next.Name} to clear");
                    await WaitForSquareToClear(next, owner);
                    GD.Print($"Done waiting, square {next.Name} is clear");
                }

                // capture if server else display not server
                if (nextPiece != null)
                {
                    if (owner == TokenOwner.Server)
                    {
                        nextPiece.Piece.Capture(Piece);
                        path.Clear();
                    }
                    else
                    {
                        GD.Print($"{nextPiece.Name} isn't in Square {nextPiece.Square.Name}");
                    }
                }

                // move into next square
                token.Square = next;
                if (owner == TokenOwner.Server)
                {
                    token.Piece.Square = next;
                }

                // finish travel into square before starting next loop
                for (double t = 0.5; t < 1.0; t += Piece.GetPhysicsProcessDeltaTime() * lerpSpeed)
                {
                    await LerpPiece(token, current, next, (float)t);
                }

                token.GlobalPosition = next.GetGlobalCenter();
            }

            EndTravel(owner);
        }

        private void EndTravel(TokenOwner owner)
        {
            if (owner == TokenOwner.Server)
            {
                serverTravelComplete = true;
            }
            else if (owner == TokenOwner.User)
            {
                userTravelComplete = true;
            }

            if (serverTravelComplete && userTravelComplete)
            {
                EmitSignal(SignalName.CommandExecutionCompleted);
                QueueFree();
            }
        }

        private async Task LerpPiece(PieceUI token, Square current, Square next, float t)
        {
            await ToSignal(Piece.GetTree(), SceneTree.SignalName.PhysicsFrame);
            token.GlobalPosition = current.GetGlobalCenter().Lerp(next.GetGlobalCenter(), t);
        }

        private static double GetLerpSpeed(Vector2I start, Vector2I end)
        {
            // corrects for diagonal movement
            return Piece.MOVE_SPEED / (start - end).Length();
        }

        private async Task WaitForSquareToClear(Square square, TokenOwner owner)
        {
            var token = square.Board.GetTokenInSquare(square, owner);
            while (token.Piece.Color == Piece.Color)
            {
                await ToSignal(Piece.GetTree(), SceneTree.SignalName.PhysicsFrame);
                token = square.Board.GetTokenInSquare(square, owner);
            }
        }

        public override Godot.Collections.Dictionary<string, Variant> Serialize()
        {
            return new()
            {
                //{ "name", Name },
                { "piece", Piece },
                { "start", Start }, 
                { "end", End },
            };
        }

        public override void Deserialize(Godot.Collections.Dictionary<string, Variant> data)
        {
            //Name = (StringName)data["name"];
            Piece = (Piece)data["piece"];
            Start = (Square)data["start"];
            End = (Square)data["end"];
        }
    }
}