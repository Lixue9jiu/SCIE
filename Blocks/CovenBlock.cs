using Engine;
using Engine.Graphics;

namespace Game
{
	public class CovenBlock : FourDirectionalBlock
	{
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == GetDirection(Terrain.ExtractData(value)))
			{
				return 242;
			}
			return 39;
		}
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
            generator.GenerateCubeVertices(this, value, x, y, z, new Color(255,153,18), geometry.OpaqueSubsetsByFace);
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, new Color(255, 153, 18), new Color(255, 153, 18), environmentData);
        }
        public const int Index = 533;
	}
}
