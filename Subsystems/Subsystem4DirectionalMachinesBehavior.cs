namespace Game
{
	public class SubsystemPresserBlockBehavior : SubsystemInventoryBlockBehavior<ComponentPresser>
	{
		public SubsystemPresserBlockBehavior() : base("Presser")
		{
		}

		public override int[] HandledBlocks => new[] { PresserBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentPresser component)
		{
			return new PresserWidget<ComponentPresser>(inventory, component);
		}
	}
	public class SubsystemKibblerBlockBehavior : SubsystemInventoryBlockBehavior<ComponentKibbler>
	{
		public SubsystemKibblerBlockBehavior() : base("Kibbler")
		{
		}

		public override int[] HandledBlocks => new[] { KibblerBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentKibbler component)
		{
			return new PresserWidget<ComponentKibbler>(inventory, component);
		}
	}
    public class SubsystemSourBlockBehavior : SubsystemInventoryBlockBehavior<ComponentMachine>
    {
        public SubsystemSourBlockBehavior() : base("Sour")
        {
        }
		public static string[] Names = new[]
		{
			"Sour",
			"Deposit"
		};
		public override int[] HandledBlocks => new[] { SourBlock.Index };

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			Name = Names[Terrain.ExtractData(value) >> 10];
			base.OnBlockAdded(value, oldValue, x, y, z);
		}
		//public override Widget GetWidget(IInventory inventory, ComponentSour component)
        //{
        //    return new SeperatorWidget(inventory, component, "Widgets/SourWidget");
        //}

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
					return new SeperatorWidget(inventory, component, "Widgets/SourWidget");
				case 1:
					return new DepositWidget(inventory, component);
			}
			return null;
		}

	}
	public class SubsystemPresserNNBlockBehavior : SubsystemInventoryBlockBehavior<ComponentPresserNN>
	{
		public SubsystemPresserNNBlockBehavior() : base("PresserNN")
		{
		}

		public override int[] HandledBlocks => new[] { PresserNNBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentPresserNN component)
		{
			return new PresserWidget<ComponentPresserNN>(inventory, component);
		}
	}
	public class SubsystemSqueezerBlockBehavior : SubsystemInventoryBlockBehavior<ComponentSqueezer>
	{
		public SubsystemSqueezerBlockBehavior() : base("Squeezer")
		{
		}

		public override int[] HandledBlocks => new[] { SqueezerBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentSqueezer component)
		{
			return new PresserWidget<ComponentSqueezer>(inventory, component);
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