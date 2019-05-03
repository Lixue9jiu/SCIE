namespace Game
{
	public class SubsystemLiquidPumpBlockBehavior : SubsystemDrillerBlockBehavior
	{
		public SubsystemLiquidPumpBlockBehavior() { Name = "LiquidPump"; }

		public override int[] HandledBlocks => new[] { LiquidPumpBlock.Index };

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			if (Utils.SubsystemGameInfo.WorldSettings.GameMode != GameMode.Adventure)
			{
				var blockEntity = Utils.GetBlockEntity(raycastResult.CellFace.Point);
				if (blockEntity != null && componentMiner.ComponentPlayer != null)
				{
					ComponentLiquidPump componentDispenser = blockEntity.Entity.FindComponent<ComponentLiquidPump>(true);
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new LiquidPumpWidget(componentMiner.Inventory, componentDispenser);
					AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					return true;
				}
			}
			return false;
		}
	}
}