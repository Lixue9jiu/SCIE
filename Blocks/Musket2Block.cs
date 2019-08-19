using Engine;
using Engine.Graphics;

namespace Game
{
	public class Musket2Block : FlatBlock
	{
		public enum LoadState
		{
			Empty,
			Bullet,
			Bullet2,
			Loaded,
		}

		public const int Index = 520;

		protected BlockMesh m_standaloneBlockMeshLoaded;

		protected BlockMesh m_standaloneBlockMeshUnloaded;

		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Musket");
			m_standaloneBlockMeshUnloaded = new BlockMesh();
			ReadOnlyList<ModelMeshPart> meshParts = model.FindMesh("Musket").MeshParts;
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Musket").ParentBone);
			m_standaloneBlockMeshUnloaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform, false, false, false, false, Color.Gray);
			meshParts = model.FindMesh("Hammer").MeshParts;
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Hammer").ParentBone);
			m_standaloneBlockMeshUnloaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform2, false, false, false, false, Color.Gray);
			m_standaloneBlockMeshLoaded = new BlockMesh();
			meshParts = model.FindMesh("Musket").MeshParts;
			m_standaloneBlockMeshLoaded.AppendModelMeshPart(meshParts[0], boneAbsoluteTransform, false, false, false, false, Color.Gray);
			meshParts = model.FindMesh("Hammer").MeshParts;
			m_standaloneBlockMeshLoaded.AppendModelMeshPart(meshParts[0], Matrix.CreateRotationX(0.7f) * boneAbsoluteTransform2, false, false, false, false, Color.Gray);
			base.Initialize();
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, GetHammerState(Terrain.ExtractData(value)) ? m_standaloneBlockMeshLoaded : m_standaloneBlockMeshUnloaded, color, 2f * size, ref matrix, environmentData);
		}

		public override bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			return Terrain.ExtractContents(oldValue) != Index
				? true
				: SetHammerState(Terrain.ExtractData(newValue), true) != SetHammerState(Terrain.ExtractData(oldValue), true);
		}

		public override int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) >> 10) & 255;
		}

		public override int SetDamage(int value, int damage)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & ~261120) | ((damage & 255) << 10));
		}

		public static LoadState GetLoadState(int data)
		{
			return (LoadState)(data & 3);
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
	}
}