using System.Collections.Generic;
using Engine;
using TemplatesDatabase;
namespace Game
{
	public class SubsystemPMachsBlockBehavior : SubsystemCombinedBlockBehavior<ComponentMachine>
	{
		public SubsystemPMachsBlockBehavior() : base(new Dictionary<int, string>
		{
			{ PresserBlock.Index, "Presser" },
			{ KibblerBlock.Index, "Kibbler" },
			{ PresserNNBlock.Index, "RiflingMachine" },
			{ SqueezerBlock.Index, "Squeezer" }
		})
		{
		}

		public override int[] HandledBlocks => new[] {
			PresserBlock.Index,
			KibblerBlock.Index,
			PresserNNBlock.Index,
			SqueezerBlock.Index
		};

		public override Widget GetWidget(IInventory inventory, ComponentMachine component, string path)
		{
			return new PresserWidget(inventory, component);
		}
	}
    public class SubsystemSourBlockBehavior : SubsystemInventoryBlockBehavior<ComponentMachine>
    {
        public SubsystemSourBlockBehavior() : base("Sour")
        {
        }
		public SubsystemBlockEntities m_subsystemBlockEntities;
		public static string[] Names = new[]
		{
			"Sour",
			"Deposit",
			"Sorter"
		};
		public override int[] HandledBlocks => new[] { SourBlock.Index };

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			Name = Names[Terrain.ExtractData(value) >> 10];
			base.OnBlockAdded(value, oldValue, x, y, z);
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
			{
				return;
			}
			ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
			Vector3 v = CellFace.FaceToVector3(cellFace.Face);
			if (blockEntity != null)
			{
				var position = new Vector3(cellFace.Point) + new Vector3(0.5f) - 0.75f * v;
				ComponentSorter inventory = blockEntity.Entity.FindComponent<ComponentSorter>(throwOnError: true);
				if(inventory.GetSlotValue(0)==worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(0f,0f,1f);
					Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position, -10f * v, Vector3.Zero, null);
					return;
				}
				if (inventory.GetSlotValue(1) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(0f, 0f, -1f);
					Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position, -10f * v, Vector3.Zero, null);
					return;
				}
				if (inventory.GetSlotValue(2) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(1f, 0f, 0f);
					Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position, -10f * v, Vector3.Zero, null);
					return;
				}
				if (inventory.GetSlotValue(3) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(-1f, 0f, 0f);
					Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position, -10f * v, Vector3.Zero, null);
					return;
				}
				worldItem.ToRemove = true;
				Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position, -10f * v, Vector3.Zero, null);
				return;
			}
		}
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemBlockEntities = base.Project.FindSubsystem<SubsystemBlockEntities>(throwOnError: true);
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int type = Terrain.ExtractData(Utils.Terrain.GetCellValueFast(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z)) >> 10;
			if (base.OnInteract(raycastResult, componentMiner) && type != 0 && componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget is StoveWidget widget)
				widget.Children.Find<LabelWidget>("Label", false).Text = Utils.Get(SourBlock.Names[type]);
			return true;
		}
		public override Widget GetWidget(IInventory inventory, ComponentMachine component)
		{
			var c = component.Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
			switch (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(c.X, c.Y, c.Z)) >> 10)
			{
				case 0:
					return new SeparatorWidget(inventory, component, "Sour", "Widgets/SourWidget");
				case 1:
					return new SeparatorWidget(inventory, component, "Deposit");
				case 2:
					return new SorterWidget(inventory, component);
			}
			return null;
		}

	}
	public class SubsystemCastMachBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentCastMach>
	{
		public override int[] HandledBlocks => new[] { CastMachBlock.Index };

		public SubsystemCastMachBlockBehavior() : base("CastMach")
		{
		}

		public override Widget GetWidget(IInventory inventory, ComponentCastMach component)
		{
			return new CastMachWidget(inventory, component);
		}
	}
	public class SubsystemCReactorBlockBehavior : SubsystemInventoryBlockBehavior<ComponentCReactor>
	{
		public SubsystemCReactorBlockBehavior() : base("CReactor")
		{
		}

		public override int[] HandledBlocks => new[] { CReactorBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentCReactor component)
		{
			return new CReactorWidget(inventory, component);
		}
	}
	public class SubsystemUnloaderBlockBehavior : SubsystemInventoryBlockBehavior<ComponentUnloader>
	{
		public override int[] HandledBlocks => new[] { Bullet2Block.Index };

		public SubsystemUnloaderBlockBehavior() : base("Unloader")
		{
		}

		public override Widget GetWidget(IInventory inventory, ComponentUnloader component)
		{
			return new NewChestWidget(inventory, component, "Unloader");
		}
	}
}