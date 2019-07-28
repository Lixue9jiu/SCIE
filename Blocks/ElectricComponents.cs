using Engine;
using Engine.Graphics;

namespace Game
{
	public class Resistor : FlatItem
	{
		protected readonly string Id;
		public Resistor(Materials type)
		{
			DefaultTextureSlot = 163;
			var name = type.ToStr();
			Id = type.ToString() + "Resistor";
			DefaultDisplayName = name + Utils.Get("电阻");
			DefaultDescription = name + " Resistor is a kind of resistor obtained by " + name + ".";
			Color = new Color(55, 55, 55);
		}
		public override string GetCraftingId() => Id;
	}
	public class Fan : FlatItem
	{
		protected readonly string Id;
		public Fan(Materials type)
		{
			DefaultTextureSlot = 210;
			var name = type.ToStr();
			Id = type.ToString() + "Fan";
			DefaultDisplayName = name + Utils.Get("扇");
			DefaultDescription = name + " Fan is a kind of fan made by " + name + ".";
		}
		public override string GetCraftingId() => Id;
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture, new Color(155, 155, 155), false, environmentData);
		}
	}
	public class Wire : FlatItem
	{
		public readonly string Id;

		public Wire(string craftingId)
		{
			Id = craftingId;
			DefaultTextureSlot = 213;
			DefaultDisplayName = Utils.Get("电线");
			DefaultDescription = "电线是电气设备中的重要组成部分，尤其是发电机。";
		}

		public override string GetCraftingId() => Id;
		
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture, Color.White, false, environmentData);
		}
	}
}