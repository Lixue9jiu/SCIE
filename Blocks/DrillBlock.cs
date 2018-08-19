using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class DrillBlock : FlatBlock
	{
		[Serializable]
		public enum Type
		{
			SteelDrill,
			DiamondDrill,
			IronTubularis,
			SteelTubularis
		}
		public const int Index = 535;
		public override IEnumerable<int> GetCreativeValues()
		{
			if (DefaultCreativeData < 0)
			{
				return base.GetCreativeValues();
			}
			var list = new List<int>(8);
			for (int i = 0; i < 8; i++)
			{
				list.Add(Terrain.ReplaceData(Index, i));
			}
			return list;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (GetType(value))
			{
				//case Type.SteelDrill:
				//break;
				case Type.DiamondDrill:
					color = Color.Cyan;
					break;
				//case Type.IronTubularis:
				//break;
				case Type.SteelTubularis:
					color = Color.Gray;
					break;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}
		public static Type GetType(int value)
		{
			return (Type)(Terrain.ExtractData(value) & 0xF);
		}
		public static int SetType(int value, Type type)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -16) | ((int)type & 0xF));
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return GetType(value).ToString();
		}
		public override string GetDescription(int value)
		{
			switch (GetType(value))
			{
				case Type.SteelDrill:
				case Type.DiamondDrill:
					return "Drill is the most important part in the autodriller, you should put it in the machine while using it. Reminding, mining will damage the drill.";
				case Type.IronTubularis:
				case Type.SteelTubularis:
					return "Tubularis is the most important part in the liquidpump, you should put it in the machine while using it. Reminding, pumping magma will damage the Tubularis.";
			}
			return "";
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			switch (GetType(value))
			{
				case Type.SteelDrill:
				case Type.DiamondDrill:
					return 214;
				case Type.IronTubularis:
				case Type.SteelTubularis:
					return 112;
			}
			return 0;
		}
	}
}
