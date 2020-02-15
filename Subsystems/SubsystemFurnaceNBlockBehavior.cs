namespace Game
{
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
		public static string[] Names =
		{
			"SteamEngine",
			"HeatEngine",
			"SteamEngine",
			"HeatEngine",
			"ICEngine",
			"ICEngine",
			"NEngine"
		};
		public override int[] HandledBlocks => new[] { EngineBlock.Index };

		public SubsystemEngineBlockBehavior() : base(null) { }

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
				case 6:
					return new NuclearWidget(inventory, component);
			}
			return new ICEngineWidget(inventory, component);
		}
	}
}