using Engine;

namespace Game
{
	public class SubsystemMachineToolBlockBehavior : SubsystemInventoryBlockBehavior<ComponentLargeCraftingTable>
	{
		public SubsystemMachineToolBlockBehavior() : base("MachineTool")
		{
		}

		public override int[] HandledBlocks
		{
			get
			{
				return new []
				{
					MachineToolBlock.Index
				};
			}
		}

		public override Widget GetWidget(IInventory inventory, ComponentLargeCraftingTable component)
		{
			return new MachineToolWidget(inventory, component);
		}
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!worldItem.ToRemove)
			{
				ComponentBlockEntity blockEntity = Utils.SubsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
				if (blockEntity != null)
				{
					ComponentLargeCraftingTable inventory = blockEntity.Entity.FindComponent<ComponentLargeCraftingTable>(true);
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
	}
}
