using Engine;

namespace Game
{
	public class FireBoxBlock : FourDirectionalBlock
	{
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
			{
				return 221;
			}
			switch (Terrain.ExtractData(value))
			{
			case 0:
				if (face == 0)
				{
					return 222;
				}
				return 221;
			case 1:
				if (face == 1)
				{
					return 222;
				}
				return 221;
			case 2:
				if (face == 2)
				{
					return 222;
				}
				return 221;
			default:
				if (face == 3)
				{
					return 222;
				}
				return 221;
			}
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Index, Utils.GetDirectionXZ(componentMiner)),
				CellFace = raycastResult.CellFace
			};
		}
		
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}
		
		public const int Index = 532;
		
		//protected readonly BlockMesh[] m_blockMeshesByData = new BlockMesh[4];
		
		//protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
	}
}
