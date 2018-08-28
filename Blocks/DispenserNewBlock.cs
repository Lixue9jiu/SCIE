using Engine;

namespace Game
{
	public class DispenserNewBlock : SixDirectionalBlock, IElectricElementBlock
	{
		public enum Mode
		{
			Dispense,
			Shoot
		}

		public const int Index = 502;

		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new DispenserNewElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return ElectricConnectorType.Input;
		}

		public int GetConnectionMask(int value)
		{
			return 2147483647;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			int direction = GetDirection(value);
			if (face == direction)
			{
				return 108;
			}
			if (face == CellFace.OppositeFace(direction))
			{
				return 110;
			}
			return 109;
		}

		public static Mode GetMode(int data)
		{
			if ((data & 8) == 0)
			{
				return Mode.Dispense;
			}
			return Mode.Shoot;
		}

		public static int SetMode(int data, Mode mode)
		{
			return (data & -9) | ((mode != 0) ? 8 : 0);
		}

		public static bool GetAcceptsDrops(int data)
		{
			return (data & 0x10) != 0;
		}

		public static int SetAcceptsDrops(int data, bool acceptsDrops)
		{
			return (data & -17) | (acceptsDrops ? 16 : 0);
		}
	}
}
