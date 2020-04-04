using Engine;
using System.Collections.Generic;
using TemplatesDatabase;
using Engine.Serialization;
using System.Linq;

namespace Game
{
	public class SubsystemPMachsBlockBehavior : SubsystemCombinedBlockBehavior<ComponentMachine>
	{
		public SubsystemPMachsBlockBehavior() : base(new Dictionary<int, string>
		{
			{ PresserBlock.Index, "Presser" },
			{ KibblerBlock.Index, "Kibbler" },
			{ PresserNNBlock.Index, "RiflingMachine" },
			{ SqueezerBlock.Index, "Squeezer" }
		})
		{
		}

		public override int[] HandledBlocks => new[] {
			PresserBlock.Index,
			KibblerBlock.Index,
			PresserNNBlock.Index,
			SqueezerBlock.Index
		};

		public override Widget GetWidget(IInventory inventory, ComponentMachine component, string path)
		{
			return new PresserWidget(inventory, component);
		}
	}

	public class SubsystemSourBlockBehavior : SubsystemInventoryBlockBehavior<ComponentMachine>
	{
		public SubsystemSourBlockBehavior() : base("Sour")
		{
		}
		public DynamicArray<Vector4> m_radations = new DynamicArray<Vector4>();
		public DynamicArray<Vector4> m_radio = new DynamicArray<Vector4>();

		public float FindNearestCompassTarget(Vector3 Position)
		{
			if (m_radations.Count > 0)
			{
				float num4 = 0;
				for (int i = 0; i < m_radations.Count; i++)
				{
					Vector4 vector = m_radations.Array[i];
					float num3 = vector.W / Vector3.DistanceSquared(Position, new Vector3(vector.X, vector.Y, vector.Z));
					if (num3 > num4)
					{
						num4 = num3;
					}
				}
				return num4;
			}
			return 0;
		}

		public SubsystemBlockEntities m_subsystemBlockEntities;

		public static string[] Names = new[]
		{
			"Sour",
			"Deposit",
			"Sorter",
			"SChest",
			"RCore",
			"RHead",
			null,
			"FRCore"
		};

		public override int[] HandledBlocks => new[] { SourBlock.Index };

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			Name = Names[Terrain.ExtractData(value) >> 10];
			base.OnBlockAdded(value, oldValue, x, y, z);
		}

