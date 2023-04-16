using Godot;

namespace BallmontGame.Core
{
    public partial class PieceSprite : Sprite2D
    {
        private const float PIECE_REGION_SIZE = 120f;


        public void Initialize(Piece piece)
        {
            var y = piece.Color == ChessColor.Black ? 0 : PIECE_REGION_SIZE;
            var x = GetPieceSpriteIndex(piece) * PIECE_REGION_SIZE;
            var rect = RegionRect;
            rect.Position = new Vector2(x, y);
            RegionRect = rect;
        }

        private static float GetPieceSpriteIndex(Piece piece)
        {
            return piece.Type switch
            {
                ChessPiece.King => 0f,
                ChessPiece.Queen => 1f,
                ChessPiece.Rook => 2f,
                ChessPiece.Bishop => 3f,
                ChessPiece.Knight => 4f,
                ChessPiece.Pawn or _ => 5f,
            };
        }
    }
}
