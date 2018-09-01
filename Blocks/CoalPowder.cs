using Engine;
using Engine.Graphics;

namespace Game
{
	public abstract class AlloyBlock : CubeBlock
	{
		public const int Index = 517;
	}
	public class CoalPowder : OrePowder, IFuel
	{
		public readonly float HeatLevel;
		public readonly float FuelFireDuration;

		public CoalPowder(string name, Color color, float heatLevel = 1700f, float fuelFireDuration = 60f, string description = "Coalpowder is black powder obtained by crushing coal chunk. It can be used to be fuel.") : base(name, color)
		{
			DefaultDescription = description;
			HeatLevel = heatLevel;
			FuelFireDuration = fuelFireDuration;
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, Color, false, environmentData);
		}
		public float GetHeatLevel(int value)
		{
			return HeatLevel;
		}
		public float GetFuelFireDuration(int value)
		{
			return FuelFireDuration;
		}
	}
}
