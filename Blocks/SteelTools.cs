using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class SteelAxeBlock : AxeBlock
	{
		public const int Index = 511;

		public SteelAxeBlock()
			: base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(Terrain.ExtractData(value)) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(Terrain.ExtractData(value)), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(Terrain.ExtractData(value))), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public class SteelMacheteBlock : MacheteBlock
	{
		public const int Index = 516;

		public SteelMacheteBlock()
			: base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(Terrain.ExtractData(value)) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(Terrain.ExtractData(value)), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(Terrain.ExtractData(value))), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public class SteelPickaxeBlock : PickaxeBlock
	{
		public const int Index = 512;

		public SteelPickaxeBlock()
			: base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(Terrain.ExtractData(value)) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(Terrain.ExtractData(value)), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(Terrain.ExtractData(value))), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public class SteelShovelBlock : ShovelBlock
	{
		public const int Index = 514;

		public SteelShovelBlock()
			: base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(Terrain.ExtractData(value)) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(Terrain.ExtractData(value)), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(Terrain.ExtractData(value))), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public class SteelSpearBlock : SpearBlock
	{
		public const int Index = 515;

		public SteelSpearBlock()
			: base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(Terrain.ExtractData(value)) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(Terrain.ExtractData(value)), DefaultDisplayName);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(Terrain.ExtractData(value))), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public class SteelRakeBlock : RakeBlock
	{
		public const int Index = 513;

		public SteelRakeBlock()
			: base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(Terrain.ExtractData(value)) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(Terrain.ExtractData(value)), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(Terrain.ExtractData(value))), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
}
