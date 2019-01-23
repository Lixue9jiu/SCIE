using Engine;
using Engine.Graphics;

namespace Game
{
	public class SteamBoat : MeshItem
	{
		public readonly Texture2D Texture = ContentManager.Get<Texture2D>("Textures/SteamBoat");

		public SteamBoat() : base("SteamBoat allows you to cross large areas of water more safely and quickly as if you have enough fuel, a powerful vehicle during the initial industrial era.")
		{
			var model = ContentManager.Get<Model>("Models/SteamBoat");
			const string Name = "Cylinder_001";
			Matrix transform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(Name, true).ParentBone) * Matrix.CreateTranslation(0f, -0.4f, 0f);
			ModelMeshPart meshPart = model.FindMesh(Name, true).MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, false, false, false, Color.White);
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, true, false, false, Color.White);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Texture, Color.White, size, ref matrix, environmentData);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(-0.7f, 0.8f, -1);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 0.45f;
		}
	}
	public class Train : MeshItem
	{
		public readonly Texture2D Texture = ContentManager.Get<Texture2D>("Textures/Train");
		public Train() : base("A steam locomotive is a type of railway locomotive that produces its pulling power through a steam engine. These locomotives are fueled by burning combustible material usually coal to produce steam in a boiler. The steam moves reciprocating pistons which are mechanically connected to the locomotive's main wheels. Both fuel and water supplies are carried with the locomotive.")
		{
			DefaultDisplayName = "Steam Locomotive";
			var model = ContentManager.Get<Model>("Models/Train");
			const string Name = "Cylinder_003";
			Matrix transform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(Name, true).ParentBone) * Matrix.CreateTranslation(0f, -0.4f, 0f);
			ModelMeshPart meshPart = model.FindMesh(Name, true).MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, false, false, false, Color.White);
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, true, false, false, Color.White);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Texture, Color.White, size, ref matrix, environmentData);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(-0.6f, 0.6f, -0.8f);
		}
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(0f, -0.2f, 0f);
		}
	}
	public class Carriage : MeshItem
	{
		public readonly Texture2D Texture = ContentManager.Get<Texture2D>("Textures/Train");
		public Carriage() : base("Carriage")
		{
			DefaultDisplayName = "Carriage";
			var model = ContentManager.Get<Model>("Models/Train");
			const string Name = "Cylinder_003";
			Matrix transform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(Name, true).ParentBone) * Matrix.CreateTranslation(0f, -0.4f, 0f);
			ModelMeshPart meshPart = model.FindMesh(Name, true).MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, false, false, false, Color.White);
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, true, false, false, Color.White);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Texture, Color.White, size, ref matrix, environmentData);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(-0.6f, 0.6f, -0.8f);
		}
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(0f, -0.2f, 0f);
		}
	}
	public class Airship : MeshItem
	{
		//public readonly Texture2D Texture = ContentManager.Get<Texture2D>("Textures/Airship");
		public static readonly Texture2D Texture = new Texture2D(1, 1, false, ColorFormat.Rgba8888);
		static Airship()
		{
			Texture.SetData(0, new byte[] { 255, 255, 255, 255 });
		}
		public Airship() : base("A craft which uses gas lighter than the air to produce buoyancy force and make it fly, also the source of its driving power is coming from a reciprocaing engine by burning aviation gasoline.")
		{
			const string Name = "Airship";
			DefaultDisplayName = Name;
			var model = ContentManager.Get<Model>("Models/Airship");
			Matrix transform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(Name, true).ParentBone) * Matrix.CreateTranslation(0f, -0.4f, 0f) * Matrix.CreateScale(0.08f);
			ModelMeshPart meshPart = model.FindMesh(Name, true).MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, false, false, false, Color.White);
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, transform, false, true, false, false, Color.White);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, Texture, Color.White, size, ref matrix, environmentData);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(-0.6f, 0.6f, -0.8f);
		}
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(0f, -0.2f, 0f);
		}
	}
}