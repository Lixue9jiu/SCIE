using Engine;
using Engine.Graphics;

namespace Game
{
	public class Resistor : FlatItem
	{
		public Resistor(Materials type)
		{
			DefaultTextureSlot = 163;
			var name = type.ToString();
			DefaultDisplayName = name + "Resistor";
			DefaultDescription = name + " Resistor is a kind of resistor obtained by " + name + ".";
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, new Color(55, 55, 55), false, environmentData);
		}
	}
	public class Fan : CustomTextureItem
	{
		public Fan(Materials type)
		{
			DefaultTextureSlot = 210;
			var name = type.ToString();
			DefaultDisplayName = name + "Fan";
			DefaultDescription = name + " Fan is a kind of fan made by " + name + ".";
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			DrawFlatBlock(primitivesRenderer, value, size, ref matrix, Texture, new Color(155, 155, 155), false, environmentData);
		}
	}
	public class Wire : CustomTextureItem
	{
		public readonly string CraftingId;

		public Wire(string craftingId)
		{
			CraftingId = craftingId;
			DefaultTextureSlot = 213;
			DefaultDisplayName = "Wire";
			DefaultDescription = "Wire is an important component in electric device, especially electrical generator.";
		}

		public override string GetCraftingId()
		{
			return CraftingId;
		}
	}
}