namespace ProjectBallard.Core
{
    public enum ChessColor
    {
        White,
        Black
    }

    public static class ChessColorExtensions
    {
        public static ChessColor InvertColor(this ChessColor color)
        {
            var other = (int)color ^ 1;
            return (ChessColor)other;
        }
    }
}