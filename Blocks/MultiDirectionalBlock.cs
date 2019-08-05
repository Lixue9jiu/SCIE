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
			Vector3 forward = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			float num5 = Vector3.Dot(forward, Vector3.UnitY);
			float num6 = Vector3.Dot(forward, -Vector3.UnitY);
			float num7 = MathUtils.Min(MathUtils.Min(num, num2, num3), MathUtils.Min(num4, num5, num6));
			int direction = 0;
			if (num == num7) direction = 0;
			else if (num2 == num7) direction = 1;
			else if (num3 == num7) direction = 2;
			else if (num4 == num7) direction = 3;
			else if (num5 == num7) direction = 4;
			else if (num6 == num7) direction = 5;
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(BlockIndex, SetDirection(Terrain.ExtractData(value), direction)),
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
	public class MachineElectricElement : ElectricElement
	{
		public Point3 Point;
		protected ICraftingMachine Inventory;
		protected double m_lastDispenseTime = double.NegativeInfinity;

		public MachineElectricElement(SubsystemElectricity subsystemElectricity, Point3 point)
			: base(subsystemElectricity, new List<CellFace>
		{
			new CellFace(point.X, point.Y, point.Z, 0),
			new CellFace(point.X, point.Y, point.Z, 1),
			new CellFace(point.X, point.Y, point.Z, 2),
			new CellFace(point.X, point.Y, point.Z, 3),
			new CellFace(point.X, point.Y, point.Z, 4),
			new CellFace(point.X, point.Y, point.Z, 5)
		})
		{
			Point = point;
		}
		public override bool Simulate()
		{
			int n = 0;
			for (int i = 0; i < Connections.Count; i++)
			{
				var connection = Connections[i];
				if (connection.ConnectorType != ElectricConnectorType.Output && connection.NeighborConnectorType != 0)
				{
					n = MathUtils.Max(n, (int)MathUtils.Round(connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f));
					if (n > 7) break;
				}
			}
			if (n > 7 && SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1)
			{
				var inventory = Inventory;
				if (inventory == null)
					inventory = Utils.GetBlockEntity(Point)?.Entity.FindComponent<ICraftingMachine>();
				if (inventory == null)
					return false;
				var position = new Vector3(Point) + new Vector3(0.5f, 1f, 0.5f);
				Utils.SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.ResultSlotIndex), inventory.RemoveSlotItems(inventory.ResultSlotIndex, MathUtils.Min(inventory.GetSlotCount(inventory.ResultSlotIndex), n)), position, null, null);
				if (inventory.GetSlotCount(inventory.RemainsSlotIndex) > 0)
					Utils.SubsystemPickables.AddPickable(inventory.GetSlotValue(inventory.RemainsSlotIndex), inventory.RemoveSlotItems(inventory.RemainsSlotIndex, MathUtils.Min(inventory.GetSlotCount(inventory.RemainsSlotIndex), n)), position, null, null);
			}
			return false;
		}
	}
}