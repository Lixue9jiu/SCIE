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
			DefaultDisplayName = name + Utils.Get("����");
			DefaultDescription = name + " Resistor is a kind of resistor obtained by " + name + ".";
			Color = new Color(55, 55, 55);
		}
		public override string GetCraftingId() => Id;
	}

	public class Flat : FlatItem
	{
		protected readonly string Id;
		public Flat(string name,string des,int texture)
		{
			DefaultTextureSlot = texture;
			Id = name;
			DefaultDisplayName = name;
			DefaultDescription = des;
			Color = Color.White;
		}
		public override string GetCraftingId() => Id;
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture, Color.White, false, environmentData);
		}
	}

	public class Fan : FlatItem
	{
		protected readonly string Id;
		public Fan(Materials type)
		{
			DefaultTextureSlot = 178;
			var name = type.ToStr();
			Id = type.ToString() + "Fan";
			DefaultDisplayName = name + Utils.Get("��");
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
			DefaultDisplayName = Utils.Get("����");
			DefaultDescription = "�����ǵ����豸�е���Ҫ��ɲ��֣������Ƿ������";
		}

		public override string GetCraftingId() => Id;
		
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture, color, false, environmentData);
		}
	}
	public class Circuit : FlatItem
	{
		public readonly string Id;

		public Circuit(string craftingId, string name ,string name2 , int slot =211)
		{
			Id = craftingId;
			DefaultTextureSlot = slot;
			DefaultDisplayName = name;
			DefaultDescription = name2;
		}

		public override string GetCraftingId() => Id;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture, color, false, environmentData);
		}
	}
	/*public class Diode : MeshItem
	{
		public Diode(string name) : base(name)
		{
		}
	}*/

	public class CPanel : FlatItem
	{
		public CPanel()
		{
			DefaultTextureSlot = 109;
			DefaultDisplayName = Utils.Get("�������");
			DefaultDescription = "�������";
		}

		public override string GetCraftingId() => "CPanel";
		
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture, color, false, environmentData);
		}
	}
}