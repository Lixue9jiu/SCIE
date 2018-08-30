using Engine;

namespace Game
{
	public class CokeCoal : OreChunk, IFuel
	{
		public CokeCoal() : base(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.0625f, 0.4375f, 0f), new Color(175, 175, 175), false, MetalType.Nickel)
		{
			DefaultDisplayName = "CokeCoal";
			DefaultDescription = "Coke Coal looks like silver chunk obtained by coking coal. It can be used to be fuel or the reductant agent in the industrial field.";
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
