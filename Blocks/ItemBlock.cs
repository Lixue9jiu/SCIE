using System.Collections.Generic;
using System.Collections;
using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	public class Item : IAnimatedItem, IUnstableItem, IFood, IExplosive, IWeapon, IScalableItem, ICollidableItem
	{
		internal static readonly BoundingBox[] m_defaultCollisionBoxes = new BoundingBox[]
		{
			new BoundingBox(Vector3.Zero, Vector3.One)
		};
		public virtual string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return string.Empty;
		}
		public virtual string GetDescription(int value)
		{
			return string.Empty;
		}
		public virtual string GetCategory(int value)
		{
			return string.Empty;
		}
		public virtual bool IsInteractive(SubsystemTerrain subsystemTerrain, int value)
		{
			return false;
		}
		public virtual IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			yield break;
		}
		public virtual CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return null;
		}
		public virtual bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return false;
		}
		public virtual bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			int num = Terrain.ExtractContents(neighborValue);
			return BlocksManager.Blocks[num].IsFaceTransparent(subsystemTerrain, CellFace.OppositeFace(face), neighborValue);
		}
		public virtual int GetShadowStrength(int value)
		{
			return 0;
		}
		public virtual int GetFaceTextureSlot(int face, int value)
		{
			return 0;
		}
		public virtual string GetSoundMaterialName(SubsystemTerrain subsystemTerrain, int value)
		{
			return string.Empty;
		}
		public virtual void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(block, value, x, y, z, Color.White, geometry.OpaqueSubsetsByFace);
		}
		public virtual void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}
		public virtual BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}
		public virtual BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = 0,
				CellFace = raycastResult.CellFace
			};
		}
		public virtual void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
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
		public virtual int GetDamageDestructionValue(int value)
		{
			return 0;
		}
		public virtual int GetRotPeriod(int value)
		{
			return 0;
		}
		public virtual float GetSicknessProbability(int value)
		{
			return 0f;
		}
		public virtual float GetMeleePower(int value)
		{
			return 1;
		}
		public virtual float GetMeleeHitProbability(int value)
		{
			return 0.66f;
		}
		public virtual float GetProjectilePower(int value)
		{
			return 1;
		}
		public virtual float GetHeat(int value)
		{
			return 0f;
		}
		public virtual float GetExplosionPressure(int value)
		{
			return 0f;
		}
		public virtual bool GetExplosionIncendiary(int value)
		{
			return false;
		}
		public virtual Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return Vector3.Zero;
		}
		public virtual Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return new Vector3(1f);
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
			return m_defaultCollisionBoxes;
		}
		public virtual BoundingBox[] GetCustomInteractionBoxes(SubsystemTerrain terrain, int value)
		{
			return GetCustomCollisionBoxes(terrain, value);
		}
		public virtual int GetEmittedLightAmount(int value)
		{
			return 0;
		}
		public virtual float GetNutritionalValue(int value)
		{
			return 0;
		}
		public virtual bool ShouldAvoid(int value)
		{
			return false;
		}
		public virtual bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			return true;
		}
		public virtual bool IsHeatBlocker(int value)
		{
			return true;
		}
	}
	public class ItemBlock : CubeBlock, IItemBlock
	{
		public const int Index = 567;
		public static Item[] Items;
		//public static DynamicArray<Item> ItemList;
		public static Dictionary<string, int> IdTable;
		public static readonly Item DefaultItem;
		public Item this[int index] => Items[index];
		public int Count => Items.Length;
		public virtual Item GetItem(ref int value)
		{
			if (Terrain.ExtractContents(value) != BlockIndex)
				return DefaultItem;
			value = Terrain.ExtractData(value);
			if (value < Items.Length)
			{
				return Items[value];
			}
			return DefaultItem;
		}
		/*public virtual void Add(Item item)
		{
		}*/
		public override void Initialize()
		{
			//ItemList = new DynamicArray<Item>();
			Items = new Item[]
			{

			};
			IdTable = new Dictionary<string, int>(Items.Length);
			base.Initialize();
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return GetItem(ref value).GetDisplayName(subsystemTerrain, value);
		}
		public override string GetDescription(int value)
		{
			return GetItem(ref value).GetDescription(value);
		}
		public override string GetCategory(int value)
		{
			return GetItem(ref value).GetCategory(value);
		}
		public override bool IsInteractive(SubsystemTerrain subsystemTerrain, int value)
		{
			return GetItem(ref value).IsInteractive(subsystemTerrain, value);
		}
		public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			return new CraftingRecipe[]
			{

			};
		}
		public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return null;
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return GetItem(ref value).IsFaceTransparent(subsystemTerrain, face, value);
		}
		public override bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			return GetItem(ref value).ShouldGenerateFace(subsystemTerrain, face, value, neighborValue);
		}
		public override int GetShadowStrength(int value)
		{
			return GetItem(ref value).GetShadowStrength(value);
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return GetItem(ref value).GetFaceTextureSlot(face, value);
		}
		public override string GetSoundMaterialName(SubsystemTerrain subsystemTerrain, int value)
		{
			return GetItem(ref value).GetSoundMaterialName(subsystemTerrain, value);
		}
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
			return GetItem(ref value).GetDamageDestructionValue(value);
		}
		public override int GetRotPeriod(int value)
		{
			return GetItem(ref value).GetRotPeriod(value);
		}
		public override float GetSicknessProbability(int value)
		{
			return GetItem(ref value).GetSicknessProbability(value);
		}
		public override float GetMeleePower(int value)
		{
			return GetItem(ref value).GetMeleePower(value);
		}
		public override float GetMeleeHitProbability(int value)
		{
			return GetItem(ref value).GetMeleeHitProbability(value);
		}
		public override float GetProjectilePower(int value)
		{
			return GetItem(ref value).GetProjectilePower(value);
		}
		public override float GetHeat(int value)
		{
			return GetItem(ref value).GetHeat(value);
		}
		public override float GetExplosionPressure(int value)
		{
			return GetItem(ref value).GetExplosionPressure(value);
		}
		public override bool GetExplosionIncendiary(int value)
		{
			return GetItem(ref value).GetExplosionIncendiary(value);
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
		public override BoundingBox[] GetCustomInteractionBoxes(SubsystemTerrain terrain, int value)
		{
			return GetItem(ref value).GetCustomInteractionBoxes(terrain, value);
		}
		public override int GetEmittedLightAmount(int value)
		{
			return GetItem(ref value).GetEmittedLightAmount(value);
		}
		public override float GetNutritionalValue(int value)
		{
			return GetItem(ref value).GetNutritionalValue(value);
		}
		public override bool ShouldAvoid(int value)
		{
			return GetItem(ref value).ShouldAvoid(value);
		}
		public override bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			return GetItem(ref oldValue).IsSwapAnimationNeeded(oldValue, newValue);
		}
		public override bool IsHeatBlocker(int value)
		{
			return GetItem(ref value).IsHeatBlocker(value);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			if (DefaultCreativeData < 0)
			{
				return new int[0];
			}
			var list = new List<int>(8);
			for (int i = 0, value = BlockIndex; GetItem(ref value) != null; value = Terrain.MakeBlockValue(BlockIndex, 0, ++i))
			{
				list.Add(value);
			}
			return list;
		}
		public bool Equals(object other, IEqualityComparer comparer)
		{
			return (Items as IStructuralEquatable).Equals(other, comparer);
		}
		public int GetHashCode(IEqualityComparer comparer)
		{
			return (Items as IStructuralEquatable).GetHashCode(comparer);
		}
		public int CompareTo(object other, IComparer comparer)
		{
			return (Items as IStructuralComparable).CompareTo(other, comparer);
		}
		public IEnumerator<Item> GetEnumerator()
		{
			return (Items as IEnumerable<Item>).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}
	}
	/*public class PaintableItemBlock : Block, IPaintableBlock
	{
	}*/
}
