using Engine;
using Engine.Graphics;

namespace Game
{
	public enum MachineMode
	{
		Dispense, Shoot,
		Charge = Dispense, Discharger = Shoot
	}

	public class DiversionBlock : SixDirectionalBlock
	{
		public const int Index = 505;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), geometry.OpaqueSubsetsByFace);
		}

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
			return face == direction ? 108 : face == CellFace.OppositeFace(direction) ? 110 : 170;
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
			return face == GetDirection(value) ? 203 : 107;
		}
	}
}