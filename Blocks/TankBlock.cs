using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class TankBlock : CubeBlock
	{
		public static readonly float[] Factors = { 0.95f, 1.05f, 1f, 0f };
		public static readonly string[] Names = { "减弱基因", "增强基因", "基因", "消除基因" };
		public const int Index = 522;
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 181 : 210;
		}

		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[(int)Trait.SetFireProbability * 4 + 1];
			int value = Index, i, j;
			for (i = 0; i <= (int)Trait.SetFireProbability; i++)
			{
				arr[i] = value;
				value += 1 << 14;
			}
			for (j = 1; j < 4; j++)
			{
				value = (1 << 22) * j | Index;
				for (i = 1; i <= (int)Trait.SetFireProbability; i++)
				{
					value += 1 << 14;
					arr[j * (int)Trait.SetFireProbability + i] = value;
				}
			}
			return arr;
		}

		public override void Initialize()
		{
			m_standaloneBlockMesh.AppendMesh("Models/Screwdriver", "obj1", Matrix.CreateRotationZ(0.5f) * Matrix.CreateTranslation(0f, -0.33f, 0f) * Matrix.CreateScale(3f), Matrix.CreateTranslation(15f / 16f, 0f, 0f) * Matrix.CreateScale(0.05f), Color.LightGray);
			base.Initialize();
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if (Terrain.ExtractData(value) == 0)
			{
				ItemBlock.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, size, ref matrix, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Utils.BlockGeometryGenerator.GenerateCubeVertices(this, value, x, y, z, Color.White, Utils.GTV(x, z, geometry).OpaqueSubsetsByFace);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return Terrain.ExtractData(value) != 0
				? default(BlockPlacementData)
				: base.GetPlacementValue(subsystemTerrain, componentMiner, value, raycastResult);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return Terrain.ExtractData(value) == 0 ? DefaultDisplayName : GetDescription(value);
		}

		public override string GetDescription(int value)
		{
			int data = Terrain.ExtractData(value);
			if (data == 0)
				return DefaultDescription;
			var trait = GetTrait(data);
			return (Utils.TR.Count < 3 ? Utils.Names[(int)trait] : trait.ToString()) + Utils.Get(Names[data >> 8]);
		}
		public override string GetCategory(int value)
		{
			return Terrain.ExtractData(value) == 0 ? DefaultCategory : Utils.Get("药物");
		}

		public static Trait GetTrait(int data)
		{
			return (Trait)((data & 255) - 1);
		}

		public static float GetFactor(int data)
		{
			return Factors[data >> 8];
		}
	}
}