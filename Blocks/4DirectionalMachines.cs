using Engine;
using Engine.Graphics;

namespace Game
{
	public class PresserBlock : FourDirectionalBlock
	{
		public const int Index = 509;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (GetDirection(value))
			{
				case 0: return face == 0 ? 207 : 107;
				case 1: return face == 1 ? 207 : 107;
				case 2: return face == 2 ? 207 : 107;
			}
			return face == 3 ? 207 : 107;
		}
	}
	public class KibblerBlock : FourDirectionalBlock
	{
		public const int Index = 518;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (GetDirection(value))
			{
				case 0: return face == 0 ? 208 : 107;
				case 1: return face == 1 ? 208 : 107;
				case 2: return face == 2 ? 208 : 107;
			}
			return face == 3 ? 208 : 107;
		}
	}
	public class PresserNNBlock : FourDirectionalBlock
	{
		public const int Index = 519;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (GetDirection(value))
			{
				case 0: return face == 0 ? 209 : 107;
				case 1: return face == 1 ? 209 : 107;
				case 2: return face == 2 ? 209 : 107;
			}
			return face == 3 ? 209 : 107;
		}
	}
	public class SqueezerBlock : FourDirectionalBlock
	{
		public const int Index = 527;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (GetDirection(value))
			{
				case 0: return face == 0 ? 236 : 107;
				case 1: return face == 1 ? 236 : 107;
				case 2: return face == 2 ? 236 : 107;
			}
			return face == 3 ? 236 : 107;
		}
	}
	public class CastMachBlock : FurnaceNBlock
	{
		public new const int Index = 530;

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (GetDirection(value))
			{
				case 0: return face == 0 ? 234 : 107;
				case 1: return face == 1 ? 234 : 107;
				case 2: return face == 2 ? 234 : 107;
			}
			return face == 3 ? 234 : 107;
		}
	}
	public class CReactorBlock : PaintedCubeBlock
	{
		public const int Index = 524;

		public CReactorBlock() : base(0)
		{
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 ? 192 : 107;
		}
	}
	public class BlastFurnaceBlock : FourDirectionalBlock
	{
		public const int Index = 531;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 219 : 70;
		}
	}
	public class BlastBlowerBlock : FourDirectionalBlock
	{
		public const int Index = 529;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 241;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Index,
				CellFace = raycastResult.CellFace
			};
		}
	}
	public class CovenBlock : FourDirectionalBlock
	{
		public const int Index = 533;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 242 : 39;
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, new Color(255, 153, 18) * SubsystemPalette.GetColor(generator, GetPaintColor(value)), geometry.OpaqueSubsetsByFace);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= SubsystemPalette.GetColor(environmentData, GetPaintColor(value)) * new Color(255, 153, 18);
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
	}
	public class HearthFurnaceBlock : FourDirectionalBlock
	{
		public const int Index = 534;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 243 : 107;
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, Color.LightGray * SubsystemPalette.GetColor(generator, GetPaintColor(value)), geometry.OpaqueSubsetsByFace);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= SubsystemPalette.GetColor(environmentData, GetPaintColor(value)) * Color.LightGray;
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
	}
    
}