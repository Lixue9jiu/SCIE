using Engine;
using System.Collections.Generic;

namespace Game
{
	public class SubsystemHPressBlockBehavior : SubsystemInventoryBlockBehavior<ComponentHPress>
	{
		public SubsystemHPressBlockBehavior() : base(null) { }

		public override int[] HandledBlocks => new[] { MetalBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentHPress component)
		{
			return new PresserWidget<ComponentHPress>(inventory, component);
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var cellFace = raycastResult.CellFace;
			int x = cellFace.X,
				y = cellFace.Y,
				z = cellFace.Z;
			if (y < 2 || y > 96 || MetalBlock.GetType(SubsystemTerrain.Terrain.GetCellValueFast(x, y, z)) != 1)
				return false;
			var dir = CellFace.FaceToPoint3(CellFace.OppositeFace(cellFace.Face));
			x += dir.X;
			z += dir.Z;
			TerrainChunk chunk = Utils.Terrain.GetChunkAtCell(x, z);
			if (chunk == null)
				return false;
			int d = TerrainChunk.CalculateCellIndex(x & 15, y, z & 15), i;
			const int Steel = MetalBlock.Index | 3 << 5 + 14, Cr = MetalBlock.Index | 10 << 5 + 14,
				Case = MetalBlock.Index | 1 << 5 + 14, Mask = 1023 | 15 << 5+14,
				Hg = RottenMeatBlock.Index | 6 << 4 + 14, Wall = MetalBlock.Index | 15 << 5 + 14;
			if ((chunk.GetCellValueFast(d - 2) ^ Steel & Mask) != 0 || Terrain.ExtractContents(chunk.GetCellValueFast(d - 1)) != MagmaBlock.Index || Terrain.ReplaceLight(chunk.GetCellValueFast(d) ^ Cr, 0) != 0)
				return false;
			for (i = 1; i <= 32; i++)
			{
				if (Terrain.ReplaceLight(chunk.GetCellValueFast(d + i) ^ Hg, 0) != 0)
					return false;
			}
			for (int j = 0; j < 4; j++)
			{
				dir = CellFace.FaceToPoint3(j);
				chunk = Utils.Terrain.GetChunkAtCell(x + dir.X, z + dir.Z);
				if (chunk == null)
					return false;
				d = TerrainChunk.CalculateCellIndex(x + dir.X & 15, y, z + dir.Z & 15);
				if (((chunk.GetCellValueFast(d - 2) ^ Steel & Mask) | (chunk.GetCellValueFast(d - 1) ^ Steel & Mask) |
					(chunk.GetCellValueFast(d) ^ Case & Mask) | (chunk.GetCellValueFast(d + 1) ^ Cr & Mask)) != 0)
					return false;
				d++;
				for (i = 1 ; i <= 7; i++)
				{
					if ((chunk.GetCellValueFast(d + i) ^ Steel & Mask) != 0)
						return false;
				}
				d += i;
				for (i = 0; i < 24; i++)
				{
					if ((chunk.GetCellValueFast(d + i) ^ Wall & Mask) != 0)
						return false;
				}
			}
			for (i = 0; i < 2; i++)
			{
				y--;
				if ((Utils.Terrain.GetCellValueFast(x + 1, y, z + 1) ^ Steel & Mask) != 0 || (Utils.Terrain.GetCellValueFast(x + 1, y, z - 1) ^ Steel & Mask) != 0 ||
					(Utils.Terrain.GetCellValueFast(x - 1, y, z + 1) ^ Steel & Mask) != 0 || (Utils.Terrain.GetCellValueFast(x - 1, y, z - 1) ^ Steel & Mask) != 0)
					return false;
			}
			if (Utils.GetBlockEntity(raycastResult.CellFace.Point) == null)
				Project.CreateBlockEntity("HPress", raycastResult.CellFace.Point);
			return base.OnInteract(raycastResult, componentMiner);
		}
	}

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

	public class SubsystemDiversionBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks => new[] { IceBlock.Index, DiversionBlock.Index };

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove)
				return;
			int value = SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			if (Terrain.ExtractContents(value) != DiversionBlock.Index) return;
			Vector3 v = CellFace.FaceToVector3(value - DiversionBlock.Index >> 14);
			Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, new Vector3(cellFace.X, cellFace.Y, cellFace.Z) + new Vector3(0.5f) + 0.75f * v, 30f * v, Vector3.Zero, null);
			worldItem.ToRemove = true;
		}

		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			int x = cellFace.X,
				y = cellFace.Y,
				z = cellFace.Z;
			if (Utils.Terrain.GetCellContentsFast(x, y, z) != IceBlock.Index || componentBody.Mass < 999f || componentBody.Density > 1f)
				return;
			SubsystemTerrain.DestroyCell(0, x, y, z, WaterBlock.Index, false, false);
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
		public SubsystemLiquidPumpBlockBehavior()
		{
			Name = "LiquidPump";
		}

		public override int[] HandledBlocks => new[] { LiquidPumpBlock.Index };

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			if (Utils.SubsystemGameInfo.WorldSettings.GameMode != GameMode.Adventure)
			{
				var blockEntity = Utils.GetBlockEntity(raycastResult.CellFace.Point);
				if (blockEntity != null && componentMiner.ComponentPlayer != null)
				{
					componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new LiquidPumpWidget(componentMiner.Inventory, blockEntity.Entity.FindComponent<ComponentDriller>(true));
					AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
					return true;
				}
			}
			return false;
		}
	}

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