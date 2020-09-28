﻿using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using System.Linq;
using System.Globalization;
using System.Xml.Linq;
using XmlUtilities;
using System;
using Engine.Media;
using Engine.Audio;
using System.Threading.Tasks;

namespace Game
{
	[PluginLoader("IndustrialMod", "", 0u)]
	public class Item : IItem
	{
		public static ElementBlock Block;
		public static Task Task;
		public static Image[] Images;
		public static Func<string, int> DecodeResult1;
		static void Initialize()
		{
			
			BlocksManager.DamageItem1 = DamageItem;
			BlocksManager.FindBlocksByCraftingId1 = FindBlocksByCraftingId;
			BlocksManager.FindBlockByTypeName1 = FindBlockByTypeName;
			CraftingRecipesManager.Initialize1 = (Action)Delegate.Combine(new Action(CRInitialize), CraftingRecipesManager.Initialize1);
			DecodeResult1 = CraftingRecipesManager.DecodeResult1;
			CraftingRecipesManager.DecodeResult1 = DecodeResult;
			CraftingRecipesManager.MatchRecipe1 = MatchRecipe;
			CraftingRecipesManager.TransformRecipe1 = TransformRecipe;
			DebugCamera.get_IsEntityControlEnabled1 = True;
			FlyCamera.get_IsEntityControlEnabled1 = True;
			RandomJumpCamera.get_IsEntityControlEnabled1 = True;
			StraightFlightCamera.get_IsEntityControlEnabled1 = True;
			AudioManager.PlaySound1 = PlaySound;
			ScreensManager.Initialized += Init;
			Utils.TR = new Dictionary<string, string>();
			
		}
		internal static void Init()
		{
			SteamBoat.BoatTexture = ContentManager.Get<Texture2D>("Textures/SteamBoat");
			Train.TrainTexture = ContentManager.Get<Texture2D>("Textures/Train");
			Carriage.TrainTexture = ContentManager.Get<Texture2D>("Textures/Creatures/Jaguar");
			Airship.ATexture = ContentManager.Get<Texture2D>("Textures/tex");
			Car.CarTexture = ContentManager.Get<Texture2D>("Textures/tex2");
			ETrain.ETrainTexture = ContentManager.Get<Texture2D>("Textures/etex3");
			Airplane.PTexture = ContentManager.Get<Texture2D>("Textures/tex4");
			Rocket.RocketTexture = ContentManager.Get<Texture2D>("Textures/Rocket");
			Submarine.SubmarineTexture = ContentManager.Get<Texture2D>("Textures/QST");
			var stream = Utils.GetTargetFile("IndustrialMod.png");
			try
			{
				ItemBlock.Texture = Texture2D.Load(stream);
			}
			finally { stream.Close(); }
			if (Task == null)
			{
				Task = Task.Run((Action)ItemBlock.InitItems);
			}
			((LoadingScreen)ScreensManager.CurrentScreen).AddLoadAction(SR);
		}
		public static bool True(object obj) => true;
		public static void PlaySound(string name, float volume, float pitch, float pan)
		{
			if (SettingsManager.SoundsVolume > 0f)
			{
				volume *= SettingsManager.SoundsVolume;
				if (volume > AudioManager.MinAudibleVolume)
					try
					{
						object obj = ContentManager.Get(name);
						if (obj is SoundBuffer buffer)
							new Sound(buffer, volume, AudioManager.ToEnginePitch(pitch), pan, false, true).Play();
						else if (obj is BaseSound sound && sound != null)
							sound.Play();
						else if (obj is StreamingSource source && false)
							new StreamingSound(source, volume, AudioManager.ToEnginePitch(pitch), pan, false, true, 0f).Play();
					}
					catch { }
			}
		}
		public static Block[] FindBlocksByCraftingId(string craftingId)
		{
			if (ItemBlock.IdTable.TryGetValue(craftingId, out int value))
				return new[] { BlocksManager.Blocks[ItemBlock.Index] };
			var c__DisplayClass = new BlocksManager.c__DisplayClass14_0 { craftingId = craftingId };
			return BlocksManager.Blocks.Where(c__DisplayClass.FindBlocksByCraftingId_b__0).ToArray();
		}
		public static Block FindBlockByTypeName(string typeName, bool throwIfNotFound)
		{
			if (ItemBlock.IdTable.TryGetValue(typeName, out int value))
				return BlocksManager.Blocks[ItemBlock.Index] ;
			BlocksManager.c__DisplayClass13_0 c__DisplayClass13_ = new BlocksManager.c__DisplayClass13_0 { typeName = typeName };
			c__DisplayClass13_.typeName = typeName;
			Block block = BlocksManager.Blocks.FirstOrDefault(c__DisplayClass13_.FindBlockByTypeName_b__0);
			if (block == null && throwIfNotFound)
			{
				throw new InvalidOperationException($"Block with type {c__DisplayClass13_.typeName} not found.");
			}
			return block;
		}

