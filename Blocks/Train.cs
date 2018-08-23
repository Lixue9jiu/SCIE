using Engine;

namespace Game
{
    public class Train : Item
    {
        public override int GetFaceTextureSlot(int face, int value)
        {
            return 2;
        }
        public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
        {
            return Vector3.One;
        }
        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return "Steam Locomotive";
        }
        public override string GetDescription(int value)
        {
            return "A steam locomotive is a type of railway locomotive that produces its pulling power through a steam engine. These locomotives are fueled by burning combustible material usually coal to produce steam in a boiler. The steam moves reciprocating pistons which are mechanically connected to the locomotive's main wheels. Both fuel and water supplies are carried with the locomotive.";
        }
    }
}