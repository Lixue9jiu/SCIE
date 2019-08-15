namespace Game
{
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
	public class AirPump : CubeDevice
	{
		public AirPump() : base("抽气机", "抽气机", 130) { }

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 && face == (Terrain.ExtractData(value) >> 15) ? 220 : 239;
		}
	}
	public class Fuseblock : CubeDevice
	{
		public Fuseblock() : base("熔丝盒", "熔丝盒", 0) { }

		public override void Simulate(ref int voltage)
		{
			if (voltage > 700)
				voltage = 0;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return 236;
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
	public class Stabilizer : CubeDevice
	{
		public int RemainsCount;
		public Stabilizer() : base("稳压器", "稳压器", 220) { }

		public override void Simulate(ref int voltage)
		{
			if (voltage > Voltage)
			{
				RemainsCount += voltage - Voltage;
				voltage = Voltage;
			}
			else if (RemainsCount > 0)
			{
				RemainsCount -= Voltage - voltage;
				voltage = Voltage;
			}
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face != 4 && face != 5 ? 208 : 107;
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