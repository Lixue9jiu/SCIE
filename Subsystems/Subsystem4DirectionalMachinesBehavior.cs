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
			"Sorter",
			"SChest"
		};
		public override int[] HandledBlocks => new[] { SourBlock.Index };

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			Name = Names[Terrain.ExtractData(value) >> 10];
			base.OnBlockAdded(value, oldValue, x, y, z);
		}
		public SubsystemPickables m_subsystemPickables;
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
			{
				return;
			}
			if (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(cellFace.X, cellFace.Y, cellFace.Z))<2048)
			{
				return;
			}
			ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
			if (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(cellFace.X, cellFace.Y, cellFace.Z)) == 3*1024)
			{
				//ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
				if (blockEntity != null)
				{
					ComponentSorter inventory = blockEntity.Entity.FindComponent<ComponentSorter>(throwOnError: true);
					Pickable pickable = worldItem as Pickable;
					int num = pickable?.Count ?? 1;
					int num2 = ComponentInventoryBase.AcquireItems(inventory, worldItem.Value, num);
					if (num2 <= 0)
					{
						worldItem.ToRemove = true;
					}
					else if (pickable != null)
					{
						pickable.Count = num2;
					}
				}
				return;
			}
			
			Vector3 v = CellFace.FaceToVector3(cellFace.Face);
			if (blockEntity != null)
			{
				Pickable pickable = worldItem as Pickable;
				int num = pickable?.Count ?? 1;
				if (num>1)
				{
					return;
				}
				var position = new Vector3(cellFace.Point) + new Vector3(0.5f);
				ComponentSorter inventory = blockEntity.Entity.FindComponent<ComponentSorter>(throwOnError: true);
				if(inventory.GetSlotValue(0)==worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(0f,0f,1f);
					if(Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position+0.75f*v, 10f * v, Vector3.Zero, null)== null)
				    {
						m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * (v), null);
					}
					return;
				}
				if (inventory.GetSlotValue(1) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(0f, 0f, -1f);
					if(Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position + 0.75f * v, 10f * v, Vector3.Zero, null) == null)
				    {
						m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * (v), null);
					}
					return;
				}
				if (inventory.GetSlotValue(2) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(1f, 0f, 0f);
					if(Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position + 0.75f * v, 10f * v, Vector3.Zero, null) == null)
				    {
						m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * (v), null);
					}
					return;
				}
				if (inventory.GetSlotValue(3) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(-1f, 0f, 0f);
					if(Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position + 0.75f * v, 10f * v, Vector3.Zero, null) == null)
				    {
						m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * (v), null);
					}
					return;
				}
				v = -CellFace.FaceToVector3(cellFace.Face);
				worldItem.ToRemove = true;
				if(Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position + 0.75f * v, 10f * v, Vector3.Zero, null) == null)
				{
					m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * (v), null);
				}
				return;
			}
		}
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(throwOnError: true);
			m_subsystemPickables = Project.FindSubsystem<SubsystemPickables>(throwOnError: true);
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
				case 3:
					return new SteelChestWidget(inventory, component);
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
	public class SubsystemUnloaderBlockBehavior : SubsystemInventoryBlockBehavior<ComponentInventoryBase>
	{
		public override int[] HandledBlocks => new[] { Bullet2Block.Index };
		public static string[] Names = new[]
		{
			"Unloader",
			"Inserter",
		};
		public SubsystemUnloaderBlockBehavior() : base("Unloader")
		{
		}
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			Name = Names[(Terrain.ExtractData(value) >> 10)-1];
			base.OnBlockAdded(value, oldValue, x, y, z);
		}
		//public override Widget GetWidget(IInventory inventory, ComponentUnloader component)
		//{
		//	return new NewChestWidget(inventory, component, "Unloader");
		//}
		public override Widget GetWidget(IInventory inventory, ComponentInventoryBase component)
		{
			var c = component.Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
			switch (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(c.X, c.Y, c.Z)) >> 10)
			{
				case 0:
					return null;
				case 1:
					return new NewChestWidget(inventory, component, "Unloader");
				case 2:
					return new NewChestWidget(inventory, component, "Inserter", "Widgets/NewChest2Widget");
			}
			return null;
		}
	}
}