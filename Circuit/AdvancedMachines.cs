using Engine;
using Engine.Graphics;

namespace Game
{
	public class Unpacker : Separator
	{
		public Unpacker()
		{
			Voltage = 100;
			Name = "Separator";
			DefaultDisplayName = DefaultDescription = "去包装机";
			Type |= ElementType.Pipe;
		}

		public override void Simulate(ref int voltage)
		{
			if (voltage > 8192 || voltage < Voltage)
				return;
			for (int i = 0; i < Component.SlotsCount; i++)
			{
				if (Component.GetSlotCount(i) > 0 && voltage<8192)
				{
					int value = Terrain.ExtractContents(Component.GetSlotValue(i));
					if (ChemicalBlock.Get(Component.GetSlotValue(i)) != null && ChemicalBlock.Get(Component.GetSlotValue(i)) is Game.Cylinder)
					{
						Component.RemoveSlotItems(i, 1);
						ComponentInventoryBase.AcquireItems(Component, ItemBlock.IdTable["钢瓶"], 1);
						return;
					}
					switch (value)
					{
						case -1:
							Component.RemoveSlotItems(i, 1);
							ComponentInventoryBase.AcquireItems(Component, EmptyBucketBlock.Index, 1);
							return;
						case WaterBucketBlock.Index:
							voltage = WaterBlock.Index << 10;
							goto case -1;
						case MagmaBucketBlock.Index:
							voltage = MagmaBlock.Index << 10;
							goto case -1;
						case PaintStripperBucketBlock.Index:
							voltage = value << 10;
							goto case -1;
						case RottenMeatBlock.Index:
							if (Terrain.ExtractData(Component.GetSlotValue(i)) >> 4 == 2)
							{
								voltage = 262384 << 10;
								goto case -1;
							}
							else if (Terrain.ExtractData(Component.GetSlotValue(i)) >> 4 == 12)
							{
								voltage = (240 | 5 << 18) << 10;
								goto case -1;
							}
							else if (Terrain.ExtractData(Component.GetSlotValue(i)) >> 4 == 13)
							{
								voltage = (240 | 3 << 18) << 10;
								goto case -1;
							}
							else if (Terrain.ExtractData(Component.GetSlotValue(i)) >> 4 == 14)
							{
								voltage = (240 | 4 << 18) << 10;
								goto case -1;
							}
							else
							{
								voltage = value << 10;
								goto case -1;
							}
					}
				}
			}
		}

		public override Widget GetWidget(IInventory inventory, ComponentSeparator component)
		{
			return new SeparatorWidget(inventory, component, "Unpacker");
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 235 : 124;
		}
	}

	public class Reductor : CubeDevice
	{
		public Reductor() : base("减压器", "减压器是一种可以降低液体或气体压力的机器，这是石油化学中必不可少的组成部分。", 100)
		{
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return 144;
		}
	}

	public class ElectricPump : CubeDevice
	{
		public ElectricPump() : base("电子泵", "电子泵是一种可以直接吸收液体于管道传输的机器", 150)
		{
			Type |= ElementType.Pipe;
		}

		public override void Simulate(ref int voltage)
		{
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 128;
		}
	}

