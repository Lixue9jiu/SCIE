using Engine;

namespace Game
{
	public class SubsystemMachineToolBlockBehavior : SubsystemInventoryBlockBehavior<ComponentLargeCraftingTable>
	{
		public SubsystemMachineToolBlockBehavior() : base("MachineTool")
		{
		}

		public override int[] HandledBlocks => new[] { MachineToolBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentLargeCraftingTable component)
		{
			return new MachineToolWidget(inventory, component);
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
			{
				return;
			}
			var blockEntity = Utils.GetBlockEntity(cellFace.Point);
			if (blockEntity == null)
			{
				return;
			}
			var inventory = blockEntity.Entity.FindComponent<ICraftingMachine>(true);
			var pickable = worldItem as Pickable;
			int value = worldItem.Value;
			int count = (pickable == null) ? 1 : pickable.Count, count2 = MathUtils.Min(count, inventory.GetSlotCapacity(inventory.SlotIndex, value) - inventory.GetSlotCount(inventory.SlotIndex));
			if (inventory.GetSlotCount(inventory.SlotIndex) != 0 && inventory.GetSlotValue(inventory.SlotIndex) != value)
			{
				return;
			}
			inventory.AddSlotItems(inventory.SlotIndex, value, count2);
			if (count2 < count)
			{
				Utils.SubsystemAudio.PlaySound("Audio/PickableCollected", 1f, 0f, worldItem.Position, 3f, true);
			}
			if (count - count2 <= 0)
			{
				worldItem.ToRemove = true;
			}
			else if (pickable != null)
			{
				pickable.Count = count - count2;
			}
		}
	}
}