using Engine;
using System.Collections.Generic;

namespace Game
{
	public class SubsystemCrusherBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks => new[] { CrusherBlock.Index };

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!ComponentEngine.IsPowered(Utils.Terrain, cellFace.X, cellFace.Y, cellFace.Z) || worldItem.Velocity.Length() < 20f)
				return;
			int l;
			Vector3 v = CellFace.FaceToVector3(cellFace.Face);
			var position = new Vector3(cellFace.Point) + new Vector3(0.5f) - 0.75f * v;
			if (Terrain.ExtractContents(worldItem.Value) == 6)
			{
				for (l = 0; l < 5; l++)
					Utils.SubsystemProjectiles.FireProjectile(79, position, -20f * v, Vector3.Zero, null);
				worldItem.ToRemove = true;
			}
			else
			{
				var list = new List<BlockDropValue>(8);
				BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].GetDropValues(SubsystemTerrain, worldItem.Value, 0, 3, list, out bool s);
				for (l = 0; l < list.Count; l++)
				{
					var blockDropValue = list[l];
					for (int i = 0; i <= blockDropValue.Count; i++)
						Utils.SubsystemProjectiles.FireProjectile(blockDropValue.Value, position, -20f * v, Vector3.Zero, null);
				}
				worldItem.ToRemove = true;
			}
		}
	}

	public class SubsystemSpinnerBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks => new[] { SpinnerBlock.Index };

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!ComponentEngine.IsPowered(Utils.Terrain, cellFace.X, cellFace.Y, cellFace.Z))
				return;
			//int num1 = SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			//int num2 = SubsystemTerrain.Terrain.GetCellContents(cellFace.X, cellFace.Y, cellFace.Z);
			Vector3 v = CellFace.FaceToVector3(cellFace.Face);
			var position = new Vector3(cellFace.Point) + new Vector3(0.5f) - 0.75f * v;
			if (Terrain.ExtractContents(worldItem.Value) == CottonWadBlock.Index)
			{
				// if ((num1 + 1) % 10 == 3)
				// {
				//     int value = Terrain.ReplaceData(num2, 0);
				//     base.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value);
				Utils.SubsystemProjectiles.FireProjectile(StringBlock.Index, position, -1f * v, Vector3.Zero, null);
				worldItem.ToRemove = true;
				//  }else
				//  {
				//     int value = Terrain.ReplaceData(num2, num1+1);
				//      base.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value);
				//      worldItem.ToRemove = true;
				//  }
			}
			else if (Terrain.ExtractContents(worldItem.Value) == StringBlock.Index)
			{
				Utils.SubsystemProjectiles.FireProjectile(CanvasBlock.Index, position, -1f * v, Vector3.Zero, null);
				worldItem.ToRemove = true;
			}
			else if (Terrain.ExtractContents(worldItem.Value) == CanvasBlock.Index)
			{
				Utils.SubsystemProjectiles.FireProjectile(CarpetBlock.Index, position, -1f * v, Vector3.Zero, null);
				worldItem.ToRemove = true;
			}
		}
	}
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

	public class SubsystemMachineToolBlockBehavior : SubsystemInventoryBlockBehavior<ComponentLargeCraftingTable>
	{
		public SubsystemMachineToolBlockBehavior() : base("MachineTool") { }

		public override int[] HandledBlocks => new[] { MachineToolBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentLargeCraftingTable component)
		{
			return new MachineToolWidget(inventory, component);
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
				return;
			var blockEntity = Utils.GetBlockEntity(cellFace.Point);
			if (blockEntity == null)
				return;
			var inventory = blockEntity.Entity.FindComponent<ICraftingMachine>(true);
			var pickable = worldItem as Pickable;
			int value = worldItem.Value;
			int count = (pickable == null) ? 1 : pickable.Count, count2 = MathUtils.Min(count, inventory.GetSlotCapacity(inventory.SlotIndex, value) - inventory.GetSlotCount(inventory.SlotIndex));
			if (inventory.GetSlotCount(inventory.SlotIndex) != 0 && inventory.GetSlotValue(inventory.SlotIndex) != value)
				return;
			inventory.AddSlotItems(inventory.SlotIndex, value, count2);
			if (count2 < count)
				Utils.SubsystemAudio.PlaySound("Audio/PickableCollected", 1f, 0f, worldItem.Position, 3f, true);
			if (count - count2 <= 0)
				worldItem.ToRemove = true;
			else if (pickable != null)
				pickable.Count = count - count2;
		}
	}
}