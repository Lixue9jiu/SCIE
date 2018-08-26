using Engine;

namespace Game
{
	public class EngineBlock : FourDirectionalBlock
	{
		public const int Index = 504;

		//protected readonly BlockMesh[] m_blockMeshesByData = new BlockMesh[4];

		//protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 4 && face != 5)
			{
				switch (Terrain.ExtractData(value))
				{
				case 0:
					switch (face)
					{
					case 0:
						return 143;
					default:
						return 159;
					case 2:
						return 107;
					}
				case 1:
					switch (face)
					{
					case 1:
						return 143;
					default:
						return 159;
					case 3:
						return 107;
					}
				case 2:
					switch (face)
					{
					case 2:
						return 143;
					default:
						return 159;
					case 0:
						return 107;
					}
				default:
					switch (face)
					{
					case 3:
						return 143;
					default:
						return 159;
					case 1:
						return 107;
					}
				}
			}
			return 107;
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
	}
}
