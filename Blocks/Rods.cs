using Engine;
using Engine.Graphics;
using System.Globalization;

namespace Game
{
	public class Rod : FlatItem
	{
		protected readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public Rod(string name, Color color, string description = null)
		{
			Model model = ContentManager.Get<Model>("Models/Rod");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("SteelRod", true).ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("SteelRod", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, color);
			if (string.IsNullOrEmpty(description))
			{
				DefaultDisplayName = char.ToUpper(name[0], CultureInfo.CurrentCulture) + name.Substring(1) + "Rod";
				DefaultDescription = "Rods are made by forging " + name + " into shape. They are useful for making many things.";
			}
			else
			{
				DefaultDisplayName = name;
				DefaultDescription = description;
			}
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(-1, 0.5f, 0);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 1.6f * size, ref matrix, environmentData);
		}
		public override float GetMeleePower(int value)
		{
			return 2f;
		}
	}
}
