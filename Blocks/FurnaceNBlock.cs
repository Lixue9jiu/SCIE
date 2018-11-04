using System.Collections.Generic;

namespace Game
{
	public class FurnaceNBlock : FourDirectionalBlock
	{
		public const int Index = 506;

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == GetDirection(value) ? 191 : 107;
		}

		public override int GetEmittedLightAmount(int value)
		{
			return GetHeatLevel(value) != 0 ? 13 : 0;
		}

		public override float GetHeat(int value)
		{
			return GetHeatLevel(value) != 0 ? 0.66f : 0f;
		}

		public static int GetHeatLevel(int value)
		{
			return (Terrain.ExtractData(value) & 8) >> 3;
		}

		public static int SetHeatLevel(int data, int level)
		{
			return (data & -9) | (level & 1) << 3;
		}

		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = DestructionDebrisScale > 0f;
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.ReplaceLight(Terrain.ReplaceData(oldValue, SetDirection(SetHeatLevel(Terrain.ExtractData(oldValue), 0), 0)), 0),
				Count = 1
			});
		}
	}
}