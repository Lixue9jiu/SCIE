namespace Game
{
	public class NewChestWidget : EntityWidget<ComponentInventoryBase>
	{
		public NewChestWidget(IInventory inventory, ComponentInventoryBase component, string text = "Freezer") : base(inventory, component, "Widgets/NewChestWidget")
		{
			Children.Find<LabelWidget>("ChestLabel").Text = text;
			if (Utils.TR.Count != 0)
				Children.Find<LabelWidget>("InventoryLabel").Text = "±³°ü";
			InitGrid("ChestGrid");
		}
	}
}