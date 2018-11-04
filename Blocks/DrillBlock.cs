using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class DrillBlock : FlatBlock, IDurability
	{
		public enum Type
		{
			SteelDrill,
			DiamondDrill,
			IronTubularis,
			SteelTubularis
		}
		public const int Index = 525;
		public override IEnumerable<int> GetCreativeValues()
		{
			var array = new int[64];
			for (int i = 0; i < 64; i++)
			{
				array[i] = Terrain.ReplaceData(Index, i >> 4 | (i & 15) << 13);
			}
			return array;
		}
		public override string GetCategory(int value)
		{
			return (Terrain.ExtractData(value) >> 13 & 15) != 0 ? "Painted" : base.GetCategory(value);
		}
		/*public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			var recipe = Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
			if (recipe != null)
			{
				for (int i = 0; i < ingredients.Length; i++)
				{
					if (!string.IsNullOrEmpty(ingredients[i]))
					{
						CraftingRecipesManager.DecodeIngredient(ingredients[i], out string craftingId, out int? data);
						if (string.Equals(craftingId, CraftingId))
						{
							recipe.ResultValue = Terrain.ReplaceData(Index, (Terrain.ExtractData(recipe.ResultValue) & 15) << 13 | (data ?? 0) & 15);
						}
					}
				}
			}
			return recipe;
		}*/
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (GetType(value))
			{
				case Type.DiamondDrill:
					color = Color.Cyan;
					break;
				case Type.SteelTubularis:
					color = Color.Gray;
					break;
			}
			CustomTextureItem.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, CustomTextureItem.Texture, color * SubsystemPalette.GetColor(environmentData, Terrain.ExtractData(value) >> 13 & 15), false, environmentData);
		}
		public static Type GetType(int value)
		{
			return (Type)(Terrain.ExtractData(value) & 0xF);
		}
		public static int SetType(int value, Type type)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -16) | ((int)type & 0xF));
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int index = Terrain.ExtractData(value) >> 13 & 15;
			return SubsystemPalette.GetName(subsystemTerrain, index == 0 ? default(int?) : index, GetType(value).ToString());
		}
		public override string GetDescription(int value)
		{
			switch (GetType(value))
			{
				default:
				//case Type.SteelDrill:
				//case Type.DiamondDrill:
					return DefaultDescription;
				case Type.IronTubularis:
				case Type.SteelTubularis:
					return "Tubularis is the most important part in the liquidpump, you should put it in the machine while using it. Reminding, pumping magma will damage the Tubularis.";
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			switch (GetType(value))
			{
				default:
				//case Type.SteelDrill:
				//case Type.DiamondDrill:
					return 211;
				case Type.IronTubularis:
				case Type.SteelTubularis:
					return 208;
			}
		}
		public int GetDurability(int value)
		{
			switch (GetType(value))
			{
				case Type.SteelDrill: return 1000;
				case Type.DiamondDrill: return 2000;
				case Type.IronTubularis: return 700;
				case Type.SteelTubularis: return 1100;
			}
			return 0;
		}
		public override int GetDamage(int value)
		{
			return base.GetDamage(value) & 2047;
		}
	}
}