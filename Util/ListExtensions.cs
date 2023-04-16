using System.Collections.Generic;

namespace BallmontGame.Core
{
    public static class ListExtensions
    {
        public static T Pop<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) { return default; }
            var popped = list[0];
            list.RemoveAt(0);
            return popped;
        }
    }
}