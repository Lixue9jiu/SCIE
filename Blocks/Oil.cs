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
        LightOil,
        HeavyOil,
        Gasoline,
		OilBucket,
		Updraft
	}
	public const int Index = 240;
	public BlockMesh m_standaloneBlockMesh;
	public BlockMesh StandaloneBlockMesh = new BlockMesh();

	public RottenMeatBlock() : base(1) {}
	public override void Initialize()
	{
		var model = ContentManager.Get<Model>("Models/FullBucket");
		var meshParts = model.FindMesh("Contents", true).MeshParts;
		StandaloneBlockMesh.AppendModelMeshPart(meshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Contents", true).ParentBone) * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, new Color(30, 30, 30));
		StandaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.8125f, 0.6875f, 0f));
		meshParts = model.FindMesh("Bucket", true).MeshParts;
		StandaloneBlockMesh.AppendModelMeshPart(meshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Bucket", true).ParentBone) * Matrix.CreateRotationY(MathUtils.DegToRad(180f)) * Matrix.CreateTranslation(0f, -0.3f, 0f), false, false, false, false, Color.White);
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
		return new int[] { Index, Index | 1 << 4 << 14, Index | 2 << 4 << 14, Index | 3 << 4 << 14 };
	}
	public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
	{
		switch (GetType(value))
		{
			case Type.RottenMeat:
				BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
				return;
			case Type.Oil:
				color = new Color(30, 30, 30);
				BlocksManager.DrawCubeBlock(primitivesRenderer, Terrain.ReplaceContents(value, 18), new Vector3(size), ref matrix, color, color, environmentData);
				return;
            case Type.LightOil:
                color = new Color(184, 134, 11);
                BlocksManager.DrawCubeBlock(primitivesRenderer, Terrain.ReplaceContents(value, 18), new Vector3(size), ref matrix, color, color, environmentData);
                return;
            case Type.HeavyOil:
                color = new Color(160, 82, 45);
                BlocksManager.DrawCubeBlock(primitivesRenderer, Terrain.ReplaceContents(value, 18), new Vector3(size), ref matrix, color, color, environmentData);
                return;
            case Type.Gasoline:
                color = new Color(255, 231, 186);
                BlocksManager.DrawCubeBlock(primitivesRenderer, Terrain.ReplaceContents(value, 18), new Vector3(size), ref matrix, color, color, environmentData);
                return;
            case Type.OilBucket:
				BlocksManager.DrawMeshBlock(primitivesRenderer, StandaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
				return;
		}
	}
	public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
	{
		if (GetType(value) == Type.Oil)
			BlocksManager.FluidBlocks[WaterBlock.Index].GenerateFluidTerrainVertices(generator, value, x, y, z, new Color(30, 30, 30), new Color(30, 30, 30), geometry.OpaqueSubsetsByFace);
        if (GetType(value) == Type.LightOil)
            BlocksManager.FluidBlocks[WaterBlock.Index].GenerateFluidTerrainVertices(generator, value, x, y, z, new Color(184, 134, 11), new Color(184, 134, 11), geometry.OpaqueSubsetsByFace);
        if (GetType(value) == Type.HeavyOil)
            BlocksManager.FluidBlocks[WaterBlock.Index].GenerateFluidTerrainVertices(generator, value, x, y, z, new Color(160, 82, 45), new Color(160, 82, 45), geometry.OpaqueSubsetsByFace);
        if (GetType(value) == Type.Gasoline)
            BlocksManager.FluidBlocks[WaterBlock.Index].GenerateFluidTerrainVertices(generator, value, x, y, z, new Color(255, 231, 186), new Color(255, 231, 186), geometry.OpaqueSubsetsByFace);
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
		return Terrain.ExtractData(value) != 0 ? GetType(value).ToString(): DefaultDisplayName;
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
	public static Type GetType(int value)
	{
		return (Type)(Terrain.ExtractData(value) >> 4);
	}
	/*public static int SetType(int value, Type type)
	{
		return Terrain.ReplaceData(value, Terrain.ExtractData(value) & 15 | (int)type << 4);
	}*/
}