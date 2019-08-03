using Engine;
using Engine.Graphics;
using Game;
using System.Collections.Generic;

public class RottenMeatBlock : FluidBlock
{
	public enum Type
	{
		RottenMeat,
		Oil,
		OilBucket,
		//Updraft = OilBucket,
		LightOil,
		HeavyOil,
		Gasoline,
		D2O,
		T2O
	}
	public const int Index = 240;
	public BlockMesh m_standaloneBlockMesh;
	public BlockMesh StandaloneBlockMesh = new BlockMesh();
	public static Color[] Colors;

	public RottenMeatBlock() : base(1) { }
	public override void Initialize()
	{
		Colors = new[] { new Color(30, 30, 30), Color.White, new Color(184, 134, 11), new Color(160, 82, 45), new Color(255, 231, 186), new Color(32, 80, 224), new Color(32, 80, 224) };
		var model = ContentManager.Get<Model>("Models/FullBucket");
		var meshParts = model.FindMesh("Contents").MeshParts;
		StandaloneBlockMesh.AppendModelMeshPart(meshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents").ParentBone) * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, new Color(30, 30, 30));
		StandaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.8125f, 0.6875f, 0f));
		meshParts = model.FindMesh("Bucket").MeshParts;
		StandaloneBlockMesh.AppendModelMeshPart(meshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket").ParentBone) * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
		var rottenMeatBlock = new Game.RottenMeatBlock()
		{
			DefaultShadowStrength = -1
		};
		rottenMeatBlock.Initialize();
		m_standaloneBlockMesh = rottenMeatBlock.m_standaloneBlockMesh;
		base.Initialize();
	}
	public override IEnumerable<int> GetCreativeValues()
	{
		return new[] { Index, Index | 1 << 14 + 4, Index | 2 << 14 + 4, Index | 3 << 14 + 4, Index | 4 << 14 + 4, Index | 5 << 14 + 4, Index | 6 << 14 + 4, Index | 7 << 14 + 4 };
	}
	public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
	{
		Type type = GetType(value);
		switch (type)
		{
			case Type.RottenMeat:
				BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
				return;
			case Type.Oil:
				color = new Color(30, 30, 30);
				BlocksManager.DrawCubeBlock(primitivesRenderer, Terrain.ReplaceContents(value, 18), new Vector3(size), ref matrix, color, color, environmentData);
				return;
			case Type.OilBucket:
			case Type.D2O:
			case Type.T2O:
				BlocksManager.DrawMeshBlock(primitivesRenderer, StandaloneBlockMesh, color * Colors[(int)type - 1], 2f * size, ref matrix, environmentData);
				return;
			default:
				color = Colors[(int)type - 1];
				BlocksManager.DrawCubeBlock(primitivesRenderer, Terrain.ReplaceContents(value, 18), new Vector3(size), ref matrix, color, color, environmentData);
				return;
		}
	}
	public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
	{
		Type type = GetType(value);
		if (type < Type.Oil || type > Type.Gasoline || type == Type.OilBucket)
			return;
		Color color = Colors[(int)type - 1];
		BlocksManager.FluidBlocks[WaterBlock.Index].GenerateFluidTerrainVertices(generator, value, x, y, z, color, color, geometry.OpaqueSubsetsByFace);
	}
	public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
	{
		return new BlockPlacementData
		{
			Value = GetType(value) == 0 ? 0 : Terrain.ReplaceLight(value, 0),
			CellFace = raycastResult.CellFace
		};
	}
	public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
	{
		return Terrain.ExtractData(value) != 0 ? GetType(value).ToString() : DefaultDisplayName;
	}
	public override string GetDescription(int value)
	{
		return Terrain.ExtractData(value) != 0 ? "" : DefaultDescription;
	}
	public override string GetCategory(int value)
	{
		return GetType(value) == Type.OilBucket ? "Tools" : GetType(value) != 0 ? "Terrain" : DefaultCategory;
	}
	public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
	{
		return GetType(value) != 0 ? Vector3.One : DefaultIconViewOffset;
	}
	public static Type GetType(int value) => (Type)(Terrain.ExtractData(value) >> 4);
}