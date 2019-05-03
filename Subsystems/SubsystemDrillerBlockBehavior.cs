namespace Game
{
	public class SubsystemDrillerBlockBehavior : SubsystemInventoryBlockBehavior<ComponentDriller>
	{
		public SubsystemDrillerBlockBehavior() : base("Driller")
		{
		}

		public override int[] HandledBlocks => new[] { DrillerBlock.Index };

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			return Utils.SubsystemGameInfo.WorldSettings.GameMode != GameMode.Adventure && base.OnInteract(raycastResult, componentMiner);
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (SixDirectionalBlock.GetAcceptsDrops(Terrain.ExtractData(Utils.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z))))
				Utils.OnHitByProjectile(cellFace, worldItem);
		}

		public override Widget GetWidget(IInventory inventory, ComponentDriller component)
		{
			return new DrillerWidget(inventory, component);
		}
	}
}