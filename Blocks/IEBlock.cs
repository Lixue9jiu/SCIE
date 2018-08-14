using Engine;
using Engine.Graphics;
using System.IO;

namespace Game
{
	public abstract class IEBlock : CustomTextureBlock
	{
		public override void Initialize()
		{
			TexturePath = Path.Combine(ContentManager.Path, "IndustrialMod.png");
			base.Initialize();
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			DrawCubeBlock(primitivesRenderer, value, m_texture, new Vector3(size), ref matrix, color, color, environmentData);
		}
	}
	public abstract class IEMeshBlock : IEBlock
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, m_texture, color, 1f, ref matrix, environmentData);
		}
	}
}
