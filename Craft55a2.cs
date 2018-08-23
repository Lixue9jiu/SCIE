using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200013B RID: 315
	public static class CraftingRecipesManager
	{
		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060009CC RID: 2508
		public static ReadOnlyList<CraftingRecipe> Recipes
		{
			get
			{
				return new ReadOnlyList<CraftingRecipe>(CraftingRecipesManager.m_recipes);
			}
		}

		// Token: 0x060009CD RID: 2509
		public static void Initialize()
		{
			foreach (XElement xelement in ContentManager.Get<XElement>("CraftingRecipes").Descendants("Recipe"))
			{
				CraftingRecipe craftingRecipe = new CraftingRecipe();
				string attributeValue = XmlUtils.GetAttributeValue<string>(xelement, "Result");
				craftingRecipe.ResultValue = CraftingRecipesManager.DecodeResult(attributeValue);
				craftingRecipe.ResultCount = XmlUtils.GetAttributeValue<int>(xelement, "ResultCount");
				string attributeValue2 = XmlUtils.GetAttributeValue<string>(xelement, "Remains", string.Empty);
				if (!string.IsNullOrEmpty(attributeValue2))
				{
					craftingRecipe.RemainsValue = CraftingRecipesManager.DecodeResult(attributeValue2);
					craftingRecipe.RemainsCount = XmlUtils.GetAttributeValue<int>(xelement, "RemainsCount");
				}
				craftingRecipe.RequiredHeatLevel = XmlUtils.GetAttributeValue<float>(xelement, "RequiredHeatLevel");
				craftingRecipe.Description = XmlUtils.GetAttributeValue<string>(xelement, "Description");
				if (craftingRecipe.ResultCount > BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.ResultValue)].MaxStacking)
				{
					throw new InvalidOperationException(string.Format("In recipe for \"{0}\" ResultCount is larger than max stacking of result block.", attributeValue));
				}
				if (craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.RemainsValue)].MaxStacking)
				{
					throw new InvalidOperationException(string.Format("In Recipe for \"{0}\" RemainsCount is larger than max stacking of remains block.", attributeValue2));
				}
				Dictionary<char, string> dictionary = new Dictionary<char, string>();
				foreach (XAttribute xattribute in from a in xelement.Attributes()
				where a.Name.LocalName.Length == 1 && char.IsLower(a.Name.LocalName[0])
				select a)
				{
					string craftingId;
					int? num;
					CraftingRecipesManager.DecodeIngredient(xattribute.Value, out craftingId, out num);
					if (BlocksManager.FindBlocksByCraftingId(craftingId).Length == 0)
					{
						throw new InvalidOperationException(string.Format("Block with craftingId \"{0}\" not found.", xattribute.Value));
					}
					if (num != null && (num.Value < 0 || num.Value > 262143))
					{
						throw new InvalidOperationException(string.Format("Data in recipe ingredient \"{0}\" must be between 0 and 0x3FFFF.", xattribute.Value));
					}
					dictionary.Add(xattribute.Name.LocalName[0], xattribute.Value);
				}
				string[] array = xelement.Value.Trim().Split(new char[]
				{
					'\n'
				});
				for (int i = 0; i < array.Length; i++)
				{
					int num2 = array[i].IndexOf('"');
					int num3 = array[i].LastIndexOf('"');
					if (num2 < 0 || num3 < 0 || num3 <= num2)
					{
						throw new InvalidOperationException("Invalid recipe line.");
					}
					string text = array[i].Substring(num2 + 1, num3 - num2 - 1);
					for (int j = 0; j < text.Length; j++)
					{
						char c = text[j];
						if (char.IsLower(c))
						{
							string text2 = dictionary[c];
							craftingRecipe.Ingredients[j + i * 6] = text2;
						}
					}
				}
				CraftingRecipesManager.m_recipes.Add(craftingRecipe);
			}
			foreach (Block block in BlocksManager.Blocks)
			{
				CraftingRecipesManager.m_recipes.AddRange(block.GetProceduralCraftingRecipes());
			}
			CraftingRecipesManager.m_recipes.Sort(delegate(CraftingRecipe r1, CraftingRecipe r2)
			{
				int y = r1.Ingredients.Count((string s) => !string.IsNullOrEmpty(s));
				int x = r2.Ingredients.Count((string s) => !string.IsNullOrEmpty(s));
				return Comparer<int>.Default.Compare(x, y);
			});
		}

		// Token: 0x060009CE RID: 2510
		public static CraftingRecipe FindMatchingRecipe(SubsystemTerrain terrain, string[] ingredients, float heatLevel)
		{
			Block[] blocks = BlocksManager.Blocks;
			for (int i = 0; i < blocks.Length; i++)
			{
				CraftingRecipe adHocCraftingRecipe = blocks[i].GetAdHocCraftingRecipe(terrain, ingredients, heatLevel);
				if (adHocCraftingRecipe != null && heatLevel >= adHocCraftingRecipe.RequiredHeatLevel && CraftingRecipesManager.MatchRecipe(adHocCraftingRecipe.Ingredients, ingredients))
				{
					return adHocCraftingRecipe;
				}
			}
			foreach (CraftingRecipe craftingRecipe in CraftingRecipesManager.Recipes)
			{
				if (heatLevel >= craftingRecipe.RequiredHeatLevel && CraftingRecipesManager.MatchRecipe(craftingRecipe.Ingredients, ingredients))
				{
					return craftingRecipe;
				}
			}
			return null;
		}

		// Token: 0x060009CF RID: 2511
		public static int DecodeResult(string result)
		{
			string[] array = result.Split(new char[]
			{
				':'
			});
			Block block = BlocksManager.FindBlockByTypeName(array[0], true);
			int data = (array.Length >= 2) ? int.Parse(array[1]) : 0;
			return Terrain.MakeBlockValue(block.BlockIndex, 0, data);
		}

		// Token: 0x060009D0 RID: 2512
		public static void DecodeIngredient(string ingredient, out string craftingId, out int? data)
		{
			string[] array = ingredient.Split(new char[]
			{
				':'
			});
			craftingId = array[0];
			data = ((array.Length >= 2) ? new int?(int.Parse(array[1])) : null);
		}

		// Token: 0x060009D1 RID: 2513
		private static bool MatchRecipe(string[] requiredIngredients, string[] actualIngredients)
		{
			string[] array = new string[36];
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j <= 4; j++)
				{
					for (int k = 0; k <= 4; k++)
					{
						bool flip = i != 0;
						if (CraftingRecipesManager.TransformRecipe(array, requiredIngredients, k, j, flip))
						{
							bool flag = true;
							for (int l = 0; l < 36; l++)
							{
								if (!CraftingRecipesManager.CompareIngredients(array[l], actualIngredients[l]))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060009D2 RID: 2514
		private static bool TransformRecipe(string[] transformedIngredients, string[] ingredients, int shiftX, int shiftY, bool flip)
		{
			for (int i = 0; i < 36; i++)
			{
				transformedIngredients[i] = null;
			}
			for (int j = 0; j < 6; j++)
			{
				for (int k = 0; k < 6; k++)
				{
					int num = (flip ? (6 - k - 1) : k) + shiftX;
					int num2 = j + shiftY;
					string text = ingredients[k + j * 6];
					if (num >= 0 && num2 >= 0 && num < 6 && num2 < 6)
					{
						transformedIngredients[num + num2 * 6] = text;
					}
					else if (!string.IsNullOrEmpty(text))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060009D3 RID: 2515
		private static bool CompareIngredients(string requiredIngredient, string actualIngredient)
		{
			if (requiredIngredient == null)
			{
				return actualIngredient == null;
			}
			if (actualIngredient == null)
			{
				return requiredIngredient == null;
			}
			string a;
			int? num;
			CraftingRecipesManager.DecodeIngredient(requiredIngredient, out a, out num);
			string b;
			int? num2;
			CraftingRecipesManager.DecodeIngredient(actualIngredient, out b, out num2);
			if (num2 == null)
			{
				throw new InvalidOperationException("Actual ingredient data not specified.");
			}
			return a == b && (num == null || num.Value == num2.Value);
		}

		// Token: 0x0400079F RID: 1951
		private static List<CraftingRecipe> m_recipes = new List<CraftingRecipe>();
	}
}
