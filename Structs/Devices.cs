using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;
namespace Game
{
	public abstract class Generator : CircuitDevice
	{
		public int Voltage;
		public bool State;
		protected Generator(int voltage) : base(ElementType.Power)
		{
			Voltage = voltage;
		}
		public override void Simulate(ref int voltage)
		{
			if (State)
			{
				voltage += Voltage;
			}
		}
	}
	public class SmallGenerator : Generator
	{
		public SmallGenerator() : base(12)
		{
		}
	}
	public class Wire : CircuitElement
	{
		BlockMesh m_standaloneBlockMesh = ((WireBlock)BlocksManager.Blocks[WireBlock.Index]).m_standaloneBlockMesh;
		public Wire() : base(ElementType.Connector)
		{
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? paintColor = GetColor(Terrain.ExtractData(value));
			Color color2 = paintColor.HasValue ? (color * SubsystemPalette.GetColor(environmentData, paintColor)) : (1.25f * WireBlock.WireColor * color);
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color2, 2f * size, ref matrix, environmentData);
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
		public override int GetResistance(int voltage)
		{
			return 1;
		}
		public static int? GetColor(int data)
		{
			return null;
		}
		public static int SetColor(int data, int? color)
		{
			return data;
		}
	}
	public abstract class Battery : CircuitElement
	{
		public int Voltage;
		public int RemainCount;
		protected Battery(int voltage) : base(ElementType.Battery)
		{
			Voltage = voltage;
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage == 0)
			{
				voltage = Voltage;
				RemainCount--;
			}
			else if(voltage >= Voltage)
				RemainCount++;
		}
	}
	public class Battery12V : Battery
	{
		public Battery12V() : base(12)
		{
		}
	}
	public class ElectricFurnace : CircuitDevice
	{
		public ElectricFurnace() : base(ElementType.Resistor)
		{
		}
	}
	public abstract class Diode : CircuitElement
	{
		public int MaxVoltage;
		protected Diode() : base(ElementType.Connector)
		{
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage < 0)
			{
				if(voltage > -MaxVoltage)
					voltage = 0;
				else MaxVoltage = 0;
			}
		}
	}
	public class DiodeDevice : Diode
	{
	}
}
