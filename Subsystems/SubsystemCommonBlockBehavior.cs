using Engine;
using GameEntitySystem;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public abstract class SubsystemInventoryBlockBehavior<T> : SubsystemCraftingTableBlockBehavior where T : Component
	{
		public string Name;

		protected SubsystemInventoryBlockBehavior(string name) { Name = name; }

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Name != null)
				Project.CreateBlockEntity(Name, new Point3(x, y, z));
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var entity = Utils.GetBlockEntity(raycastResult.CellFace.Point);
			if (entity == null || componentMiner.ComponentPlayer == null)
				return false;
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = GetWidget(componentMiner.Inventory, entity.Entity.FindComponent<T>(true));
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem) => Utils.OnHitByProjectile(cellFace, worldItem);

		public abstract Widget GetWidget(IInventory inventory, T component);
	}

	public abstract class SubsystemInventoryBlockBehavior<T, U, V> : SubsystemCraftingTableBlockBehavior where T : Component
	{
		public string Name;

		protected SubsystemInventoryBlockBehavior(string name) { Name = name; }

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			if (Name != null)
				Project.CreateBlockEntity(Name, new Point3(x, y, z));
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var entity = Utils.GetBlockEntity(raycastResult.CellFace.Point);
			if (entity == null || componentMiner.ComponentPlayer == null)
				return false;
			componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = GetWidget(componentMiner.Inventory, entity.Entity.FindComponent<T>(true));
			AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			return true;
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem) => Utils.OnHitByProjectile(cellFace, worldItem);

		public abstract Widget GetWidget(IInventory inventory, T component);
	}

	public abstract class SubsystemFurnaceBlockBehavior<T> : SubsystemInventoryBlockBehavior<T> where T : Component
	{
		public static readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		public static SubsystemParticles m_subsystemParticles;

		protected SubsystemFurnaceBlockBehavior(string name) : base(name) { }

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			base.OnBlockAdded(value, oldValue, x, y, z);
			if (FurnaceNBlock.GetHeatLevel(value) != 0)
				AddFire(x, y, z);
		}

		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			base.OnBlockRemoved(value, newValue, x, y, z);
			if (FurnaceNBlock.GetHeatLevel(value) != 0)
				RemoveFire(new Point3(x, y, z));
		}

		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			oldValue = FurnaceNBlock.GetHeatLevel(oldValue);
			value = FurnaceNBlock.GetHeatLevel(value);
			if (oldValue < value)
			{
				AddFire(x, y, z); return;
			}
			if (oldValue > value)
			{
				RemoveFire(new Point3(x, y, z));
			}
		}

		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (FurnaceNBlock.GetHeatLevel(value) != 0)
				AddFire(x, y, z);
		}

		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			Utils.RemoveElementsInChunk(chunk, m_particleSystemsByCell.Keys, RemoveFire);
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(true);
		}

		public static void AddFire(int x, int y, int z)
		{
			var fireParticleSystem = new FireParticleSystem(new Vector3(x + 0.5f, y + 0.2f, z + 0.5f), 0.15f, 16f);
			m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		public static void RemoveFire(Point3 key)
		{
			m_subsystemParticles.RemoveParticleSystem(m_particleSystemsByCell[key]);
			m_particleSystemsByCell.Remove(key);
		}
	}

	public partial class Utils
	{
		public static void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
				return;
			var blockEntity = GetBlockEntity(cellFace.Point);
			if (blockEntity == null)
				return;
			var inventory = blockEntity.Entity.FindComponent<ComponentInventoryBase>(true);
			var pickable = worldItem as Pickable;
			int count = (pickable == null) ? 1 : pickable.Count;
			int value = worldItem.Value;
			int max = ComponentInventoryBase.AcquireItems(inventory, value, count);
			if (max < count)
				SubsystemAudio.PlaySound("Audio/PickableCollected", 1f, 0f, worldItem.Position, 3f, true);
			if (max <= 0)
				worldItem.ToRemove = true;
			else if (pickable != null)
				pickable.Count = max;
		}
	}
}