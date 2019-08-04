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
			return new PresserWidget<ComponentPresser>(inventory, component, "Widgets/PresserWidget");
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
			return new PresserWidget<ComponentKibbler>(inventory, component, "Widgets/KibblerWidget");
		}
	}
    public class SubsystemSourBlockBehavior : SubsystemInventoryBlockBehavior<ComponentSour>
    {
        public SubsystemSourBlockBehavior() : base("Sour")
        {
        }

        public override int[] HandledBlocks => new[] { SourBlock.Index };

        public override Widget GetWidget(IInventory inventory, ComponentSour component)
        {
            return new SeperatorWidget(inventory, component, "Widgets/SourWidget");
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
			return new PresserWidget<ComponentPresserNN>(inventory, component, "Widgets/PresserNNWidget");
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
			return new PresserWidget<ComponentSqueezer>(inventory, component, "Widgets/SqueezerWidget");
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