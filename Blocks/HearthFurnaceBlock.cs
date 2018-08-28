using Engine;
using Engine.Graphics;

namespace Game
{
	public class HearthFurnaceBlock : FourDirectionalBlock
	{
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == GetDirection(value))
			{
				return 243;
			}
			return 107;
		}
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
            generator.GenerateCubeVertices(this, value, x, y, z,Color.LightGray , geometry.OpaqueSubsetsByFace);
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, Color.LightGray, Color.LightGray, environmentData);
        }
        public const int Index = 534;
	}
}
