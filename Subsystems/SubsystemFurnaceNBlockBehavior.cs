using Engine;
using GameEntitySystem;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public abstract class SubsystemFurnaceBlockBehavior<T> : SubsystemInventoryBlockBehavior<T> where T : Component
	{
		public static readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		public static SubsystemParticles m_subsystemParticles;

		protected SubsystemFurnaceBlockBehavior(string name) : base(name)
		{
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			base.OnBlockAdded(value, oldValue, x, y, z);
			if (FurnaceNBlock.GetHeatLevel(value) != 0)
			{
				AddFire(value, x, y, z);
			}
		}

		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			base.OnBlockRemoved(value, newValue, x, y, z);
			if (FurnaceNBlock.GetHeatLevel(value) != 0)
			{
				RemoveFire(new Point3(x, y, z));
			}
		}

		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			switch (FurnaceNBlock.GetHeatLevel(oldValue).CompareTo(FurnaceNBlock.GetHeatLevel(value)))
			{
				case -1: AddFire(value, x, y, z); return;
				case 1: RemoveFire(new Point3(x, y, z)); return;
			}
		}

		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			if (FurnaceNBlock.GetHeatLevel(value) != 0)
			{
				AddFire(value, x, y, z);
			}
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

		public static void AddFire(int value, int x, int y, int z)
		{
			const float size = 0.15f;
			var fireParticleSystem = new FireParticleSystem(new Vector3((float)x + 0.5f, (float)y + 0.2f, (float)z + 0.5f), size, 16f);
			m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		public static void RemoveFire(Point3 key)
		{
			m_subsystemParticles.RemoveParticleSystem(m_particleSystemsByCell[key]);
			m_particleSystemsByCell.Remove(key);
		}
	}

	public class SubsystemFurnaceNBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentFurnaceN>
	{
		public SubsystemFurnaceNBlockBehavior() : base("FurnaceN")
		{
		}

		public override int[] HandledBlocks => new[] { FurnaceNBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentFurnaceN component)
		{
			return new FurnaceNWidget(inventory, component);
		}
	}
}