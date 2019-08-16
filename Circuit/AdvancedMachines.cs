using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	public class Unpacker : Separator
	{
		public Unpacker()
		{
			Voltage = 100;
			DefaultDisplayName = DefaultDescription = "去包装机";
			Type |= ElementType.Pipe;
		}

		public override void Simulate(ref int voltage)
		{
			if (voltage > 1023 || voltage < Voltage)
				return;
			for (int i = 0; i < Component.SlotsCount; i++)
			{
				if (Component.GetSlotCount(i) > 0)
				{
					int value = Terrain.ExtractContents(Component.GetSlotValue(i));
					switch (value)
					{
						case -1:
							Component.RemoveSlotItems(i, 1);
							ComponentInventoryBase.AcquireItems(Component, EmptyBucketBlock.Index, 1);
							voltage = value << 10;
							return;
						case WaterBucketBlock.Index:
						case MagmaBucketBlock.Index:
						case PaintStripperBucketBlock.Index:
							goto case -1;
						case RottenMeatBlock.Index:
							if (Terrain.ExtractData(Component.GetSlotValue(i)) >> 4 == 2)
								goto case -1;
							continue;
					}
				}
			}
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 235 : 124;
		}
	}

	public class ElectricPump : CubeDevice
	{
		public ElectricPump() : base("电子泵", "电子泵是一种可以直接吸收液体于管道传输的机器", 150) { Type |= ElementType.Pipe; }

		/*public override void Simulate(ref int voltage)
		{
		}*/

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 128;
		}
	}

	public class OilPlant : InventoryEntityDevice<ComponentOilPlant>
	{
		public OilPlant() : base("OilPlant", "石油化工厂", "石油化工厂可以用来处理石油，产生各种石油产品") { Type |= ElementType.Pipe; }

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 310)
				voltage -= 310;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 123 : 124 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentOilPlant component)
		{
			return new SeperatorWidget(inventory, component, "Widgets/OilPlantWidget");
		}
	}

	public class ElectricDriller : InventoryEntityDevice<ComponentElectricDriller>
	{
		public ElectricDriller() : base("ElectricDriller", "电子采矿机", "电子采矿机，一种电动采矿机") { }

		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 310)
				voltage -= 310;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? face == (Terrain.ExtractData(value) >> 15) ? 119 : 124 : 107;
		}

		public override Widget GetWidget(IInventory inventory, ComponentElectricDriller component)
		{
			return new ElectricDrillerWidget(inventory, component);
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
	}

	public class WaterExtractor : Separator
	{
		public WaterExtractor()
		{
			DefaultDisplayName = DefaultDescription = "重水提取机";
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 236 : 224;
		}
	}

	public class AirPump : CubeDevice
	{
		public AirPump() : base("抽气机", "抽气机", 130) { }

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 220 : 239;
		}
	}

	public class WaterCuttingMachine : CubeDevice
	{
		public WaterCuttingMachine() : base("水切割机", "水切割机", 180) { }

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
				Matrix m = i >= 4 ? (i != 4 ? (Matrix.CreateRotationX((float)Math.PI) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY(i * (float)Math.PI / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
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
	/*public class ElectricMicroscope : CubeDevice
	{

	}
	public class Dryer : FixedDevice
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

		public override void Simulate(ref int voltage) => Powered = voltage >= 60;

		public void Execute(string code)
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
		public AirCompressor() : base("空气压缩机", "空气压缩机") { }
		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = Terrain.ExtractData(value) >> 15;
			return face == 4 || face == 5 ? 170 : face == direction ? 120 : face == CellFace.OppositeFace(direction) ? 110 : 170;
		}
		public override void Simulate(ref int voltage)
		{
			if (Powered = voltage >= 60)
				voltage -= 60;
		}
	}
}