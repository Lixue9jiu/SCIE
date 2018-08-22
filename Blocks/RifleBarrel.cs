using Engine;
using Engine.Graphics;

namespace Game
{
	public class RifleBarrel : Rod
	{
		public RifleBarrel() : base(string.Empty, Color.Gray)
		{
			DefaultDescription = "Rifle Barrel are made by Rifling Machine. They are useful for making guns.";
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
	}
}