		public static int DamageItem(int value, int damageCount)
		{
			var block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			if (block.Durability < 0)
				return value;
			damageCount += block.GetDamage(value);
			return damageCount <= (block is IDurability item ? item.GetDurability(value) : block.Durability)
				? block.SetDamage(value, damageCount)
				: block.GetDamageDestructionValue(value);
		}
		public static void SR()
		{
			ScreensManager.m_screens["Recipaedia"] = new RecipaediaScreen2();
		}
		public static void CRInitialize()
		{
			CraftingRecipesManager.m_recipes = new List<CraftingRecipe>();
			var recipes = new List<CraftingRecipe>();
			var enumerator = ContentManager.CombineXml(ContentManager.Get<XElement>("CraftingRecipes"), ModsManager.GetEntries(".cr"), "Description", "Result", "Recipes").Descendants("Recipe").GetEnumerator();
			while (enumerator.MoveNext())
			{
				XElement xelement = enumerator.Current;
				var craftingRecipe = new CraftingRecipe { Ingredients = new string[36] };
				string attributeValue = XmlUtils.GetAttributeValue<string>(xelement, "Result");
				craftingRecipe.ResultValue = CraftingRecipesManager.DecodeResult(attributeValue);
				craftingRecipe.ResultCount = XmlUtils.GetAttributeValue<int>(xelement, "ResultCount");
				string attributeValue2 = XmlUtils.GetAttributeValue(xelement, "Remains", string.Empty);
				if (!string.IsNullOrEmpty(attributeValue2))
				{
					craftingRecipe.RemainsValue = CraftingRecipesManager.DecodeResult(attributeValue2);
					craftingRecipe.RemainsCount = XmlUtils.GetAttributeValue<int>(xelement, "RemainsCount");
				}
				craftingRecipe.RequiredHeatLevel = XmlUtils.GetAttributeValue<float>(xelement, "RequiredHeatLevel");
				craftingRecipe.Description = XmlUtils.GetAttributeValue<string>(xelement, "Description");
#if !DEBUG
				if (craftingRecipe.ResultCount > BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.ResultValue)].MaxStacking)
					throw new InvalidOperationException($"In recipe for \"{attributeValue}\" ResultCount is larger than max stacking of result block.");
				if (craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.RemainsValue)].MaxStacking)
					throw new InvalidOperationException($"In Recipe for \"{attributeValue2}\" RemainsCount is larger than max stacking of remains block.");
