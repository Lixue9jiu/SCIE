using Engine;
using Engine.Graphics;

namespace Game
{
	public abstract class Mould : MeshItem
	{
		public readonly float Size;
		protected Mould(string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, string description, float size = 1f) : base(description)
		{
			Size = size;
			Model model = ContentManager.Get<Model>(modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(meshName, true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh(meshName, true).MeshParts[0], boneAbsoluteTransform * boneTransform, false, false, false, false, Color.LightGray);
			blockMesh.TransformTextureCoordinates(tcTransform * Matrix.CreateScale(0.05f), -1);
			m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
		}
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * Size, ref matrix, environmentData);
        }
	}
	public class SteelGear : Mould
	{
        public SteelGear() : base("Models/Gear", "Gear", Matrix.CreateTranslation(0f, 0f, 0f) * 2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "An Gear made of steel, the neccessary part of all the machine during the initial industrial era.")
		{
        }
	}
	public class SteelWheel : Mould
	{
		public SteelWheel() : base("Models/Wheel", "Wheel", Matrix.CreateTranslation(0f, 0f, 0f) * 1.2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "An wheel made of steel, the neccessary part of the steam engine train.", 2f)
		{
		}
	}
	public class SteelWheelMould : Mould
	{
		public SteelWheelMould() : base("Models/WheelMould", "WheelMould", Matrix.CreateTranslation(0f, -0.02f, 0f), Matrix.CreateTranslation(2.6f, 1.4f, 0f), "An wheel Mould made of dirt and sand, the neccessary part in making steel wheel.", 1.6f)
		{
		}
	}
	public class SteelGearMould : Mould
	{
		public SteelGearMould() : base("Models/GearMould", "GearMould", Matrix.CreateTranslation(0f, -0.02f, 0f) * 1.6f, Matrix.CreateTranslation(2.6f, 1.4f, 0f), "An Gear Mould made of dirt and sand, the neccessary part in making steel gear.")
		{
		}
	}
}
