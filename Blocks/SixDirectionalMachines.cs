using Engine;

namespace Game
{
	public enum MachineMode
	{
		Dispense,
		Shoot
	}
	public class DiversionBlock : SixDirectionalBlock
	{
		public const int Index = 505;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 59 : 186;
		}
	}
	public class CrusherBlock : SixDirectionalBlock
	{
		public const int Index = 503;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 127 : 111;
		}
	}
	public class DrillerBlock : SixDirectionalBlock, IElectricElementBlock
	{
		public const int Index = 502;

		public new ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new DrillerElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public new ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return ElectricConnectorType.Input;
		}

		public new int GetConnectionMask(int value)
		{
			return 2147483647;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = GetDirection(value);
			return face == direction ? 108 : face == CellFace.OppositeFace(direction) ? 110 : 109;
		}

		public static MachineMode GetMode(int data)
		{
			return (MachineMode)(data >> 3 & 1);
		}

		public static int SetMode(int data, MachineMode mode)
		{
			return (data & -9) | ((mode != 0) ? 8 : 0);
		}
	}
	public class LiquidPumpBlock : SixDirectionalBlock, IElectricElementBlock
	{
		public const int Index = 526;

		public new ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new LiquidPumpElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public new ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return ElectricConnectorType.Input;
		}

		public new int GetConnectionMask(int value)
		{
			return 2147483647;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 203 : 107;
		}

		/*public static MachineMode GetMode(int data)
		{
			return (data & 8) == 0 ? MachineMode.Dispense : MachineMode.Shoot;
		}
		public static int SetMode(int data, MachineMode mode)
		{
			return (data & -9) | ((mode != 0) ? 8 : 0);
		}*/
	}
}