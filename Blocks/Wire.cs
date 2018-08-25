using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	
    public class CopperWire : Wire
    {
		public override string GetCraftingId()
		{
			return "CopperWire";
		}
    }
    public abstract class Wire : FlatItem
	{
		protected Wire()
		{
			DefaultTextureSlot = 162;
			DefaultDisplayName = "Wire";
			DefaultDescription = "Wire is a important component in electric device,especially electrical generator.";
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
		}
	}
}
