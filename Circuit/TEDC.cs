using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
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
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, ItemBlock.Texture, color * SubsystemPalette.GetColor(environmentData, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), size, ref matrix, environmentData);
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
				if (RemainCount <= 0)
				{
					int x = Point.X;
					int y = Point.Y;
					int z = Point.Z;
					Utils.Terrain.SetCellValueFast(x, y, z, Utils.Terrain.GetCellValueFast(x, y, z) | 16384 << 14);
				}
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
				dropValue.Value = Terrain.ReplaceLight(blockValue, 0) | 16384 << 14;
		}
	}
	public class TEDC : DeviceBlock, IInteractiveBlock
	{
		//public static Jint.Engine JsEngine;
		public static ComponentPlayer ComponentPlayer;
		protected static string lastcode = "";
		protected bool Powered;

		public TEDC() : base(60, "晶体管数字电子计算机", "晶体管数字电子计算机")
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
	public class AirCompressor : FixedDevice
	{
		protected bool Powered;
		public AirCompressor() : base("空气压缩机", "空气压缩机") { }
		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = Terrain.ExtractData(value) >> 15;
			return face == 4 || face == 5 ? 170 : face == direction ? 120 : face == CellFace.OppositeFace(direction) ? 110 : 170;
		}
		public override void Simulate(ref int voltage) => Powered = voltage >= 60;
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(20, componentMiner, value, raycastResult);
		}
	}
}