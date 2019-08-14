using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public enum BatteryType
	{
		Cu_Zn_Battery, Lead_Battery, Fission_Battery, Fusion_Battery, Flashlight, Electric_Prod, ElectricSaw, ElectricDrill
	}

	public class IEBatteryBlock : FlatBlock, IDurability
	{
		public const int Index = 536;
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public BlockMesh m_standaloneBlockMesh2 = new BlockMesh();
		public BlockMesh m_standaloneBlockMesh3 = new BlockMesh();
		public BlockMesh m_standaloneBlockMesh4 = new BlockMesh();

		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[120];
			for (int i = 0; i < 120; i++)
				arr[i] = Terrain.ReplaceData(Index, i >> 4 | (i & 15) << 13);
			return arr;
		}

		public override void Initialize()
		{
			m_standaloneBlockMesh.AppendMesh("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateScale(.5f, .5f, 1.2f) * Matrix.CreateTranslation(0.5f, 0.5f, -0.3f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray);
			m_standaloneBlockMesh.AppendMesh("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateScale(.7f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray);
			m_standaloneBlockMesh2.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateTranslation(0, -0.5f, 0), Matrix.Identity, Color.DarkGray);
			m_standaloneBlockMesh3.AppendMesh("Models/Saw", "Saw", Matrix.CreateRotationX(1.6f) * Matrix.CreateRotationY(1.6f)* Matrix.CreateRotationY(0.8f) * Matrix.CreateRotationZ(0.8f) * Matrix.CreateTranslation(0.3f, -0.5f, 0.5f) * Matrix.CreateScale(.75f), Matrix.CreateTranslation(12f / 16f, -1f / 16f, 0f) * Matrix.CreateScale(5f), Color.White);
			m_standaloneBlockMesh4.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateRotationZ(-MathUtils.PI / 2) * Matrix.CreateTranslation(-0.5f, 0.3f, 0), Matrix.Identity, Color.White);
			m_standaloneBlockMesh4.AppendMesh("Models/Battery", "Battery", Matrix.CreateRotationZ(-MathUtils.PI / 2) * Matrix.CreateScale(.7f) * Matrix.CreateTranslation(-0.2f, 0.3f, 0.0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray);
			m_standaloneBlockMesh4.AppendMesh("Models/Battery", "Battery", Matrix.CreateScale(.5f) * Matrix.CreateTranslation(0.1f, -0.3f, 0.0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), Color.DarkGray);
			base.Initialize();
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
			if (type == BatteryType.Flashlight)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size, ref matrix, environmentData);
				return;
			}
			if (type == BatteryType.Electric_Prod)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh2, color, size, ref matrix, environmentData);
				return;
			}
			if (type == BatteryType.ElectricSaw)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh3, color, size, ref matrix, environmentData);
				return;
			}
			if (type == BatteryType.ElectricDrill)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh4, color, size, ref matrix, environmentData);
				return;
			}
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, ItemBlock.Texture,
				type == BatteryType.Fission_Battery ? Color.Cyan : type == BatteryType.Lead_Battery ? Color.Gray : color, false, environmentData);
		}

		public static BatteryType GetType(int value)
		{
			return (BatteryType)(Terrain.ExtractData(value) & 0xF);
		}

		public static int SetType(int value, BatteryType type)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -16) | ((int)type & 0xF));
		}

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

		public override int GetFaceTextureSlot(int face, int value)
		{
			return 194;
		}

		public int GetDurability(int value)
		{
			switch (GetType(value))
			{
				case BatteryType.Cu_Zn_Battery: return 800;
				case BatteryType.Fission_Battery: return 8000;
				case BatteryType.Fusion_Battery: return 40000;
				case BatteryType.Lead_Battery: return 1200;
				case BatteryType.Flashlight: return 800;
				case BatteryType.ElectricSaw: return 800;
			}
			return 300;
		}

		public override int GetDamageDestructionValue(int value)
		{
			return SetDamage(value, GetDurability(value));
		}

		public override int GetDamage(int value)
		{
			return base.GetDamage(value) & 2047;
		}

		public override float GetMeleePower(int value)
		{
			return GetType(value) == BatteryType.Electric_Prod ? MathUtils.Lerp(1f, 6f, GetDamage(value) / 300f) : DefaultMeleePower;
		}
		public override float GetMeleeHitProbability(int value)
		{
			return GetType(value) == BatteryType.Electric_Prod ? 0.8f : DefaultMeleeHitProbability;
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData) => GetType(value) == BatteryType.Flashlight ? Vector3.One : DefaultIconViewOffset;
	}
}