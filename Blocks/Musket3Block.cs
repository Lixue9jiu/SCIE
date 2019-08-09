using Engine;
using Engine.Graphics;

namespace Game
{
	public class Musket3Block : Musket2Block
	{
		public new const int Index = 538;

        public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Musket");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Musket").ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Hammer").ParentBone);
			m_standaloneBlockMeshUnloaded = new BlockMesh();
			BlockMesh standaloneBlockMeshUnloaded = m_standaloneBlockMeshUnloaded;
			ReadOnlyList<ModelMeshPart> meshParts = model.FindMesh("Musket").MeshParts;
			standaloneBlockMeshUnloaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform, false, false, false, false, Color.Gray);
			BlockMesh standaloneBlockMeshUnloaded2 = m_standaloneBlockMeshUnloaded;
			meshParts = model.FindMesh("Hammer").MeshParts;
			standaloneBlockMeshUnloaded2.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform2, false, false, false, false, Color.Gray);
			m_standaloneBlockMeshLoaded = new BlockMesh();
			BlockMesh standaloneBlockMeshLoaded = m_standaloneBlockMeshLoaded;
			meshParts = model.FindMesh("Musket").MeshParts;
			standaloneBlockMeshLoaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform, false, false, false, false, Color.DarkGray);
			BlockMesh standaloneBlockMeshLoaded2 = m_standaloneBlockMeshLoaded;
			meshParts = model.FindMesh("Hammer").MeshParts;
			standaloneBlockMeshLoaded2.AppendModelMeshPart(meshParts[0], Matrix.CreateRotationX(0.7f) * boneAbsoluteTransform2, false, false, false, false, Color.Gray);

            Model model2 = ContentManager.Get<Model>("Models/Brick");
            Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model2.FindMesh("Brick").ParentBone);

            BlockMesh standaloneBlockMeshUnloaded3 = m_standaloneBlockMeshUnloaded;
            meshParts = model2.FindMesh("Brick").MeshParts;
            standaloneBlockMeshUnloaded3.AppendModelMeshPart(meshParts[0], Matrix.CreateRotationY(1.6f) * Matrix.CreateRotationX(1.6f)* Matrix.CreateScale(0.4f) * boneAbsoluteTransform3 * Matrix.CreateTranslation(0.01f, -0.07f, 0.05f), false, false, false, false, Color.DarkGray);
            BlockMesh standaloneBlockMeshUnloaded4 = m_standaloneBlockMeshLoaded;
            meshParts = model2.FindMesh("Brick").MeshParts;
            standaloneBlockMeshUnloaded4.AppendModelMeshPart(meshParts[0], Matrix.CreateRotationY(1.6f) * Matrix.CreateRotationX(1.6f) *  Matrix.CreateScale(0.4f) * boneAbsoluteTransform3 * Matrix.CreateTranslation(0.01f, -0.07f, 0.05f), false, false, false, false, Color.DarkGray);

            //new Mould("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.5f, 0.5f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "¹¤Òµ´ÅÌú", "IndustrialMagnet"),

            base.Initialize();
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