#endif
				var dictionary = new Dictionary<char, string>();
				foreach (XAttribute item in xelement.Attributes().Where(CraftingRecipesManager.c._.Initialize_b__3_1))
				{
					if (item.Value.StartsWith("New", StringComparison.Ordinal))
						continue;
					CraftingRecipesManager.DecodeIngredient(item.Value, out string craftingId, out int? data);
					if (BlocksManager.FindBlocksByCraftingId(craftingId).Length == 0)
#if DEBUG
						throw new InvalidOperationException($"Block with craftingId \"{item.Value}\" not found.");
#else
						continue;
#endif
					if (data.HasValue && (data.Value < 0 || data.Value > 262143))
						throw new InvalidOperationException($"Data in recipe ingredient \"{item.Value}\" must be between 0 and 0x3FFFF.");
					dictionary.Add(item.Name.LocalName[0], item.Value);
				}
				string[] array = xelement.Value.Trim().Split('\n');
				string[] ingredients = craftingRecipe.Ingredients;
				for (int i = 0; i < array.Length; i++)
				{
					int num2 = array[i].IndexOf('"');
					int num3 = array[i].LastIndexOf('"');
					if (num2 < 0 || num3 < 0 || num3 <= num2)
						throw new InvalidOperationException("Invalid recipe line.");
					string text = array[i].Substring(num2 + 1, num3 - num2 - 1);
					for (int j = 0; j < text.Length; j++)
					{
						char c = text[j];
						if (char.IsLower(c))
						{
							if (!dictionary.TryGetValue(c, out string text2)) continue;
							if (ItemBlock.IdTable.TryGetValue(text2, out int value))
								text2 = BlocksManager.Blocks[Terrain.ExtractContents(value)].CraftingId + ":" + Terrain.ExtractData(value);
							ingredients[j + i * 6] = text2;
						}
					}
				}
				CraftingRecipesManager.m_recipes.Add(craftingRecipe);
				if (craftingRecipe.RequiredHeatLevel >= 300f)
				{
					CraftingRecipesManager.DecodeIngredient(ingredients[0], out string craftingId, out int? num);
					if (!num.HasValue)
						continue;
					recipes.Add(new CraftingRecipe
					{
						ResultValue = craftingRecipe.ResultValue,//Terrain.ReplaceData(BlocksManager.FindBlocksByCraftingId(craftingId)[0].BlockIndex, num.Value),
						ResultCount = craftingRecipe.ResultCount/2,//GetCount(ingredients),
						RemainsValue = craftingRecipe.RemainsValue,
						RemainsCount = craftingRecipe.RemainsCount,
						RequiredHeatLevel = 1f,
						Ingredients = (string[])ingredients.Clone(),
						Description = craftingRecipe.Description + Utils.Get(" (温度过低)")
					});
				}
			}
			var blocks = BlocksManager.Blocks;
			for (int i = 0; i < blocks.Length; i++)
				using (var enumerator2 = blocks[i].GetProceduralCraftingRecipes().GetEnumerator())
					while (enumerator2.MoveNext())
					{
						var old = enumerator2.Current.Ingredients;
						if (old.Length == 9)
						{
							var ingredients = new string[36];
							ingredients[0] = old[0];
							ingredients[1] = old[1];
							ingredients[2] = old[2];
							ingredients[6] = old[3];
							ingredients[7] = old[4];
							ingredients[8] = old[5];
							ingredients[12] = old[6];
							ingredients[13] = old[7];
							ingredients[14] = old[8];
							enumerator2.Current.Ingredients = ingredients;
						}
						CraftingRecipesManager.m_recipes.Add(enumerator2.Current);
					}
			CraftingRecipesManager.m_recipes.Sort(CraftingRecipesManager.c._.Initialize_b__3_0);
			CraftingRecipesManager.m_recipes.AddRange(recipes);
		}
		public static int GetCount(string[] ingredients)
		{
			if (ingredients == null || ingredients.Length == 0)
				return 0;
			string target = ingredients[0];
			int i = 0;
			for (; i < ingredients.Length; i++)
				if (ingredients[i] != target)
					return i;
			return i;
		}
		public static int DecodeResult(string result)
		{
			if (ItemBlock.IdTable.TryGetValue(result, out int value))
				return value;
			if (DecodeResult1 != null)
				return DecodeResult1(result);
			var array = result.Split(':');
			int data = array.Length >= 2 ? int.Parse(array[1], CultureInfo.InvariantCulture) : 0;
			return Terrain.MakeBlockValue(BlocksManager.FindBlockByTypeName(array[0], true).BlockIndex, 0, data);
		}
		public static bool MatchRecipe(string[] requiredIngredients, string[] actualIngredients)
		{
			var array = new string[36];
			for (int i = 0; i < 2; i++)
				for (int j = 0; j <= 6; j++)
					for (int k = 0; k <= 6; k++)
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
								return true;
						}
					}
			return false;
		}

		public static bool TransformRecipe(string[] transformedIngredients, string[] ingredients, int shiftX, int shiftY, bool flip)
		{
			Array.Clear(transformedIngredients, 0, 36);
			for (int j = 0; j < 6; j++)
			{
				for (int k = 0; k < 6; k++)
				{
					int num = (flip ? (6 - k - 1) : k) + shiftX;
					int num2 = j + shiftY;
					string text = ingredients[k + j * 6];
					if (num >= 0 && num2 >= 0 && num < 6 && num2 < 6)
						transformedIngredients[num + num2 * 6] = text;
					else if (!string.IsNullOrEmpty(text))
						return false;
				}
			}
			return true;
		}
		public virtual string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return GetType().Name;
		}
		public virtual string GetCraftingId()
		{
			return GetType().Name;
		}
		public virtual string GetDescription()
		{
			return string.Empty;
		}
		public virtual string GetCategory()
		{
			return "Items";
		}
		public virtual bool IsInteractive(SubsystemTerrain subsystemTerrain, int value)
		{
			return false;
		}
		/*public virtual IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			yield break;
		}
		public virtual CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return null;
		}*/
		public virtual bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}
		/*public virtual bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			return BlocksManager.Blocks[Terrain.ExtractContents(neighborValue)].IsFaceTransparent(subsystemTerrain, CellFace.OppositeFace(face), neighborValue);
		}
		public virtual int GetShadowStrength(int value)
		{
			return 0;
		}*/
		public virtual int GetFaceTextureSlot(int face, int value)
		{
			return 0;
		}
		/*public virtual string GetSoundMaterialName(SubsystemTerrain subsystemTerrain, int value)
		{
			return string.Empty;
		}*/
		public virtual void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(Block, value, x, y, z, Color.White, geometry.OpaqueSubsetsByFace);
		}
		public virtual void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public virtual BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { CellFace = raycastResult.CellFace };
		}
		public virtual BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { CellFace = raycastResult.CellFace };
		}
		public virtual void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue { Value = Terrain.ReplaceLight(oldValue, 0), Count = 1 });
		}
		public virtual int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) >> 4) & 0xFFF;
		}
		public virtual int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num &= 0xF;
			num |= MathUtils.Clamp(damage, 0, 4095) << 4;
			return Terrain.ReplaceData(value, num);
		}
		public virtual int GetDamageDestructionValue()
		{
			return 0;
		}
		public virtual int GetRotPeriod()
		{
			return 0;
		}
		public virtual float GetSicknessProbability()
		{
			return 0f;
		}
		public virtual float GetMeleePower()
		{
			return 1;
		}
		public virtual float GetMeleeHitProbability()
		{
			return 0.66f;
		}
		public virtual float GetProjectilePower()
		{
			return 1;
		}
		/*public virtual float GetHeat(int value)
		{
			return 0f;
		}*/
		public virtual float GetExplosionPressure()
		{
			return 0f;
		}
		public virtual bool GetExplosionIncendiary()
		{
			return false;
		}
		public virtual Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return Vector3.Zero;
		}
		public virtual Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return Vector3.One;
		}
		public virtual float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return 1f;
		}
		public virtual BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, 1f, Color.White, GetFaceTextureSlot(4, value));
		}
		public virtual BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return Game.Block.m_defaultCollisionBoxes;
		}
		/*public virtual BoundingBox[] GetCustomInteractionBoxes(SubsystemTerrain terrain, int value)
		{
			return GetCustomCollisionBoxes(terrain, value);
		}*/
		public virtual int GetEmittedLightAmount(int value)
		{
			return 0;
		}
		public virtual float GetNutritionalValue()
		{
			return 0;
		}
		/*public virtual bool ShouldAvoid(int value)
		{
			return false;
		}
		public virtual bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			return true;
		}
		public virtual bool IsHeatBlocker(int value)
		{
			return false;
		}*/
	}
	public abstract partial class ItemBlock : CubeBlock, IItemBlock, IFuel
	{
		public const int Index = 246;
		public virtual IItem GetItem(ref int value)
		{
			return Terrain.ExtractContents(value) != Index ? null : Items[Terrain.ExtractData(value)];
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return GetItem(ref value).GetDisplayName(subsystemTerrain, value);
		}
		public override string GetDescription(int value)
		{
			return Utils.Get(GetItem(ref value).GetDescription());
		}
		public override string GetCategory(int value)
		{
			return GetItem(ref value).GetCategory();
		}
		public override bool IsInteractive(SubsystemTerrain subsystemTerrain, int value)
		{
			return GetItem(ref value).IsInteractive(subsystemTerrain, value);
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return GetItem(ref value).IsFaceTransparent(subsystemTerrain, face, value);
		}
		/*public override bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			return GetItem(ref value).ShouldGenerateFace(subsystemTerrain, face, value, neighborValue);
		}
		public override int GetShadowStrength(int value)
		{
			return GetItem(ref value).GetShadowStrength(value);
		}*/
		public override int GetFaceTextureSlot(int face, int value)
		{
			return GetItem(ref value).GetFaceTextureSlot(face, value);
		}
		/*public override string GetSoundMaterialName(SubsystemTerrain subsystemTerrain, int value)
		{
			return GetItem(ref value).GetSoundMaterialName(subsystemTerrain, value);
		}*/
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			GetItem(ref value).GenerateTerrainVertices(this, generator, geometry, value, x, y, z);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			GetItem(ref value).DrawBlock(primitivesRenderer, value, color, size, ref matrix, environmentData);
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int value2 = value;
			return GetItem(ref value2).GetPlacementValue(subsystemTerrain, componentMiner, value, raycastResult);
		}
		public override BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult)
		{
			return GetItem(ref value).GetDigValue(subsystemTerrain, componentMiner, value, toolValue, raycastResult);
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			GetItem(ref oldValue).GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
		}
		public override int GetDamage(int value)
		{
			return GetItem(ref value).GetDamage(value);
		}
		public override int SetDamage(int value, int damage)
		{
			return GetItem(ref value).SetDamage(value, damage);
		}
		public override int GetDamageDestructionValue(int value)
		{
			return GetItem(ref value).GetDamageDestructionValue();
		}
		public override int GetRotPeriod(int value)
		{
			return GetItem(ref value).GetRotPeriod();
		}
		public override float GetSicknessProbability(int value)
		{
			return GetItem(ref value).GetSicknessProbability();
		}
		public override float GetMeleePower(int value)
		{
			return GetItem(ref value).GetMeleePower();
		}
		public override float GetMeleeHitProbability(int value)
		{
			return GetItem(ref value).GetMeleeHitProbability();
		}
		public override float GetProjectilePower(int value)
		{
			return GetItem(ref value).GetProjectilePower();
		}
		/*public override float GetHeat(int value)
		{
			return GetItem(ref value).GetHeat();
		}*/
		public override float GetExplosionPressure(int value)
		{
			return GetItem(ref value).GetExplosionPressure();
		}
		public override bool GetExplosionIncendiary(int value)
		{
			return GetItem(ref value).GetExplosionIncendiary();
		}
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return GetItem(ref value).GetIconBlockOffset(value, environmentData);
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return GetItem(ref value).GetIconViewOffset(value, environmentData);
		}
		public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return GetItem(ref value).GetIconViewScale(value, environmentData);
		}
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return GetItem(ref value).CreateDebrisParticleSystem(subsystemTerrain, position, value, strength);
		}
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return GetItem(ref value).GetCustomCollisionBoxes(terrain, value);
		}
		/*public override BoundingBox[] GetCustomInteractionBoxes(SubsystemTerrain terrain, int value)
		{
			return GetItem(ref value).GetCustomInteractionBoxes(terrain, value);
		}*/
		public override int GetEmittedLightAmount(int value)
		{
			return GetItem(ref value).GetEmittedLightAmount(value);
		}
		public override float GetNutritionalValue(int value)
		{
			return GetItem(ref value).GetNutritionalValue();
		}
		/*public override bool ShouldAvoid(int value)
		{
			return GetItem(ref value).ShouldAvoid(value);
		}
		public override bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			return GetItem(ref newValue).IsSwapAnimationNeeded(oldValue, newValue);
		}
		public override bool IsHeatBlocker(int value)
		{
			return GetItem(ref value).IsHeatBlocker(value);
		}*/
		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[Items.Length];
			int value = Index;
			for (int i = 0; i < Items.Length; i++)
			{
				arr[i] = value;
				value += 1 << 14;
			}
			return arr;
		}
		public float GetHeatLevel(int value)
		{
			return (GetItem(ref value) is IFuel fuel) ? fuel.GetHeatLevel(value) : 0f;
		}
		public float GetFuelFireDuration(int value)
		{
			return (GetItem(ref value) is IFuel fuel) ? fuel.GetFuelFireDuration(value) : 0f;
		}
	}
	public abstract class PaintableItemBlock : ItemBlock, IPaintableBlock
	{
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			var item = GetItem(ref value);
			return item != null
				? SubsystemPalette.GetName(subsystemTerrain, GetPaintColor(value), item.GetDisplayName(subsystemTerrain, value))
				: base.GetDisplayName(subsystemTerrain, value);
		}
		public override string GetCategory(int value)
		{
			return GetPaintColor(value).HasValue ? "Painted" : base.GetCategory(value);
		}
		public int? GetPaintColor(int value)
		{
			return GetColor(Terrain.ExtractData(value));
		}
		public int Paint(SubsystemTerrain subsystemTerrain, int value, int? color)
		{
			return Terrain.ReplaceData(value, SetColor(Terrain.ExtractData(value), color));
		}
		public static int? GetColor(int data)
		{
			return (data & 0b11110000000000) != 0 ? data >> 10 & 15 : new int?();
		}
		public static int SetColor(int data, int? color)
		{
			data &= -15361;
			return color.HasValue ? (color == 0 ? data : (data | (color.Value & 15) << 10)) : data;
		}
	}
	public class RottenEggBlock : ItemBlock { public new const int Index = 246; }
}