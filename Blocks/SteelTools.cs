using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class SteelAxeBlock : AxeBlock
	{
		public const int Index = 511;
		public SteelAxeBlock() : base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(value) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(value), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(value)), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public class SteelMacheteBlock : MacheteBlock
	{
		public const int Index = 516;
		public SteelMacheteBlock() : base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(value) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(value), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(value)), 2f * size, ref matrix, environmentData);
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
			return Utils.GetColor(value) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(value), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(value)), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public class SteelShovelBlock : ShovelBlock
	{
		public const int Index = 514;
		public SteelShovelBlock() : base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(value) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(value), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(value)), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public class SteelSpearBlock : SpearBlock
	{
		public const int Index = 515;
		public SteelSpearBlock() : base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(value) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(value), DefaultDisplayName);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(value)), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public class SteelRakeBlock : RakeBlock
	{
		public const int Index = 513;
		public SteelRakeBlock() : base(47, 180)
		{
		}
		public override string GetCategory(int value)
		{
			return Utils.GetColor(value) != 0 ? "Painted" : base.GetCategory(value);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Utils.GetColor(value), DefaultDisplayName);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			return Utils.GetCreativeValues(Index);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, Utils.GetColor(value)), 2f * size, ref matrix, environmentData);
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}
	}
	public partial class Utils
	{
		public static CraftingRecipe GetAdHocCraftingRecipe(int index, SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			if (heatLevel < 1f)
			{
				return null;
			}
			int i = 0, num = 0;
			var array = new string[2];
			for (; i < ingredients.Length; i++)
			{
				if (!string.IsNullOrEmpty(ingredients[i]))
				{
					if (num > 1)
					{
						return null;
					}
					array[num] = ingredients[i];
					num++;
				}
			}
			if (num != 2)
			{
				return null;
			}
			num = 0;
			int num2 = 0;
			int num3 = 0;
			for (i = 0; i < array.Length; i++)
			{
				string item = array[i];
				CraftingRecipesManager.DecodeIngredient(item, out string craftingId, out int? data);
				int d = data ?? 0;
				if (craftingId == BlocksManager.Blocks[index].CraftingId)
				{
					num3 = Terrain.MakeBlockValue(index, 0, d);
				}
				else if (craftingId == BlocksManager.Blocks[129].CraftingId)
				{
					num = Terrain.MakeBlockValue(129, 0, d);
				}
				else if (craftingId == BlocksManager.Blocks[128].CraftingId)
				{
					num2 = Terrain.MakeBlockValue(128, 0, d);
				}
			}
			if (num != 0 && num3 != 0)
			{
				int num4 = GetColor(Terrain.ExtractData(num3));
				int color = PaintBucketBlock.GetColor(Terrain.ExtractData(num));
				int num5 = PaintBucketBlock.CombineColors(num4, color);
				if (num5 != num4)
				{
					return new CraftingRecipe
					{
						ResultCount = 1,
						ResultValue = Terrain.MakeBlockValue(index, 0, SetColor(Terrain.ExtractData(num3), num5)),
						RemainsCount = 1,
						RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(129, 0, color), BlocksManager.Blocks[129].GetDamage(num) + 1),
						RequiredHeatLevel = 1f,
						Description = "Dye tool " + SubsystemPalette.GetName(subsystemTerrain, color, null),
						Ingredients = (string[])ingredients.Clone()
					};
				}
			}
			if (num2 != 0 && num3 != 0)
			{
				if (GetColor(num3) != 0)
				{
					return new CraftingRecipe
					{
						ResultCount = 1,
						ResultValue = Terrain.MakeBlockValue(index, 0, SetColor(Terrain.ExtractData(num3), 0)),
						RemainsCount = 1,
						RemainsValue = BlocksManager.DamageItem(Terrain.MakeBlockValue(128), BlocksManager.Blocks[128].GetDamage(num2) + 1),
						RequiredHeatLevel = 1f,
						Description = "Undye tool",
						Ingredients = (string[])ingredients.Clone()
					};
				}
			}
			return null;
		}
	}
}