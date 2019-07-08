using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class TEDC : DeviceBlock, IInteractiveBlock
	{
		public static Jint.Engine JsEngine;
		public static ComponentPlayer ComponentPlayer;
		protected static string lastcode = "";
		protected bool Powered;

		public TEDC() : base(60, "晶体管数字电子计算机", "晶体管数字电子计算机")
		{
			JsEngine = new Jint.Engine();
		}

		public override int GetFaceTextureSlot(int face, int value) => face > 3 ? 109 : Powered ? 109 : 111;

		public override void Simulate(ref int voltage) => Powered = voltage >= 60;

		public void Execute(string code)
		{
			if (string.IsNullOrWhiteSpace(code)) return;
			try
			{
				Jint.Engine engine = JsEngine.Execute(code);
				if (ComponentPlayer != null)
					ComponentPlayer.ComponentGui.DisplaySmallMessage(engine.GetCompletionValue().ToString(), false, false);
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("JS", e);
			}
			lastcode = code;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}

		public bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentPlayer = componentMiner.ComponentPlayer;
			if (!Powered || ComponentPlayer == null) return false;
			DialogsManager.ShowDialog(ComponentPlayer.View.GameWidget, new TextBoxDialog("Enter Code", lastcode, int.MaxValue, Execute));
			return true;
		}
	}

	public class Battery : DeviceBlock, IHarvestingItem, IEquatable<Battery>
	{
		public int Factor = 0;
		public readonly BoundingBox[] m_collisionBoxes;
		public readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public int RemainCount = 500;
		readonly string Id;
		public Battery(Matrix boneTransform, Matrix tcTransform, string name = "", string description = "", string id = "", int voltage = 12, string modelName = "Models/Battery", string meshName = "Battery") : base(voltage, name, description, ElementType.Connector | ElementType.Container)
		{
			m_standaloneBlockMesh.AppendMesh(modelName, meshName, boneTransform, tcTransform, Color.LightGray);
			m_collisionBoxes = new BoundingBox[] { m_standaloneBlockMesh.CalculateBoundingBox() };
			Id = id;
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			if ((Terrain.ExtractData(value) & 16384) != 0)
				RemainCount = 0;
			generator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), null, geometry.SubsetOpaque);
			WireDevice.GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), size, ref matrix, environmentData);
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue { Value = Terrain.ReplaceLight(oldValue, 0), Count = 1 });
		}
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
		public override void Simulate(ref int voltage)
		{
			if (voltage == 0 && RemainCount > 0)
			{
				voltage = Voltage;
				RemainCount--;
				return;
			}
			if (voltage >= Voltage)
			{
				voltage -= Voltage;
				RemainCount += Factor;
			}
		}
		public bool Equals(Battery other) => base.Equals(other) && Factor == other.Factor;
		public override string GetCraftingId() => Id;
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;
		public void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			if (RemainCount <= 0)
				dropValue.Value = Terrain.ReplaceData(Terrain.ReplaceLight(blockValue, 0), Terrain.ExtractData(blockValue) | 16384);
		}
	}
	public class Fridge : InteractiveEntityDevice<ComponentNewChest>
	{
		public Fridge() : base("Freezer", "冰箱", "冰箱是保护食物的好地方，可以延缓食物的腐烂。 它最多可容纳16件物品。") { }
		public override void Simulate(ref int voltage)
		{
			Component.Powered = voltage >= 110;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : face == (Terrain.ExtractData(value) >> 15) ? 106 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(0, componentMiner, value, raycastResult);
		}
		public override Widget GetWidget(IInventory inventory, ComponentNewChest component)
		{
			return new NewChestWidget(inventory, component);
		}
	}

	public class Magnetizer : InteractiveEntityDevice<ComponentMagnetizer>
	{
		public Magnetizer() : base("Magnetizer", "磁化机", "磁化机是通过在由线提供的强磁场中熔化钢锭来制造工业磁体的装置。") { }
		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 12)
				voltage -= 12;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 239 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(2, componentMiner, value, raycastResult);
		}
		public override Widget GetWidget(IInventory inventory, ComponentMagnetizer component)
		{
			return new StoveWidget(inventory, component, "Widgets/MagnetizerWidget");
		}
	}

	public class Separator : InteractiveEntityDevice<ComponentSeperator>
	{
		public Separator() : base("Seperator", "分离器", "分离器是通过高频旋转分离处材料的装置，它是离心机的缩小版本。") { }
		/*public override Device Create(Point3 p)
		{
			var other = (Separator)base.Create(p);
			return other;
		}*/
		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 120)
				voltage -= 120;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 240 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(3, componentMiner, value, raycastResult);
		}
		public override Widget GetWidget(IInventory inventory, ComponentSeperator component)
		{
			return new SeperatorWidget(inventory, component);
		}
	}

	public class UThickener : Separator
	{
		public UThickener()
		{
			DefaultDisplayName = DefaultDescription = "铀浓缩机";
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 131 : 221;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(20, componentMiner, value, raycastResult);
		}
	}

	public class Canpack : InteractiveEntityDevice<ComponentCanpack>
    {
        public Canpack() : base("Canpack", "灌装机", "灌装机是一种可以封装液体，变成液体罐头的机器") { }
        public override void Simulate(ref int voltage)
        {
            if (Component.Powered = voltage >= 80)
                voltage -= 80;
        }
        public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
        {
            base.OnBlockAdded(subsystemTerrain, value, oldValue);
            Component.Powered = false;
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 235 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            return GetPlacementValue(7, componentMiner, value, raycastResult);
        }
        public override Widget GetWidget(IInventory inventory, ComponentCanpack component)
        {
            return new CanpackWidget(inventory, component);
        }
    }

    public class Electrobath : InteractiveEntityDevice<ComponentElectrobath>
    {
        public Electrobath() : base("Electrobath", "电解机", "电解机是一种可以将电解质进行氧化还原反应的机器，在冶炼一些活泼金属上有重要的作用") { }
        public override void Simulate(ref int voltage)
        {
            if (Component.Powered = voltage >= 200)
                voltage -= 200;
        }
        public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
        {
            base.OnBlockAdded(subsystemTerrain, value, oldValue);
            Component.Powered = false;
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            return face == 4 || face == 5 ? 224 : 107;
        }
        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            return GetPlacementValue(8, componentMiner, value, raycastResult);
        }
        public override Widget GetWidget(IInventory inventory, ComponentElectrobath component)
        {
            return new SeperatorWidget(inventory, component, "Widgets/ElectrobathWidget");
        }
    }

    public class AirBlower : FixedDevice, IBlockBehavior
	{
		public int Level = -1;
		public AirBlower() : base("鼓风机", "鼓风机是一种将空气输送到需要大量热空气的大型机器中的装置。")
		{
		}
		public override void Simulate(ref int voltage)
		{
			int level;
			if (voltage < 20)
				level = 0;
			else
			{
				level = voltage < 20 ? 0 : (voltage - 20) / 40;
				voltage -= level * 40 + 20;
			}
			if (level == Level)
				return;
			var p = Point;

			//SubsystemTerrain.ChangeCell(p.X, p.Y, p.Z, , true);
			var chunk = Utils.Terrain.GetChunkAtCell(p.X, p.Z);
			if (chunk != null)
			{
				if (chunk.State < TerrainChunkState.InvalidLight)
					Level = -1;
				else
				{
					p.X &= 15; p.Z &= 15;
					int i;
					for (i = 1; i <= level && chunk.GetCellContentsFast(p.X, p.Y + i, p.Z) == 0; i++)
						chunk.SetCellValueFast(p.X, p.Y + i, p.Z, RottenMeatBlock.Index | 2 << 4 << 14);
					for (; i < 8 && p.Y < 127; i++)
						if (Terrain.ReplaceLight(chunk.GetCellValueFast(p.X, ++p.Y, p.Z), 0) == (RottenMeatBlock.Index | 2 << 4 << 14))
							chunk.SetCellValueFast(p.X, p.Y, p.Z, 0);
					Level = level;
				}
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 220;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(4, componentMiner, value, raycastResult);
		}
		public void OnBlockAdded(SubsystemTerrain terrain, int value, int oldValue)
		{
		}
		public void OnBlockRemoved(SubsystemTerrain terrain, int value, int newValue)
		{
			Level = -1;
			value = 0;
			Simulate(ref value);
		}
	}
	public class AirCompressor : FixedDevice
	{
		public AirCompressor() : base("空气压缩机", "空气压缩机")
		{
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = Terrain.ExtractData(value) >> 15;
			return face == 4 || face == 5 ? 170 : face == direction ? 120 : face == CellFace.OppositeFace(direction) ? 110 : 170;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(20, componentMiner, value, raycastResult);
		}
	}

	public class EFurnace : InteractiveEntityDevice<ComponentElectricFurnace>, IElectricElementBlock
	{
		public EFurnace() : base("ElectricFurnace", "电阻炉", "电阻炉是一种通过电阻器加热物品的装置，它可以达到很高的温度，但需要大量的热量。") { }
		/*public override Device Create(Point3 p)
		{
			var other = (EFurnace)base.Create(p);
			return other;
		}*/
		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 300)
				voltage -= 300;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 164 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(6, componentMiner, value, raycastResult);
		}
		public override Widget GetWidget(IInventory inventory, ComponentElectricFurnace component)
		{
			return new ElectricFurnaceWidget(inventory, component);
		}
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new CraftingMachineElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return face == 4 || face == 5 ? (ElectricConnectorType?)ElectricConnectorType.Input : null;
		}

		public int GetConnectionMask(int value)
		{
			int? color = PaintableItemBlock.GetColor(Terrain.ExtractData(value));
			return color.HasValue ? 1 << color.Value : 2147483647;
		}
	}
}