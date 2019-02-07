using Chemistry;
using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public interface ICraftingMachine : IInventory
	{
		int ResultSlotIndex { get; }
		int RemainsSlotIndex { get; }
		int SlotIndex { get; set; }
		CraftingRecipe GetRecipe();
	}
	public interface INode
	{
		int GetWeight(int value = 0);
		void Simulate(ref int value);
	}
	public interface IChemicalItem : IItem
	{
		DispersionSystem GetDispersionSystem();
	}
	public interface IDurability
	{
		int GetDurability(int value);
	}
	public interface IFuel
	{
		float GetHeatLevel(int value);
		float GetFuelFireDuration(int value);
	}
	public interface IItemBlock // : IReadOnlyList<Item>, IStructuralComparable, IStructuralEquatable
	{
		IItem GetItem(ref int value);
	}
	public interface IItem : IAnimatedItem, IUnstableItem, IFood, IExplosive, IWeapon, IScalableItem, ICollidableItem
	{
	}
	public interface IItemBase
	{
		string GetDisplayName(SubsystemTerrain subsystemTerrain, int value);
		string GetDescription(int value);
		string GetCategory(int value);
		int GetFaceTextureSlot(int face, int value);
		void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData);
		Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData);
		Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData);
	}
	public interface IAnimatedItem
	{
		bool IsSwapAnimationNeeded(int oldValue, int newValue);
	}
	public interface IUnstableItem
	{
		int GetDamage(int value);
		int SetDamage(int value, int damage);
		int GetDamageDestructionValue(int value);
	}
	public interface IFood
	{
		int GetRotPeriod(int value);
		float GetSicknessProbability(int value);
		float GetNutritionalValue(int value);
	}
	public interface IExplosive
	{
		float GetExplosionPressure(int value);
		bool GetExplosionIncendiary(int value);
	}
	public interface IWeapon
	{
		float GetMeleePower(int value);
		float GetMeleeHitProbability(int value);
		float GetProjectilePower(int value);
	}
	public interface IScalableItem : IItemBase
	{
		float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData);
	}
	public interface IPlaceableItem : IItemBase
	{
		bool IsInteractive(SubsystemTerrain subsystemTerrain, int value);
		bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value);
		bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue);
		int GetShadowStrength(int value);
		string GetSoundMaterialName(SubsystemTerrain subsystemTerrain, int value);
		void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z);
		BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult);
		BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult);
		void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris);
		float GetHeat(int value);
		BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength);
		int GetEmittedLightAmount(int value);
		bool ShouldAvoid(int value);
		bool IsHeatBlocker(int value);
	}
	public interface ICollidableItem : IPlaceableItem
	{
		BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value);
		BoundingBox[] GetCustomInteractionBoxes(SubsystemTerrain terrain, int value);
	}
	public interface IBlockBehavior
	{
		void OnBlockAdded(SubsystemTerrain terrain, int value, int oldValue);
		//void OnBlockModified(int value, int oldValue, int x, int y, int z);
		void OnBlockRemoved(SubsystemTerrain terrain, int value, int newValue);
	}
	/*public interface IChunkAction
	{
		void OnChunkDiscarding(TerrainChunk chunk);
		void OnChunkInitialized(TerrainChunk chunk);
	}
	public interface IGeneratedBlock
	{
		void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded);
	}
	public interface IAimableItem
	{
		bool OnAim(Vector3 start, Vector3 direction, ComponentMiner componentMiner, AimState state);
	}
	public interface IUseableItem
	{
		bool OnUse(Vector3 start, Vector3 direction, ComponentMiner componentMiner);
	}
	public interface IPlaceableItem
	{
		void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue);
	}*/
	public interface IHarvestingItem
	{
		void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue);
	}
	public interface IInteractiveBlock
	{
		bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner);
	}
	public interface IItemAcceptableBlock
	{
		void OnHitByProjectile(CellFace cellFace, WorldItem worldItem);
	}
	/*public interface IProjectile
	{
		void OnFiredAsProjectile(Projectile projectile);
		bool OnHitAsProjectile(CellFace? cellFace, ComponentBody componentBody, WorldItem worldItem);
	}
	public interface ICollidableBlock
	{
		void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody);
	}*/
	public interface IUnstableBlock
	{
		void OnNeighborBlockChanged(SubsystemTerrain terrain, int neighborX, int neighborY, int neighborZ);
	}
	/*public interface IEditableBlock
	{
		bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer);
	}
	public interface IEditableItem
	{
		bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer);
	}
	public interface IExplosiveBlock
	{
		void OnExplosion(int value, int x, int y, int z, float damage);
	}
	public interface IInventoryBehavior
	{
		int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value);
		void ProcessInventoryItem(IInventory inventory, int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount);
	}*/
}