using Engine;

namespace Game
{
	public class Generator : Device, IBlockBehavior, IUnstableBehavior
	{
		public int Voltage;
		public bool Powered;
		public Generator(int voltage = 380) : base(ElementType.Container | ElementType.Connector)
		{
			Voltage = voltage;
		}
		public override void Simulate(ref int voltage)
		{
			if (Powered)
			{
				voltage += Voltage;
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 4 && face != 5)
			{
				switch (Terrain.ExtractData(value) >> 15)
				{
					case 0:
						if (face == 0)
						{
							return 145;
						}
						return 107;
					case 1:
						if (face == 1)
						{
							return 145;
						}
						return 107;
					case 2:
						if (face == 2)
						{
							return 145;
						}
						return 107;
					default:
						if (face == 3)
						{
							return 145;
						}
						return 107;
				}
			}
			return 107;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return "DirectCurrentDynamo";
		}
		public override string GetDescription(int value)
		{
			return "DirectCurrentDynamo is the fundation of the electricity before the creation of the alternating current generator, it allows you to transfer mechanical energy to electricity so that you can use many electric appliance.";
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			float max = MathUtils.Max(num, num2, num3, num4);
			if (num == max)
			{
				data = 2;
			}
			else if (num2 == max)
			{
				data = 3;
			}
			else if (num3 == max)
			{
				data = 0;
			}
			else if (num4 == max)
			{
				data = 1;
			}
			BlockPlacementData result = default(BlockPlacementData);
			result.Value = Terrain.ReplaceData(ElementBlock.Index, data << 15 | 1);
			result.CellFace = raycastResult.CellFace;
			return result;
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