	public class OilPlant : InventoryEntityDevice<ComponentOilPlant>
	{
		public ComponentOilPlant Inventory;
		public OilPlant() : base("石油化工厂", "石油化工厂可以用来处理石油，产生各种石油产品")
		{
			Type |= ElementType.Pipe;
		}

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Inventory = Component.Entity.FindComponent<ComponentOilPlant>(true);
		}

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 10000)
			{
				Inventory = Component.Entity.FindComponent<ComponentOilPlant>(true);
				if (ComponentInventoryBase.AcquireItems(Inventory, voltage >> 10, 1) == 0)
				{
					voltage = 0;
				}
			}
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 123 : 124 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentOilPlant component)
		{
			return new SeparatorWidget(inventory, component, "OilPlant", "Widgets/OilPlantWidget");
		}
	}



	public class Hchanger : InventoryEntityDevice<ComponentHChanger>
	{
		public ComponentHChanger Inventory;
		public Hchanger() : base("热交换器", "通过热交换让水变成蒸汽")
		{
			Type |= ElementType.Pipe;
		}

		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Inventory = Component.Entity.FindComponent<ComponentHChanger>(true);
		}

		public override void Simulate(ref int voltage)
		{
			if (voltage >= 10000)
			{
				Inventory = Component.Entity.FindComponent<ComponentHChanger>(true);
				if (ComponentInventoryBase.AcquireItems(Inventory, voltage >> 10, 1) == 0)
				{
					voltage = 0;
				}
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 138 : 140 : 141;
		}
		//return face == 4 || face == 5 ? 141 : 140;
		public override Widget GetWidget(IInventory inventory, ComponentHChanger component)
		{
			return new HchangerWidget(inventory, component);
		}
	}


	public class OilFractionalTower : InventoryEntityDevice<ComponentFractionalTower>
	{
		public OilFractionalTower() : base("石油裂解塔", "分馏塔用于通过将化学化合物加热到一个或多个化合物部分将蒸发的温度来分离化学化合物，该机器通常用于石油化学。")
		{
			Type |= ElementType.Pipe;
			Name = "OilTower";
		}

		public override void Simulate(ref int voltage)
		{
			Component.valuein = voltage;
			Component.Powered = voltage > 0 && voltage < 10000;
			if (voltage >= 10000)
			{
				ComponentFractionalTower inventory = Component.Entity.FindComponent<ComponentFractionalTower>(true);
				if (ComponentInventoryBase.AcquireItems(inventory, voltage >> 10, 1) == 0)
				{
					voltage = 0;
				}
			}
			else if (Component.value >= 10000)
			{
				voltage = Component.value;
				Component.value = 0;
			}
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? 112 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentFractionalTower component)
		{
			return new FractionalTowerWidget(inventory, component);
		}
	}

	public class ElectricDriller : InventoryEntityDevice<ComponentElectricDriller>
	{
		public ElectricDriller() : base("电子采矿机", "电子采矿机，一种电动采矿机", 310)
		{
		}
		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 119 : 124 : 107;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override Widget GetWidget(IInventory inventory, ComponentElectricDriller component)
		{
			return new ElectricDrillerWidget(inventory, component);
		}
	}


	public class VaFurnace : InventoryEntityDevice<ComponentVaFurnace>
	{
		public VaFurnace() : base("真空炉", "真空炉，一种把物品在真空中通过电加热的电器", 310)
		{
		}
		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 130 : 159 : 107;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override Widget GetWidget(IInventory inventory, ComponentVaFurnace component)
		{
			return new VaFurnaceWidget(inventory, component, "Widgets/VaFurnaceWidget");
		}
	}

	public class RControl : InventoryEntityDevice<ComponentRControl>
	{
		public RControl() : base("核反应控制器", "核反应控制器,控制燃料棒，碳棒，控制棒的及监视核反应堆的状态")
		{
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 139 : 141 : 138;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			//Component.Powered = false;
		}
		public override Widget GetWidget(IInventory inventory, ComponentRControl component)
		{
			return new RControlWidget(inventory, component);
		}
	}

	public class UThickener : InventoryEntityDevice<ComponentUThicker>
	{
		public UThickener() : base("铀浓缩机", "铀浓缩机，把粉末状的U浓缩成饼", 310)
		{
		}
		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 136: 221;
			
		}
		public override Widget GetWidget(IInventory inventory, ComponentUThicker component)
		{
			return new SeparatorWidget(inventory, component, "UThinker");
		}
	}

	public class Centrifugal : InventoryEntityDevice<ComponentCentrifugal>
	{
		public Centrifugal() : base("离心机", "离心机，一种通过高速旋转来分离物品的机器", 310)
		{
		}
		public override void Simulate(ref int voltage)
		{
			base.Simulate(ref voltage);
			Component.Powered = Powered;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 240 : 241 : 147;
		}

		public override Widget GetWidget(IInventory inventory, ComponentCentrifugal component)
		{
			return new SeparatorWidget(inventory, component, "Centrifugal");
		}
	}

	public class Ultracentrifuge : Centrifugal
	{
		public Ultracentrifuge()
		{
			DefaultDisplayName = "超速离心机";
			DefaultDescription = "超速离心机可用于分离各种细胞器";
		}
	}

	public class Workshop : InventoryEntityDevice<ComponentLargeCraftingTable>
	{
		public Workshop() : base("车间", "车间，可以制造更多物品", 0)
		{
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			//return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 240 : 147;
			return face != 4 && face != 5 ?  107: 135;
		}

		public override Widget GetWidget(IInventory inventory, ComponentLargeCraftingTable component)
		{
			return new MachineToolWidget(inventory, component, "Widgets/MachineToolWidget2");
		}
	}

	public class WaterExtractor : InventoryEntityDevice<ComponentSMachine>
	{
		public WaterExtractor() : base("重水提取机", "重水提取机，可以从蒸汽中提取重水", 200)
		{
			//DefaultDisplayName = DefaultDescription = "重水提取机";
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ?  face == (Terrain.ExtractData(value) >> 15) ? 158 : 224 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentSMachine component)
		{
			return new SeparatorWidget(inventory, component, "WaterExtractor");
		}
	}

	public class AirPump : CubeDevice
	{
		public AirPump() : base("抽气机", "抽气机", 130)
		{
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 220 : 239;
		}
	}

	public class WaterCuttingMachine : CubeDevice
	{
		public WaterCuttingMachine() : base("水切割机", "水切割机", 180)
		{
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? 209 : 241;
		}
	}

	public class LED : CubeDevice, IBlockBehavior
	{
		public BlockMesh[] m_blockMeshesByFace = new BlockMesh[6];
		public BoundingBox[][] m_collisionBoxesByFace = new BoundingBox[6][];
		public GlowPoint m_glowPoint;
		public bool LastPowered;

		public LED() : base("LED", "LED", 12)
		{
			ModelMesh modelMesh = ContentManager.Get<Model>("Models/Leds").FindMesh("OneLed");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(modelMesh.ParentBone);
			for (int i = 0; i < 6; i++)
			{
				Matrix m = i >= 4 ? (i != 4 ? (Matrix.CreateRotationX(MathUtils.PI) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(MathUtils.PI / 2f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY(i * MathUtils.PI / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				m_blockMeshesByFace[i] = new BlockMesh();
				m_blockMeshesByFace[i].AppendModelMeshPart(modelMesh.MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				m_collisionBoxesByFace[i] = new[] { m_blockMeshesByFace[i].CalculateBoundingBox() };
			}
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_blockMeshesByFace[0], color, size, ref matrix, environmentData);
		}

		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(Block, x, y, z, m_blockMeshesByFace[Terrain.ExtractData(value) >> 14 & 7], SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), null, geometry.SubsetOpaque);
		}

		public override int GetEmittedLightAmount(int value)
		{
			return ((Terrain.ExtractData(value) >> 17) & 1) * 15;
		}

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxesByFace[Terrain.ExtractData(value) >> 14 & 7];

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => face != 4;

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(value, Terrain.ExtractData(value) & -229377 | raycastResult.CellFace.Face << 14 | Index),
				CellFace = raycastResult.CellFace
			};
		}

		public void OnBlockAdded(SubsystemTerrain terrain, int value, int oldValue)
		{
			int mountingFace = Terrain.ExtractData(value) >> 14 & 7;
			Vector3 vector = CellFace.FaceToVector3(mountingFace);
			Vector3 vector2 = (mountingFace < 4) ? Vector3.UnitY : Vector3.UnitX;
			var right = Vector3.Cross(vector, vector2);
			m_glowPoint = Utils.SubsystemGlow.AddGlowPoint();
			m_glowPoint.Position = new Vector3(Point) + new Vector3(0.5f) - 0.4375f * CellFace.FaceToVector3(mountingFace);
			m_glowPoint.Forward = vector;
			m_glowPoint.Up = vector2;
			m_glowPoint.Right = right;
			m_glowPoint.Color = Color.Transparent;
			m_glowPoint.Size = 0.52f;
			m_glowPoint.FarSize = 0.52f;
			m_glowPoint.FarDistance = 1f;
			m_glowPoint.Type = GlowPointType.Square;
		}

		public void OnBlockRemoved(SubsystemTerrain terrain, int value, int newValue)
		{
			Utils.SubsystemGlow.RemoveGlowPoint(m_glowPoint);
		}

		public override void Simulate(ref int voltage)
		{
			int x = Point.X,
				y = Point.Y,
				z = Point.Z,
				value,
				v = Utils.Terrain.GetCellValueFast(x, y, z);
			base.Simulate(ref voltage);
			if (m_glowPoint == null)
				return;
			if (Powered)
			{
				
				m_glowPoint.Color = Color.White;
				value = v | 1 << 31;
				goto a;
			}
			m_glowPoint.Color = Color.Transparent;
			value = v & ~(1 << 31);
			a:
			if (Terrain.ReplaceLight(value ^ v, 0) == 0)
				return;
			Utils.Terrain.SetCellValueFast(x, y, z, value);
			TerrainChunk chunkAtCell = Utils.Terrain.GetChunkAtCell(x, z);
			if (chunkAtCell != null)
				Utils.SubsystemTerrain.TerrainUpdater.DowngradeChunkNeighborhoodState(chunkAtCell.Coords, 1, TerrainChunkState.InvalidLight, false);
		}
	}

	public class FlowCytometer : CubeDevice
	{
		public FlowCytometer() : base("流式细胞仪", "流式细胞仪可用于分析和分选细胞", 130) { }
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 220 : 239;
		}
	}
	public class Cyclotron : CubeDevice
	{
		public Cyclotron() : base("回旋加速器", "回旋加速器可用于生产人工放射性核素", -4000) { }
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? 236 : 220;
		}
	}
	public class CloudChamber : CubeDevice
	{
		public CloudChamber() : base("云室", "云室是显示能导致电离的粒子径迹的装置", 60) { }
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? 236 : 241;
		}
	}
	public class LightningCatcher : CubeDevice
	{
		public LightningCatcher() : base("雷电捕捉器", "雷电捕捉器", 8000) { }
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? 236 : 121;
		}
	}
	/*public class ElectricMicroscope : CubeDevice
	{
	}
	public class Dryer : CubeDevice
	{
		public Dryer() : base("烘干机", "烘干机", 130) { }
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 220 : 239;
		}
	}
	public abstract class Diode : Device
	{
		public int MaxVoltage;
		protected Diode() : base(ElementType.Connector)
		{
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage < 0)
			{
				if (voltage > -MaxVoltage)
					voltage = 0;
				else MaxVoltage = 0;
			}
		}
	}*/

	public class TEDC : CubeDevice, IInteractiveBlock
	{
		//public static Jint.Engine JsEngine;
		public static ComponentPlayer ComponentPlayer;

		protected static string lastcode = "";

		public TEDC() : base("晶体管数字电子计算机", "晶体管数字电子计算机", 60)
		{
			//JsEngine = new Jint.Engine();
		}

		public override int GetFaceTextureSlot(int face, int value) => face > 3 ? 109 : Powered ? 109 : 111;

		public static void Execute(string code)
		{
			/*if (string.IsNullOrWhiteSpace(code)) return;
			try
			{
				Jint.Engine engine = JsEngine.Execute(code);
				if (ComponentPlayer != null)
					ComponentPlayer.ComponentGui.DisplaySmallMessage(engine.GetCompletionValue().ToString(), false, false);
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("JS", e);
			}*/
			lastcode = code;
		}

		public bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			ComponentPlayer = componentMiner.ComponentPlayer;
			if (!Powered || ComponentPlayer == null) return false;
			DialogsManager.ShowDialog(ComponentPlayer.View.GameWidget, new TextBoxDialog("Enter Code", lastcode, int.MaxValue, Execute));
			return true;
		}
	}

	public class AirCompressor : CubeDevice
	{
		public AirCompressor() : base("空气压缩机", "空气压缩机", 60)
		{
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = Terrain.ExtractData(value) >> 15;
			return face == 4 || face == 5 ? 170 : face == direction ? 120 : face == CellFace.OppositeFace(direction) ? 110 : 170;
		}
	}
}