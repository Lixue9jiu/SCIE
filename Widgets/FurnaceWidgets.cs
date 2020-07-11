namespace Game
{
	public class FurnaceNWidget : EntityWidget<ComponentFurnace>
	{
		protected readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_fuelSlot;

		protected readonly ValueBarWidget m_progress;

		protected readonly InventorySlotWidget m_remainsSlot,
												m_resultSlot;

		public FurnaceNWidget(IInventory inventory, ComponentFurnace component) : base(inventory, component, "Widgets/FurnaceNWidget")
		{
			m_fire = Children.Find<FireWidget>("Fire");
			m_progress = Children.Find<ValueBarWidget>("Progress");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			m_fuelSlot = Children.Find<InventorySlotWidget>("FuelSlot");
			InitGrid();
			m_fuelSlot.AssignInventorySlot(component, component.FuelSlotIndex);
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			base.Update();
		}
	}

	public class FurnaceWidget<T> : EntityWidget<T> where T : ComponentInventoryBase
	{
		protected readonly FireWidget m_fire;

		protected readonly InventorySlotWidget m_remainsSlot,
												m_resultSlot;

		protected readonly CheckboxWidget m_acceptsDropsBox;
		protected readonly ValueBarWidget m_progress;

		protected readonly InventorySlotWidget m_cir1,
												m_cir2;

		public FurnaceWidget(IInventory inventory, T component, string path) : base(inventory, component, path)
		{
			m_fire = Children.Find<FireWidget>("Fire");
			m_resultSlot = Children.Find<InventorySlotWidget>("ResultSlot");
			m_remainsSlot = Children.Find<InventorySlotWidget>("RemainsSlot");
			//m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsElectBox");
			m_cir1 = Children.Find<InventorySlotWidget>("CircuitSlot1", false);
			m_cir2 = Children.Find<InventorySlotWidget>("CircuitSlot2", false);
			m_progress = Children.Find<ValueBarWidget>("Progress");
			InitGrid();
		}
	}

	public class ElectricFurnaceWidget : FurnaceWidget<ComponentElectricFurnace>
	{
		public ElectricFurnaceWidget(IInventory inventory, ComponentElectricFurnace component, string path = "Widgets/ElectricFurnaceWidget") : base(inventory, component, path)
		{
			m_resultSlot.AssignInventorySlot(component, component.ResultSlotIndex);
			m_remainsSlot.AssignInventorySlot(component, component.RemainsSlotIndex);
			m_cir1.AssignInventorySlot(component, component.Cir1SlotIndex);
			m_cir2.AssignInventorySlot(component, component.Cir2SlotIndex);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			base.Update();
		}
	}

	public class GasWidget : FurnaceWidget<ComponentMachine>
	{
		protected readonly ValueBarWidget m_progress2;
		public GasWidget(IInventory inventory, ComponentMachine component, string path = "Widgets/GasWidget") : base(inventory, component, path)
		{
			m_resultSlot.AssignInventorySlot(component, 2);
			m_remainsSlot.AssignInventorySlot(component, 3);
			m_cir1.AssignInventorySlot(component, 4);
			m_cir2.AssignInventorySlot(component, 5);
			m_progress2 = Children.Find<ValueBarWidget>("Progress2");
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			m_progress2.Value = m_component.m_fireTimeRemaining/10000f;
			if (m_component.GetSlotCount(4)>0 && m_component.m_fireTimeRemaining<10000f)
			{
				var block = BlocksManager.Blocks[Terrain.ExtractContents(m_component.GetSlotValue(4))];
				if (block is IFuel fuel && block.BlockIndex != ElementBlock.Index)
				{
					m_component.m_fireTimeRemaining += fuel.GetFuelFireDuration(m_component.GetSlotValue(4));
					m_component.AddSlotItems(5, ItemBlock.IdTable["¸ÖÆ¿"], 1);
					m_component.RemoveSlotItems(4, 1);
				}
			}
			


			base.Update();
		}
	}

	public class VaFurnaceWidget : FurnaceWidget<ComponentVaFurnace>
	{
		public VaFurnaceWidget(IInventory inventory, ComponentVaFurnace component, string path = "Widgets/VaFurnaceWidget") : base(inventory, component, path)
		{
			m_resultSlot.AssignInventorySlot(component, 4);
			m_remainsSlot.AssignInventorySlot(component, 5);
			m_cir1.AssignInventorySlot(component, 6);
			m_cir2.AssignInventorySlot(component, 7);
		}

		public override void Update()
		{
			m_fire.ParticlesPerSecond = m_component.HeatLevel > 0f ? 24f : 0f;
			m_progress.Value = m_component.SmeltingProgress;
			base.Update();
		}
	}
}