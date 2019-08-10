using Engine;
using Engine.Graphics;

namespace Game
{
	public class TEDC : FixedDevice, IInteractiveBlock
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
	public class AirCompressor : FixedDevice
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
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(20, componentMiner, value, raycastResult);
		}
	}
	public class AirPump : FixedDevice
	{
		public AirPump() : base("抽气机", "抽气机", 130) { }

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 220 : 239;
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(32, componentMiner, value, raycastResult);
		}
	}
	public class Rectifier : FixedDevice
	{
		public Rectifier() : base("整流器", "整流器", 130) { }
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 220 : 239;
		}
	}
	/*public class Dryer : FixedDevice
	{
		public Dryer() : base("烘干机", "烘干机", 130) { }
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 220 : 239;
		}
	}*/
}