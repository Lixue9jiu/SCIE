using Engine;
using Engine.Graphics;

namespace Game
{
	public class CoalPowder : Powder, IFuel
	{
		public readonly float HeatLevel;
		public readonly float FuelFireDuration;

		public CoalPowder(string name, Color color, float heatLevel = 1700f, float fuelFireDuration = 60f, string description = "Coalpowder is black powder obtained by crushing coal chunk. It can be used to be fuel.") : base(name + "Powder", color)
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
	public class CokeCoal : OreChunk, IFuel
	{
		public CokeCoal() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(175, 175, 175), false, Materials.Steel)
		{
			DefaultDisplayName = "CokeCoal";
			DefaultDescription = "Coke Coal looks like silver chunk obtained by coking coal. It can be used to be fuel or the reductant agent in the industrial field.";
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		public override string GetCategory(int value)
		{
			return "Items";
		}

		public float GetHeatLevel(int value)
		{
			return 2000f;
		}

		public float GetFuelFireDuration(int value)
		{
			return 100f;
		}
	}
}