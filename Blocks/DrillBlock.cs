using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public enum DrillType
	{
		SteelDrill,
		DiamondDrill,
		IronTubularis,
		SteelTubularis
	}
	public class DrillBlock : FlatBlock, IDurability
	{
		public const int Index = 525;
		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[64];
			for (int i = 0; i < 64; i++)
				arr[i] = Terrain.ReplaceData(Index, i >> 4 | (i & 15) << 13);
			return arr;
		}
		public override string GetCategory(int value)
		{
			return (Terrain.ExtractData(value) >> 13 & 15) != 0 ? "Painted" : base.GetCategory(value);
		}
		/*public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			var recipe = Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
			if (recipe != null)
				for (int i = 0; i < ingredients.Length; i++)
					if (!string.IsNullOrEmpty(ingredients[i]))
					{
						CraftingRecipesManager.DecodeIngredient(ingredients[i], out string craftingId, out int? data);
						if (string.Equals(craftingId, CraftingId))
							recipe.ResultValue = Terrain.ReplaceData(Index, (Terrain.ExtractData(recipe.ResultValue) & 15) << 13 | (data ?? 0) & 15);
					}
			return recipe;
		}*/
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			var type = GetType(value);
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture,
				(type == DrillType.DiamondDrill ? Color.Cyan : type == DrillType.SteelTubularis ? color = Color.Gray : color) * SubsystemPalette.GetColor(environmentData, Terrain.ExtractData(value) >> 13 & 15), false, environmentData);
		}
		public static DrillType GetType(int value)
		{
			return (DrillType)(Terrain.ExtractData(value) & 0xF);
		}
		public static int SetType(int value, DrillType type)
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
					//case DrillType.SteelDrill:
					//case DrillType.DiamondDrill:
					return DefaultDescription;
				case DrillType.IronTubularis:
				case DrillType.SteelTubularis:
					return Utils.Get("输液管是液体泵中最重要的部分，您应该在使用它时将其放入机器中。 提醒，泵送岩浆会损坏输液管。");
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return (uint)(GetType(value) - 2) > 1u ? 211 : 208;
		}
		public int GetDurability(int value)
		{
			switch (GetType(value))
			{
				case DrillType.SteelDrill: return 1000;
				case DrillType.DiamondDrill: return 2000;
				case DrillType.IronTubularis: return 700;
				case DrillType.SteelTubularis: return 1100;
			}
			return 0;
		}
		public override int GetDamage(int value)
		{
			return base.GetDamage(value) & 2047;
		}
	}
}