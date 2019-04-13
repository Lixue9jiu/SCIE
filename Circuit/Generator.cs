namespace Game
{//ElementType.Container | ElementType.Connector |
	public class Generator : Device, IBlockBehavior, IInteractiveBlock, IUnstableBlock
	{
		public readonly int Voltage;
		public bool Powered;
		public Generator(int voltage = 310) : base(ElementType.Supply | ElementType.Connector)
		{
			Voltage = voltage;
		}
		public override void Simulate(ref int voltage)
		{
			if (Powered)
				voltage += Voltage;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (Terrain.ExtractData(value) >> 15)
			{
				case 0:
					return face == 0 ? 145 : 107;
				case 1:
					return face == 1 ? 145 : 107;
				case 2:
					return face == 2 ? 145 : 107;
			}
					return face == 3 ? 145 : 107;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return "DirectCurrentDynamo";
		}
		public override string GetDescription(int value)
		{
			return "发电机是交流发电机产生之前的电力基础，它允许你把机械能转换成电能，这样你就可以使用很多电器了。";
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return FixedDevice.GetPlacementValue(1, componentMiner, value, raycastResult);
		}

		public static int GetDirection(int data)
		{
			return data & 7;
		}

		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}
		public void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			Powered = ComponentEngine.IsPowered(subsystemTerrain.Terrain, Point.X, Point.Y, Point.Z);
		}
		public void OnBlockRemoved(SubsystemTerrain subsystemTerrain, int value, int newValue)
		{
		}
		public void OnNeighborBlockChanged(SubsystemTerrain subsystemTerrain, int neighborX, int neighborY, int neighborZ)
		{
			Powered = ComponentEngine.IsPowered(subsystemTerrain.Terrain, Point.X, Point.Y, Point.Z);
		}
		public bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			return componentMiner.Project.FindSubsystem<SubsystemSignBlockBehavior>(true).OnInteract(raycastResult, componentMiner);
		}
	}
	/*public abstract class Diode : Element
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
	}
	public class DiodeDevice : Diode
	{
	}*/
}