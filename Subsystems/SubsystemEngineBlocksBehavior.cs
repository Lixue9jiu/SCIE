namespace Game
{
	public class SubsystemEngineBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentEngine>
	{
		public override int[] HandledBlocks => new[] { EngineBlock.Index };

		public SubsystemEngineBlockBehavior() : base("SteamEngine")
		{
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			if (base.OnInteract(raycastResult, componentMiner) && Terrain.ExtractData(Utils.Terrain.GetCellValueFast(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z)) >> 10 != 0 && componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget is StoveWidget widget)
			{
				widget.Children.Find<LabelWidget>("Label", false).Text = "SteamTurbine";
			}
			return true;
		}

		public override Widget GetWidget(IInventory inventory, ComponentEngine component)
		{
			return new StoveWidget(inventory, component, "Widgets/EngineWidget");
		}
	}
	public class SubsystemEngineHBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentEngineH>
	{
		public override int[] HandledBlocks => new[] { EngineHBlock.Index };

		public SubsystemEngineHBlockBehavior() : base("HeatEngine")
		{
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			return (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z)) & 1024) == 0 && base.OnInteract(raycastResult, componentMiner);
		}

		public override Widget GetWidget(IInventory inventory, ComponentEngineH component)
		{
			return new FireBoxWidget<ComponentEngineH>(inventory, component, "Widgets/EngineHWidget");
		}
	}
}