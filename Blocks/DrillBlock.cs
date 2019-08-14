using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public enum DrillType
	{
		SteelDrill, DiamondDrill,
		IronTubularis, SteelTubularis
	}
	public class DrillBlock : FlatBlock, IDurability
	{
		public const int Index = 525;
		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[4];
			for (int i = 0; i < 4; i++)
				arr[i] = Terrain.ReplaceData(Index, i);
			return arr;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			var type = GetType(value);
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture, type == DrillType.DiamondDrill ? Color.Cyan : type == DrillType.SteelTubularis ? Color.Gray : color, false, environmentData);
		}
		public static DrillType GetType(int value)
		{
			return (DrillType)(Terrain.ExtractData(value) & 0xF);
		}
		public static int SetType(int value, DrillType type)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -16) | ((int)type & 0xF));
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int index = Terrain.ExtractData(value) >> 13 & 15;
			return SubsystemPalette.GetName(subsystemTerrain, index == 0 ? default(int?) : index, GetType(value).ToString());
		}
		public override string GetDescription(int value)
		{
			switch (GetType(value))
			{
				default:
					//case DrillType.SteelDrill:
					//case DrillType.DiamondDrill:
					return DefaultDescription;
				case DrillType.IronTubularis:
				case DrillType.SteelTubularis:
					return Utils.Get("输液管是液体泵中最重要的部分，您应该在使用它时将其放入机器中。 提醒，泵送岩浆会损坏输液管。");
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return (uint)(GetType(value) - 2) > 1u ? 179 : 176;
		}
		public int GetDurability(int value)
		{
			switch (GetType(value))
			{
				case DrillType.SteelDrill: return 1000;
				case DrillType.DiamondDrill: return 2000;
				case DrillType.IronTubularis: return 700;
				case DrillType.SteelTubularis: return 1100;
			}
			return 0;
		}
		public override int GetDamage(int value)
		{
			return base.GetDamage(value) & 2047;
		}
	}
}