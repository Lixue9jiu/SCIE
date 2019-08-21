using Engine;
using Engine.Graphics;

namespace Game
{
	public class Musket4Block : Musket2Block
	{
		public new const int Index = 537;

		public override void Initialize()
		{
			m_standaloneBlockMeshUnloaded = new BlockMesh();
			Model model = ContentManager.Get<Model>("Models/Gun");
			ReadOnlyList<ModelMeshPart> meshParts = model.FindMesh("Musket").MeshParts;
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Musket").ParentBone);
			m_standaloneBlockMeshUnloaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform, false, false, false, false, Color.Gray);
			meshParts = model.FindMesh("Hammer").MeshParts;
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Hammer").ParentBone);
			m_standaloneBlockMeshUnloaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform2, false, false, false, false, Color.Gray);
			m_standaloneBlockMeshLoaded = new BlockMesh();
			meshParts = model.FindMesh("Musket").MeshParts;
			m_standaloneBlockMeshLoaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform, false, false, false, false, Color.DarkGray);
			meshParts = model.FindMesh("Hammer").MeshParts;
			m_standaloneBlockMeshLoaded.AppendModelMeshPart(meshParts[0], Matrix.CreateRotationX(0.7f) * boneAbsoluteTransform2, false, false, false, false, Color.Gray);
			model= ContentManager.Get<Model>("Models/Battery");
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Battery").ParentBone);
			meshParts = model.FindMesh("Battery").MeshParts;
			m_standaloneBlockMeshUnloaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform * Matrix.CreateScale(0.1f)*Matrix.CreateTranslation(0f,0.1f,0.1f), false, false, false, false, Color.Black);
			m_standaloneBlockMeshLoaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform * Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(0f, 0.1f, 0.1f), false, false, false, false, Color.Black);
			m_standaloneBlockMeshUnloaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform * Matrix.CreateRotationX(1.6f)*Matrix.CreateScale(0.1f,0.1f,0.3f) * Matrix.CreateTranslation(0f, 0.18f, -0.05f), false, false, false, false, Color.Black);
			m_standaloneBlockMeshLoaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform * Matrix.CreateRotationX(1.6f) * Matrix.CreateScale(0.1f, 0.1f, 0.3f) * Matrix.CreateTranslation(0f, 0.18f, -0.05f), false, false, false, false, Color.Black);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, GetLoadState2(Terrain.ExtractData(value)) ? m_standaloneBlockMeshLoaded : m_standaloneBlockMeshUnloaded, color, 2f * size, ref matrix, environmentData);
		}

		public override bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			return Terrain.ExtractContents(oldValue) != Index
				? true
				: SetHammerState(Terrain.ExtractData(newValue), true) != SetHammerState(Terrain.ExtractData(oldValue), true);
		}

		public static bool GetLoadState2(int data)
		{
			return data != ((data & -4) | (int)(LoadState.Empty & LoadState.Loaded));
		}

		public static int GetBulletNum(int data)
        {
			return data >> 4 & 63;
        }

		public static int SetBulletNum(int data)
        {
			return (data & ~63) | ((data & 63) << 4);
        }
	}
}