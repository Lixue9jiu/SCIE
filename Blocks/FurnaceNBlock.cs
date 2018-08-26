using Engine;

namespace Game
{
	public class FurnaceNBlock : FourDirectionalBlock
	{
		public const int Index = 506;

		//protected readonly BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		//protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Index, Utils.GetDirectionXZ(componentMiner)),
				CellFace = raycastResult.CellFace
			};
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == GetDirection(Terrain.ExtractData(value)))
			{
				return 191;
			}
			return 107;
		}
	}
}
