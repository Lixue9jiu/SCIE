using Engine;
using Engine.Graphics;

namespace Game
{
	public class RefractoryBrick : MeshItem
	{
		public RefractoryBrick() : base("A refractory brick is a block of refractory ceramic material used in lining furnaces, kilns, advanced firebox, and fireplaces. It is bulit to withstand high temperature, but also have a low thermal conductivity for great energy efficiency.")
		{
			m_standaloneBlockMesh = Utils.CreateMesh("Models/Brick", "Brick", Matrix.CreateTranslation(0f, -0.02f, 0f) * 1.4f, Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), new Color(255, 153, 18));
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color.White, 2f * size, ref matrix, environmentData);
		}

		public override float GetMeleePower(int value)
		{
			return 2f;
		}

		public override float GetProjectilePower(int value)
		{
			return 2f;
		}
	}
}