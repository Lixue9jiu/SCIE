using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public enum RodType
	{
		UFuelRod, PFuelRod, ControlRod, CarbonRod
	}

	public class FuelRodBlock : FlatBlock, IDurability
	{
		public const int Index = 538;
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
						 

		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[4];
			for (int i = 0; i < 4; i++)
				arr[i] = Terrain.ReplaceData(Index, i);
			return arr;
		}

		public override void Initialize()
		{
			m_standaloneBlockMesh.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(0f, -0.5f, 0f) * Matrix.CreateScale(3f, 1.6f, 3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.White);
			//m_standaloneBlockMesh.AppendMesh("Models/Battery", "Battery",  Matrix.CreateScale(.6f) * Matrix.CreateTranslation(0.0f, -0.6f, 0.0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray);
			base.Initialize();
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
			BlockMesh mesh;
			switch (type)
			{
				case RodType.UFuelRod:
					color = Color.LightGreen;
					break;
				case RodType.CarbonRod:
					color = Color.Black;
					break;
				case RodType.ControlRod:
					color = Color.LightYellow;
					break;
				case RodType.PFuelRod:
					color = Color.DarkGray;
					break;
			}
		//	ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture,
		//		type == RodType.Fission_Rod ? Color.Cyan : type == RodType.Lead_Rod ? Color.Gray : color, false, environmentData);
		//	return;
		//	a:
			mesh = m_standaloneBlockMesh;
			BlocksManager.DrawMeshBlock(primitivesRenderer, mesh, color, size, ref matrix, environmentData);

		}

		public static RodType GetType(int value)
		{
			return (RodType)(Terrain.ExtractData(value) & 0xF);
		}

		/*public static int SetType(int value, RodType type)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -16) | ((int)type & 0xF));
		}*/

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			//int index = Terrain.ExtractData(value) >> 13 & 15;
			return GetType(value).ToString();
		}

		public override string GetDescription(int value)
		{
			//switch (GetType(value))
			//{
			//	default:
			//		//case DrillType.SteelDrill:
			//		//case DrillType.DiamondDrill:
			//		return DefaultDescription;
			//	case DrillType.IronTubularis:
			//	case DrillType.SteelTubularis:
			//		return Utils.Get("输液管是液体泵中最重要的部分，您应该在使用它时将其放入机器中。 提醒，泵送岩浆会损坏输液管。");
			//}
			return DefaultDescription;
		}


		public int GetDurability(int value)
		{
			return 4096;
		}

		public override int GetDamageDestructionValue(int value)
		{
			return SetDamage(value, GetDurability(value));
		}


		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => DefaultIconViewOffset;
	}
}