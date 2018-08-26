using Engine;
using Engine.Graphics;

namespace Game
{
	public class MetalIngot : BlockItem
	{
		protected BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public MetalIngot(MetalType type)
		{
			Type = type;
			DefaultDisplayName = Type.ToString() + "Ingot";
			DefaultDescription = "An ingot of pure " + Type.ToString() + ". Can be crafted into very durable and strong " + Type.ToString() + " items. Very important in the industrial Era.";
			Model model = ContentManager.Get<Model>("Models/Ingots");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("IronIngot", true).ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("IronIngot", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), false, false, false, false, Color.White);
		}
		public readonly MetalType Type;
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (Type)
			{
				case MetalType.Steel:
					color = Color.LightGray;
					break;
				case MetalType.Gold:
					color = new Color(255,215,0);
					break;
				case MetalType.Lead:
					color = new Color(88, 87, 86);
					break;
				case MetalType.Chromium:
					color = new Color(58, 57, 56);
					break;
				case MetalType.Platinum:
					color = new Color(253, 253, 253);
					break;
				default:
					color = new Color(232,232,232);
					break;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.85f;
		}
	}
}
