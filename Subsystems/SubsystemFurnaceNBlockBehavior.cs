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
			switch (FurnaceNBlock.GetHeatLevel(oldValue).CompareTo(FurnaceNBlock.GetHeatLevel(value)))
			{
				case -1: AddFire(x, y, z); return;
				case 1: RemoveFire(new Point3(x, y, z)); return;
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

	public class SubsystemFurnaceNBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentFurnaceN>
	{
		public SubsystemFurnaceNBlockBehavior() : base("FurnaceN") { }

		public override int[] HandledBlocks => new[] { FurnaceNBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentFurnaceN component)
		{
			return new FurnaceNWidget(inventory, component);
		}
	}
	public class SubsystemFireBoxBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentFireBox>
	{
		public override int[] HandledBlocks => new[] { FireBoxBlock.Index };

		public SubsystemFireBoxBlockBehavior() : base("FireBox") { }

		public override Widget GetWidget(IInventory inventory, ComponentFireBox component)
		{
			return new FireBoxWidget<ComponentFireBox>(inventory, component, "Widgets/FireBoxWidget");
		}
	}
	public class SubsystemEngineBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentMachine>
	{
		public static string[] Names = new[]
		{
			"SteamEngine",
			"HeatEngine",
			"SteamEngine",
			"HeatEngine",
			"ICEngine",
			"ICEngine"
		};
		public override int[] HandledBlocks => new[] { EngineBlock.Index };

		public SubsystemEngineBlockBehavior() : base("SteamEngine") { }

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int type = Terrain.ExtractData(Utils.Terrain.GetCellValueFast(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z)) >> 10;
			if (base.OnInteract(raycastResult, componentMiner) && type != 0 && componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget is StoveWidget widget)
				widget.Children.Find<LabelWidget>("Label", false).Text = Utils.Get(EngineBlock.Names[type]);
			return true;
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			Name = Names[Terrain.ExtractData(value) >> 10];
			base.OnBlockAdded(value, oldValue, x, y, z);
		}

		public override Widget GetWidget(IInventory inventory, ComponentMachine component)
		{
			var c = component.Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
			switch (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(c.X, c.Y, c.Z)) >> 10)
			{
				case 0:
				case 2:
					return new StoveWidget(inventory, component, "Widgets/EngineWidget");
				case 1:
				case 3:
					return new FireBoxWidget<ComponentMachine>(inventory, component, "Widgets/EngineHWidget");
			}
			return new ICEngineWidget(inventory, component);
		}
	}
}