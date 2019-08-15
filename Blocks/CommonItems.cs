using Engine;
using Engine.Graphics;
using System.Threading.Tasks;

namespace Game
{
	public class BlockItem : Item
	{
		public string DefaultDisplayName, DefaultDescription;

		public BlockItem()
		{
			DefaultDescription = DefaultDisplayName = GetType().ToString().Substring(5);
		}

		public override string GetCraftingId() => DefaultDisplayName;

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => DefaultDisplayName;

		public override string GetDescription() => DefaultDescription;
	}

	public class FlatItem : BlockItem
	{
		public Color Color = Color.White;
		public int DefaultTextureSlot;

		public override int GetFaceTextureSlot(int face, int value) => DefaultTextureSlot;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, Color * color, false, environmentData);
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3 { Z = 1 };
	}

	public class MeshItem : BlockItem
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public MeshItem(string description = "") => DefaultDescription = description;

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size, ref matrix, environmentData);
		}

		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, Color.White, null, geometry.SubsetOpaque);
		}

		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData) => 0.85f;

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;

		public MeshItem AppendMesh(string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, Color color)
		{
			m_standaloneBlockMesh.AppendMesh(modelName, meshName, boneTransform, tcTransform, color);
			return this;
		}
		/*public MeshItem Transform(Matrix matrix)
		{
			m_standaloneBlockMesh.TransformPositions(matrix);
			return this;
		}
		public MeshItem AppendMesh(BlockMesh mesh, Matrix matrix)
		{
			mesh.TransformPositions(matrix);
			m_standaloneBlockMesh.AppendBlockMesh(mesh);
			return this;
		}*/
		public static BlockMesh CreateRing()
		{
			var mesh = new BlockMesh();
			var m = Matrix.CreateScale(0.7f, 0.15f, 0.7f) * Matrix.CreateTranslation(0.5f, 0f, 0f) * Matrix.CreateRotationX(MathUtils.PI / 2);
			for (float i = 0; i < 8 * MathUtils.PI; i += MathUtils.PI / 12)
				mesh.AppendMesh("Models/Rods", "SteelRod", m * Matrix.CreateTranslation(0, 0.03f * i - 0.5f, 0) * Matrix.CreateRotationY(i), Matrix.Identity, Color.White);
			return mesh;
		}
	}

	public class Mould : MeshItem
	{
		public readonly float Size;
		public readonly BoundingBox[] m_collisionBoxes;

		public Mould(string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, Color color, float size = 1) : base("")
		{
			Size = size;
			var model = ContentManager.Get<Model>(modelName);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh(meshName).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(meshName).ParentBone) * boneTransform, this is LightMould, false, false, false, color);
			tcTransform *= Matrix.CreateScale(0.05f);
			m_standaloneBlockMesh.TransformTextureCoordinates(tcTransform);
			m_collisionBoxes = new[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public Mould(string meshName, Matrix boneTransform, Matrix tcTransform, string description = "", float size = 1) : this("Models/" + meshName, meshName, boneTransform, tcTransform, Color.LightGray, size)
		{
			DefaultDisplayName = "Steel" + meshName;
			DefaultDescription = description;
		}

		public Mould(string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, string description = "", string name = "", float size = 1) : this(modelName, meshName, boneTransform, tcTransform, Color.LightGray, size)
		{
			DefaultDisplayName = name;
			DefaultDescription = description;
		}

		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, Color.White, null, geometry.SubsetOpaque);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * Size, ref matrix, environmentData);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
	}

	public class Bucket : MeshItem
	{
		public readonly float Size;
		public readonly BoundingBox[] m_collisionBoxes;

		public Bucket(string name, Color color) : base(name)
		{
			DefaultDisplayName = name;
			var model = ContentManager.Get<Model>("Models/FullBucket");
			var meshParts = model.FindMesh("Contents").MeshParts;
			m_standaloneBlockMesh.AppendModelMeshPart(meshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents").ParentBone) * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, color);
			m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.8125f, 0.6875f, 0f));
			meshParts = model.FindMesh("Bucket").MeshParts;
			m_standaloneBlockMesh.AppendModelMeshPart(meshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket").ParentBone) * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
	}

	public partial class ItemBlock
	{
		public static Texture2D Texture;
		public static Task Task;

		public static void DrawCubeBlock(PrimitivesRenderer3D primitivesRenderer, int value, Vector3 size, ref Matrix matrix, Color color, Color topColor, DrawBlockEnvironmentData environmentData)
		{
			environmentData = environmentData ?? BlocksManager.m_defaultEnvironmentData;
			TexturedBatch3D texturedBatch3D = primitivesRenderer.TexturedBatch(Texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
			float s = LightingManager.LightIntensityByLightValue[environmentData.Light];
			color = Color.MultiplyColorOnly(color, s);
			topColor = Color.MultiplyColorOnly(topColor, s);
			Vector3 translation = matrix.Translation;
			Vector3 vector = matrix.Right * size.X;
			Vector3 v = matrix.Up * size.Y;
			Vector3 v2 = matrix.Forward * size.Z;
			Vector3 v3 = translation + 0.5f * (-vector - v - v2);
			Vector3 v4 = translation + 0.5f * (vector - v - v2);
			Vector3 v5 = translation + 0.5f * (-vector + v - v2);
			Vector3 v6 = translation + 0.5f * (vector + v - v2);
			Vector3 v7 = translation + 0.5f * (-vector - v + v2);
			Vector3 v8 = translation + 0.5f * (vector - v + v2);
			Vector3 v9 = translation + 0.5f * (-vector + v + v2);
			Vector3 v10 = translation + 0.5f * (vector + v + v2);
			if (environmentData.ViewProjectionMatrix.HasValue)
			{
				Matrix m = environmentData.ViewProjectionMatrix.Value;
				Vector3.Transform(ref v3, ref m, out v3);
				Vector3.Transform(ref v4, ref m, out v4);
				Vector3.Transform(ref v5, ref m, out v5);
				Vector3.Transform(ref v6, ref m, out v6);
				Vector3.Transform(ref v7, ref m, out v7);
				Vector3.Transform(ref v8, ref m, out v8);
				Vector3.Transform(ref v9, ref m, out v9);
				Vector3.Transform(ref v10, ref m, out v10);
			}
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			Vector4 vector2 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(0, value)];
			var color2 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(-matrix.Forward));
			texturedBatch3D.QueueQuad(v3, v5, v6, v4, new Vector2(vector2.X, vector2.W), new Vector2(vector2.X, vector2.Y), new Vector2(vector2.Z, vector2.Y), new Vector2(vector2.Z, vector2.W), color2);
			Vector4 vector3 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(2, value)];
			var color3 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(matrix.Forward));
			texturedBatch3D.QueueQuad(v7, v8, v10, v9, new Vector2(vector3.Z, vector3.W), new Vector2(vector3.X, vector3.W), new Vector2(vector3.X, vector3.Y), new Vector2(vector3.Z, vector3.Y), color3);
			Vector4 vector4 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(5, value)];
			var color4 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(-matrix.Up));
			texturedBatch3D.QueueQuad(v3, v4, v8, v7, new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.Y), new Vector2(vector4.Z, vector4.W), new Vector2(vector4.X, vector4.W), color4);
			Vector4 vector5 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(4, value)];
			var color5 = Color.MultiplyColorOnly(topColor, LightingManager.CalculateLighting(matrix.Up));
			texturedBatch3D.QueueQuad(v5, v9, v10, v6, new Vector2(vector5.X, vector5.W), new Vector2(vector5.X, vector5.Y), new Vector2(vector5.Z, vector5.Y), new Vector2(vector5.Z, vector5.W), color5);
			Vector4 vector6 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(1, value)];
			var color6 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(-matrix.Right));
			texturedBatch3D.QueueQuad(v3, v7, v9, v5, new Vector2(vector6.Z, vector6.W), new Vector2(vector6.X, vector6.W), new Vector2(vector6.X, vector6.Y), new Vector2(vector6.Z, vector6.Y), color6);
			Vector4 vector7 = BlocksManager.m_slotTexCoords[block.GetFaceTextureSlot(3, value)];
			var color7 = Color.MultiplyColorOnly(color, LightingManager.CalculateLighting(matrix.Right));
			texturedBatch3D.QueueQuad(v4, v6, v10, v8, new Vector2(vector7.X, vector7.W), new Vector2(vector7.X, vector7.Y), new Vector2(vector7.Z, vector7.Y), new Vector2(vector7.Z, vector7.W), color7);
		}

		public static void DrawFlatBlock(PrimitivesRenderer3D primitivesRenderer, int value, float size, ref Matrix matrix, Texture2D texture, Color color, bool isEmissive, DrawBlockEnvironmentData environmentData)
		{
			environmentData = environmentData ?? BlocksManager.m_defaultEnvironmentData;
			if (!isEmissive)
				color = Color.MultiplyColorOnly(color, LightingManager.LightIntensityByLightValue[environmentData.Light]);
			Vector3 translation = matrix.Translation, vector, v;
			if (environmentData.BillboardDirection.HasValue)
			{
				vector = Vector3.Normalize(Vector3.Cross(environmentData.BillboardDirection.Value, Vector3.UnitY));
				v = -Vector3.Normalize(Vector3.Cross(environmentData.BillboardDirection.Value, vector));
			}
			else
			{
				vector = matrix.Right;
				v = matrix.Up;
			}
			Vector3 v2 = translation + 0.85f * size * (-vector - v);
			Vector3 v3 = translation + 0.85f * size * (vector - v);
			Vector3 v4 = translation + 0.85f * size * (-vector + v);
			Vector3 v5 = translation + 0.85f * size * (vector + v);
			if (environmentData.ViewProjectionMatrix.HasValue)
			{
				Matrix m = environmentData.ViewProjectionMatrix.Value;
				Vector3.Transform(ref v2, ref m, out v2);
				Vector3.Transform(ref v3, ref m, out v3);
				Vector3.Transform(ref v4, ref m, out v4);
				Vector3.Transform(ref v5, ref m, out v5);
			}
			Vector4 vector2 = BlocksManager.m_slotTexCoords[BlocksManager.Blocks[Terrain.ExtractContents(value)].GetFaceTextureSlot(-1, value)];
			if (texture == null)
				texture = (environmentData.SubsystemTerrain != null) ? environmentData.SubsystemTerrain.SubsystemAnimatedTextures.AnimatedBlocksTexture : BlocksTexturesManager.DefaultBlocksTexture;
			TexturedBatch3D texturedBatch3D = primitivesRenderer.TexturedBatch(texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
			texturedBatch3D.QueueQuad(v2, v4, v5, v3, new Vector2(vector2.X, vector2.W), new Vector2(vector2.X, vector2.Y), new Vector2(vector2.Z, vector2.Y), new Vector2(vector2.Z, vector2.W), color);
			if (!environmentData.BillboardDirection.HasValue)
				texturedBatch3D.QueueQuad(v2, v3, v5, v4, new Vector2(vector2.X, vector2.W), new Vector2(vector2.Z, vector2.W), new Vector2(vector2.Z, vector2.Y), new Vector2(vector2.X, vector2.Y), color);
		}
	}
}