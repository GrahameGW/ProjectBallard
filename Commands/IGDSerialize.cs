using Godot;
using Godot.Collections;

namespace ProjectBallard.Core
{
    public interface IGDSerialize
    {
        Dictionary<string, Variant> Serialize();
        void Deserialize(Dictionary<string, Variant> data);
    }
}