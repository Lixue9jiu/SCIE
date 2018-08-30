using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public abstract class AlloyBlock : CubeBlock
	{
		public const int Index = 517;
	}
	public class CoalPowder : OrePowder, IFuel
	{
		[Serializable]
        public enum Type
        {
            Coal,
            CokeCoal
        }
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
        public static Type GetType(int value)
        {
            return (Type)(Terrain.ExtractData(value) & 0xF);
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
