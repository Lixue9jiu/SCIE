using Engine;
using Engine.Graphics;

namespace Game
{
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
		public override void Simulate(ref int voltage)
		{
			if (Powered = voltage >= 60)
				voltage -= 60;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(20, componentMiner, value, raycastResult);
		}
	}
	//public class Dryer : FixedDevice
	public class WashingMach : FixedDevice
	{
		public WashingMach() : base("洗衣机", "洗衣机") { }
		public bool Powered;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 145 : 107;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(27, componentMiner, value, raycastResult);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			color = SubsystemPalette.GetColor(environmentData, PaintableItemBlock.GetColor(Terrain.ExtractData(value)));
			ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(Block, value, x, y, z, SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), geometry.OpaqueSubsetsByFace);
		}
		public override void Simulate(ref int voltage)
		{
			if (Powered = voltage >= 130)
				voltage -= 130;
		}
	}
}