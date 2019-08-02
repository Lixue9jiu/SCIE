using Engine;
using Engine.Graphics;

namespace Game
{
	public class Musket3Block : FlatBlock
	{
		public enum LoadState
		{
			Empty,
			bullet,
            bullet2,
			Loaded
		}

		public const int Index = 538;

		protected BlockMesh m_standaloneBlockMeshLoaded;

		protected BlockMesh m_standaloneBlockMeshUnloaded;


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

            Model model2 = ContentManager.Get<Model>("Models/Battery");
            Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model2.FindMesh("Battery").ParentBone);

          //  BlockMesh standaloneBlockMeshUnloaded3 = m_standaloneBlockMeshUnloaded;
          //  meshParts = model2.FindMesh("Battery").MeshParts;
          //  standaloneBlockMeshUnloaded3.AppendModelMeshPart(meshParts[0], Matrix.CreateRotationX(-1.6f) * Matrix.CreateScale(0.3f)* boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, -0.08f, +0.15f), false, false, false, false, Color.Gray);
            BlockMesh standaloneBlockMeshUnloaded4 = m_standaloneBlockMeshLoaded;
            meshParts = model2.FindMesh("Battery").MeshParts;
            standaloneBlockMeshUnloaded4.AppendModelMeshPart(meshParts[0], Matrix.CreateRotationX(-1.6f) * Matrix.CreateScale(0.3f) * boneAbsoluteTransform3 * Matrix.CreateTranslation(0f, -0.08f, +0.15f), false, false, false, false, Color.Gray);

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

		public static LoadState GetLoadState(int data)
		{
			return (LoadState)(data & 3);
		}

        public static bool GetLoadState2(int data)
        {
            
            return data != ((data & -4) | (int)(LoadState.Empty & LoadState.Loaded));
        }

        public static int SetLoadState(int data, LoadState loadState)
		{
			return (data & -4) | (int)(loadState & LoadState.Loaded);
		}

		public static bool GetHammerState(int data)
		{
			return (data & 4) != 0;
		}

		public static int SetHammerState(int data, bool state)
		{
			return (data & -5) | ((state ? 1 : 0) << 2);
		}

		/*public static Bullet2Block.BulletType? GetBulletType(int data)
		{
			int num = (data >> 4) & 0xF;
			if (num != 0)
				return (Bullet2Block.BulletType)(num - 1);
			return null;
		}*/

		public static int SetBulletType(int data, Bullet2Block.BulletType? bulletType)
		{
			int num = (int)(bulletType.HasValue ? (bulletType.Value + 1) : Bullet2Block.BulletType.IronBullet);
			return (data & -241) | ((num & 0xF) << 4);
		}
        public static int SetBulletNum(int data)
        {
            int num = (int)(data);
            return (data & -241) | ((num & 0xF) << 4);
        }
    }
}