using Godot;
using System.Collections.Generic;

namespace BallmontGame.Core
{
    public static class NodeExtensions
    {
        public static List<T> GetTypedChildren<T>(this Node node)
            where T : Node
        {
            var children = new List<T>();
            foreach (var n in node.GetChildren())
            {
                if (n is T)
                {
                    children.Add(n as T);
                }
            }
            return children;
        }
    }
}