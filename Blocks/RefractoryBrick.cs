using Engine;
using Engine.Graphics;

namespace Game
{
	public class RefractoryBrick : MeshItem
	{
		public RefractoryBrick() : base("耐火砖是一种耐火陶瓷材料，用于炉衬炉，窑炉，高级燃烧室和壁炉。 它具有耐高温性能，但导热系数低，能效高。")
		{
			m_standaloneBlockMesh = Utils.CreateMesh("Models/Brick", "Brick", Matrix.CreateTranslation(0f, -.02f, 0f) * 1.4f, Matrix.CreateTranslation(-32 % 16 / 16f, -32 / 16 / 16f, 0f), new Color(255, 153, 18));
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color.White, 2f * size, ref matrix, environmentData);
		}

		public override float GetMeleePower(int value) => 2f;

		public override float GetProjectilePower(int value) => 2f;
	}
}