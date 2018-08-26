using Engine;
using Engine.Graphics;

namespace Game
{
    public class Train : MeshItem
	{
		public Train() : base("A steam locomotive is a type of railway locomotive that produces its pulling power through a steam engine. These locomotives are fueled by burning combustible material usually coal to produce steam in a boiler. The steam moves reciprocating pistons which are mechanically connected to the locomotive's main wheels. Both fuel and water supplies are carried with the locomotive.")
		{
			DefaultDisplayName = "Steam Locomotive";
		}
		public override int GetFaceTextureSlot(int face, int value)
        {
            return 2;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
        {
            return Vector3.One;
        }
    }
}