using Engine;
using GameEntitySystem;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public abstract class SubsystemFurnaceBlockBehavior<T> : SubsystemInventoryBlockBehavior<T> where T : Component
	{
		protected readonly Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		protected SubsystemParticles m_subsystemParticles;

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
				RemoveFire(x, y, z);
			}
		}

		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			switch (FurnaceNBlock.GetHeatLevel(oldValue).CompareTo(FurnaceNBlock.GetHeatLevel(value)))
			{
				case -1:
					AddFire(value, x, y, z);
					break;
				case 1:
					RemoveFire(x, y, z);
					break;
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
			int originX = chunk.Origin.X, originY = chunk.Origin.Y;
			var list = new List<Point3>();
			for (var i = m_particleSystemsByCell.Keys.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current;
				if (key.X >= originX && key.X < originX + 16 && key.Z >= originY && key.Z < originY + 16)
				{
					list.Add(key);
				}
			}
			for (originX = 0; originX < list.Count; originX++)
			{
				Point3 item = list[originX];
				RemoveFire(item.X, item.Y, item.Z);
			}
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(true);
		}

		protected void AddFire(int value, int x, int y, int z)
		{
			const float size = 0.15f;
			var fireParticleSystem = new FireParticleSystem(new Vector3((float)x + 0.5f, (float)y + 0.2f, (float)z + 0.5f), size, 16f);
			m_subsystemParticles.AddParticleSystem(fireParticleSystem);
			m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
		}

		protected void RemoveFire(int x, int y, int z)
		{
			var key = new Point3(x, y, z);
			m_subsystemParticles.RemoveParticleSystem(m_particleSystemsByCell[key]);
			m_particleSystemsByCell.Remove(key);
		}
	}
	public class SubsystemFurnaceNBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentFurnaceN>
	{
		public SubsystemFurnaceNBlockBehavior() : base("FurnaceN")
		{
		}

		public override int[] HandledBlocks
		{
			get
			{
				return new[] { FurnaceNBlock.Index };
			}
		}
		public override Widget GetWidget(IInventory inventory, ComponentFurnaceN component)
		{
			return new FurnaceNWidget(inventory, component);
		}
	}
}
