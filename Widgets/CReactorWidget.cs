namespace Game
{
	public class CReactorWidget : ProcessWidget<ComponentMachine>
	{
		//protected readonly FireWidget m_fire;

		//protected readonly InventorySlotWidget m_fuelSlot;

		public CReactorWidget(IInventory inventory, ComponentMachine component) : base(inventory, component, "Widgets/CReactorWidget")
		{
			int num = InitGrid();
			m_result1.AssignInventorySlot(component, num);
			m_result2.AssignInventorySlot(component, num + 1);
			m_result3.AssignInventorySlot(component, num + 2);
		}

		public override void Update()
		{
			m_progress.Value = m_component.SmeltingProgress;
			base.Update();
		}
	}
}