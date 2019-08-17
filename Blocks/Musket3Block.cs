using Engine;
using Engine.Graphics;

namespace Game
{
	public class Musket3Block : Musket2Block
	{
		public new const int Index = 523;

		public override void Initialize()
		{
			m_standaloneBlockMeshUnloaded = new BlockMesh();
			Model model = ContentManager.Get<Model>("Models/Musket");
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

			Model model2 = ContentManager.Get<Model>("Models/Brick");
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model2.FindMesh("Brick").ParentBone);
			meshParts = model2.FindMesh("Brick").MeshParts;
			m_standaloneBlockMeshUnloaded.AppendModelMeshPart(meshParts[0], Matrix.CreateRotationY(1.6f) * Matrix.CreateRotationX(1.6f) * Matrix.CreateScale(0.4f) * boneAbsoluteTransform3 * Matrix.CreateTranslation(0.01f, -0.07f, 0.05f), false, false, false, false, Color.DarkGray);
			meshParts = model2.FindMesh("Brick").MeshParts;
			m_standaloneBlockMeshLoaded.AppendModelMeshPart(meshParts[0], Matrix.CreateRotationY(1.6f) * Matrix.CreateRotationX(1.6f) * Matrix.CreateScale(0.4f) * boneAbsoluteTransform3 * Matrix.CreateTranslation(0.01f, -0.07f, 0.05f), false, false, false, false, Color.DarkGray);

			//new Mould("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.5f, 0.5f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "¹¤Òµ´ÅÌú", "IndustrialMagnet"),
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

		public override int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) >> 8) & 0xFF;
		}

		public override int SetDamage(int value, int damage)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -65281) | (MathUtils.Clamp(damage, 0, 255) << 8));
		}

		public static bool GetLoadState2(int data)
		{
			return data != ((data & -4) | (int)(LoadState.Empty & LoadState.Loaded));
		}

		/*public static int SetBulletNum(int data)
        {
            int num = data;
            return (data & -241) | ((num & 0xF) << 4);
        }*/
	}
}