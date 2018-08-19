using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	public class IronLine : MetalLineBlock
	{
		public IronLine() : base(MetalType.IronLine)
		{
		}
	}
	public class CopperLine : MetalLineBlock
	{
		public CopperLine() : base(MetalType.CopperLine)
		{
		}
	}
	public class SteelLine : MetalLineBlock
	{
		public SteelLine() : base(MetalType.SteelLine)
		{
		}
	}
	public abstract class MetalLineBlock : FlatItem
	{
		[Serializable]
		public enum MetalType
		{
			IronLine,
			CopperLine,
			SteelLine
		}
		public readonly MetalType Type;
		protected MetalLineBlock(MetalType type)
		{
			DefaultTextureSlot = 235;
			Type = type;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (Type)
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
		public override string GetDescription(int value)
		{
			switch (Type)
			{
				case MetalType.IronLine:
					return "IronLine is made of Iron Ingot, it can be used in many place in the industrial era like heating wire.";
				case MetalType.CopperLine:
					return "CopperLine is made of Copper Ingot, it can be used in many place in the industrial era like electric wire.";
				case MetalType.SteelLine:
					return "SteelLine is made of Steel Ingot, it can be used in many place in the industrial era.";
			}
			return string.Empty;
		}
	}
}
