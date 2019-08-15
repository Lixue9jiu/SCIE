using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public abstract class FourDirectionalBlock : CubeBlock, IPaintableBlock, IElectricElementBlock
	{
		protected int Front, Back;
		protected FourDirectionalBlock() { }

		protected FourDirectionalBlock(int front, int back = 107)
		{
			Front = front;
			Back = back;
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, SubsystemPalette.GetColor(generator, GetPaintColor(value)), Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color *= SubsystemPalette.GetColor(environmentData, GetPaintColor(value));
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var array = new int[17];
			array[0] = BlockIndex;
			for (int i = 1; i < 17; i++)
				array[i] = BlockIndex | SetColor(0, i - 1) << 14;
			return array;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = BlockIndex | SetDirection(Terrain.ExtractData(value), Utils.GetDirectionXZ(componentMiner)) << 14,
				CellFace = raycastResult.CellFace
			};
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = DestructionDebrisScale > 0f;
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(oldValue, 0, SetDirection(Terrain.ExtractData(oldValue), 0)),
				Count = 1
			});
		}

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}

		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, GetPaintColor(value)), GetFaceTextureSlot(0, value));
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, GetPaintColor(value), DefaultDisplayName);
		}

		public override string GetCategory(int value)
		{
			return GetPaintColor(value).HasValue ? "Painted" : Utils.Get("机器");
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == GetDirection(value) ? Front : Back;
		}

		public int? GetPaintColor(int value)
		{
			return GetColor(Terrain.ExtractData(value));
		}

		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			return Terrain.ReplaceData(value, SetColor(Terrain.ExtractData(value), color));
		}

		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new MachineElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return ElectricConnectorType.Input;
		}

		public int GetConnectionMask(int value)
		{
			int? color = GetColor(Terrain.ExtractData(value));
			return color.HasValue ? 1 << color.Value : 2147483647;
		}

		public static int? GetColor(int data)
		{
			return (data & 0x10) != 0 ? (data >> 5) & 0xF : (int?)null;
		}

		public static int SetColor(int data, int? color)
		{
			data &= -497;
			return color.HasValue ? data | 0x10 | ((color.Value & 0xF) << 5) : data;
		}

		public static int GetDirection(int value)
		{
			return Terrain.ExtractData(value) & 7;
		}

		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}
	}
	public abstract class SixDirectionalBlock : FourDirectionalBlock
	{
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(BlockIndex, SetDirection(Terrain.ExtractData(value), Utils.GetDirectionXYZ(componentMiner))),
				CellFace = raycastResult.CellFace
			};
		}

		public static bool GetAcceptsDrops(int data)
		{
			return (data & 0x10) != 0;
		}

		public static int SetAcceptsDrops(int data, bool acceptsDrops)
		{
			return (data & -17) | (acceptsDrops ? 16 : 0);
		}
	}
}