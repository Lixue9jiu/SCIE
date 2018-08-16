using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class MetalLineBlock : FlatBlock
	{
		[Serializable]
		public enum MetalType
		{
			Unknown,
			IronLine,
			CopperLine,
			SteelLine
		}
		public const int Index = 553;
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (GetMetalType(value))
			{
				case MetalType.CopperLine:
					color = new Color(255, 127, 80);
					break;
				case MetalType.SteelLine:
					color = new Color(192, 192, 192);
					break;
				default:
					color = Color.White;
					break;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var list = new List<int>(4);
			for (int i = 1; i < 4; i++)
			{
				list.Add(SetMetalType(Index, (MetalType)i));
			}
			return list;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return GetMetalType(value).ToString();
		}
		public override string GetDescription(int value)
		{
			switch (GetMetalType(value))
			{
				case MetalType.IronLine:
					return "IronLine is made of Iron Ingot, it can be used in many place in the industrial era like heating wire.";
				case MetalType.CopperLine:
					return "CopperLine is made of Copper Ingot, it can be used in many place in the industrial era like electric wire.";
				case MetalType.SteelLine:
					return "SteelLine is made of Steel Ingot, it can be used in many place in the industrial era.";
			}
			return DefaultDescription;
		}
		public static MetalType GetMetalType(int value)
		{
			if (Terrain.ExtractContents(value) != Index)
				return MetalType.Unknown;
			return (MetalType)Terrain.ExtractData(value);
		}
		public static int SetMetalType(int value, MetalType type)
		{
			return Terrain.ReplaceData(value, (int)type);
		}
	}
}