		public SubsystemPickables m_subsystemPickables;

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (worldItem.ToRemove || Terrain.ExtractData(Utils.Terrain.GetCellValueFast(cellFace.X, cellFace.Y, cellFace.Z)) < 2048)
			{
				return;
			}
			ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
			if (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(cellFace.X, cellFace.Y, cellFace.Z)) == 3 * 1024)
			{
				//ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(cellFace.X, cellFace.Y, cellFace.Z);
				if (blockEntity != null)
				{
					var pickable = worldItem as Pickable;
					int num = pickable?.Count ?? 1;
					int num2 = ComponentInventoryBase.AcquireItems(blockEntity.Entity.FindComponent<ComponentSorter>(true), worldItem.Value, num);
					if (num2 <= 0)
					{
						worldItem.ToRemove = true;
					}
					else if (pickable != null)
					{
						pickable.Count = num2;
					}
				}
				return;
			}
			if (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(cellFace.X, cellFace.Y, cellFace.Z)) > 3 * 1024)
			{
				return;
			}

			if (blockEntity != null)
			{
				var pickable = worldItem as Pickable;
				int num = pickable?.Count ?? 1;
				if (num > 1)
				{
					return;
				}
				var position = new Vector3(cellFace.Point) + new Vector3(0.5f);
				ComponentSorter inventory = blockEntity.Entity.FindComponent<ComponentSorter>(true);

				Vector3 v = CellFace.FaceToVector3(cellFace.Face);
				if (inventory.GetSlotValue(0) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(0f, 0f, 1f);
					if (Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position + 0.75f * v, 10f * v, Vector3.Zero, null) == null)
					{
						m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * v, null);
					}
					return;
				}
				if (inventory.GetSlotValue(1) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(0f, 0f, -1f);
					if (Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position + 0.75f * v, 10f * v, Vector3.Zero, null) == null)
					{
						m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * v, null);
					}
					return;
				}
				if (inventory.GetSlotValue(2) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(1f, 0f, 0f);
					if (Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position + 0.75f * v, 10f * v, Vector3.Zero, null) == null)
					{
						m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * v, null);
					}
					return;
				}
				if (inventory.GetSlotValue(3) == worldItem.Value)
				{
					worldItem.ToRemove = true;
					v = new Vector3(-1f, 0f, 0f);
					if (Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position + 0.75f * v, 10f * v, Vector3.Zero, null) == null)
					{
						m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * v, null);
					}
					return;
				}
				v = -CellFace.FaceToVector3(cellFace.Face);
				worldItem.ToRemove = true;
				if (Utils.SubsystemProjectiles.FireProjectile(worldItem.Value, position + 0.75f * v, 10f * v, Vector3.Zero, null) == null)
				{
					m_subsystemPickables.AddPickable(worldItem.Value, 1, position + 0.75f * v, 1f * v, null);
				}
				return;
			}
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(throwOnError: true);
			m_subsystemPickables = Project.FindSubsystem<SubsystemPickables>(throwOnError: true);
			m_radations = new DynamicArray<Vector4>(HumanReadableConverter.ValuesListFromString<Vector4>(';', valuesDictionary.GetValue("Radiation", "")));
			m_radio = new DynamicArray<Vector4>(HumanReadableConverter.ValuesListFromString<Vector4>(';', valuesDictionary.GetValue("Radio", "")));
		}
		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			string value = HumanReadableConverter.ValuesListToString(';', m_radations.ToArray());
			string value2 = HumanReadableConverter.ValuesListToString(';', m_radio.ToArray());
			valuesDictionary.SetValue("Radiation", value);
			valuesDictionary.SetValue("Radio", value2);
		}
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int type = Terrain.ExtractData(Utils.Terrain.GetCellValueFast(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z)) >> 10;
			if (base.OnInteract(raycastResult, componentMiner) && type != 0 && componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget is StoveWidget widget && type != 7)
				widget.Children.Find<LabelWidget>("Label", false).Text = Utils.Get(SourBlock.Names[type]) ;
			if (type == 6)
				return false;
			return true;
		}

		public override Widget GetWidget(IInventory inventory, ComponentMachine component)
		{
			var c = component.Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
			switch (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(c.X, c.Y, c.Z)) >> 10)
			{
				case 0:
					return new SeparatorWidget(inventory, component, "Sour", "Widgets/SourWidget");
				case 1:
					return new SeparatorWidget(inventory, component, "Deposit");
				case 2:
					return new SorterWidget(inventory, component);
				case 3:
					return new SteelChestWidget(inventory, component);
				case 4:
					return new NewChestWidget(inventory, component ,"ReactorCore");
				case 5:
					return new NewChestWidget(inventory, component ,"RactorHead");
				case 6:
					return null;
				case 7:
					return new NewChestWidget(inventory, component, "HotNeutronReactor");
			}
			return null;
		}
	}

	public class SubsystemCastMachBlockBehavior : SubsystemFurnaceBlockBehavior<ComponentCastMach>
	{
		public override int[] HandledBlocks => new[] { CastMachBlock.Index };

		public SubsystemCastMachBlockBehavior() : base("CastMach")
		{
		}

		public override Widget GetWidget(IInventory inventory, ComponentCastMach component)
		{
			return new CastMachWidget(inventory, component);
		}
	}

	public class SubsystemCReactorBlockBehavior : SubsystemInventoryBlockBehavior<ComponentCReactor>
	{
		public SubsystemCReactorBlockBehavior() : base("CReactor")
		{
		}

		public override int[] HandledBlocks => new[] { CReactorBlock.Index };

		public override Widget GetWidget(IInventory inventory, ComponentCReactor component)
		{
			return new CReactorWidget(inventory, component);
		}
	}

	public class SubsystemUnloaderBlockBehavior : SubsystemInventoryBlockBehavior<ComponentInventoryBase>
	{
		public override int[] HandledBlocks => new[] { Bullet2Block.Index };

		public static string[] Names = new[]
		{
			"Unloader",
			"Inserter",
			"SInserter",
		};

		public SubsystemUnloaderBlockBehavior() : base("Unloader")
		{
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			Name = Names[(Terrain.ExtractData(value) >> 10) - 1];
			base.OnBlockAdded(value, oldValue, x, y, z);
		}

		//public override Widget GetWidget(IInventory inventory, ComponentUnloader component)
		//{
		//	return new NewChestWidget(inventory, component, "Unloader");
		//}
		public override Widget GetWidget(IInventory inventory, ComponentInventoryBase component)
		{
			var c = component.Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
			switch (Terrain.ExtractData(Utils.Terrain.GetCellValueFast(c.X, c.Y, c.Z)) >> 10)
			{
				case 0:
					return null;
				case 1:
					return new NewChestWidget(inventory, component, "Unloader");
				case 2:
					return new NewChestWidget(inventory, component, "Inserter", "Widgets/NewChest2Widget");
				case 3:
					var b = component.Entity.FindComponent<ComponentSInserter>(true);
					return new SIWidget(inventory, b);
			}
			return null;
		}
	}
}