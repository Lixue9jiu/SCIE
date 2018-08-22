using Engine;
using Engine.Graphics;

namespace Game
{
	public class SteelRod : Rod
	{
		public SteelRod() : base("steel", Color.LightGray)
		{
		}
	}
	public class CopperRod : Rod
	{
		public CopperRod() : base("copper", new Color(255, 127, 80))
		{
		}
	}
	public class GoldRod : Rod
	{
		public GoldRod() : base("gold", new Color(255, 215, 0))
		{
		}
	}
	public class SliverRod : Rod
	{
		public SliverRod() : base("sliver", new Color(253, 253, 253))
		{
		}
	}
	public class LeadRod : Rod
	{
		public LeadRod() : base("lead", new Color(88, 87, 86))
		{
		}
	}
	public class PlatinumRod : Rod
	{
		public PlatinumRod() : base("platinum", new Color(253, 253, 253))
		{
		}
	}
	public class ZincRod : Rod
	{
		public ZincRod() : base("zinc", new Color(232, 232, 232))
		{
		}
	}
	public class StannaryRod : Rod
	{
		public StannaryRod() : base("stannary", new Color(232, 232, 232))
		{
		}
	}
	public class TitaniumRod : Rod
	{
		public TitaniumRod() : base("titanium", new Color(253, 253, 253))
		{
		}
	}
	public class NickelRod : Rod
	{
		public NickelRod() : base("nickel", new Color(253, 253, 253))
		{
		}
	}
	public class AluminumRod : Rod
	{
		public AluminumRod() : base("aluminum", new Color(232, 232, 232))
		{
		}
	}
	public abstract class Rod : FlatItem
	{
		protected readonly string Name;
		protected readonly Color Color;
		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public Rod(string name, Color color)
		{
			Name = name;
			Color = color;
			DefaultTextureSlot = 227;
			DefaultDescription = "Rods are made by forging " + Name + " into shape. They are useful for making many things.";
			Model model = ContentManager.Get<Model>("Models/Rod");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("SteelRod", true).ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("SteelRod", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(-1, 0.5f, 0);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Color, 1.6f * size, ref matrix, environmentData);
		}
		public override float GetMeleePower(int value)
		{
			return 2f;
		}
	}
}
