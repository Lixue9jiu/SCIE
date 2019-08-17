using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemNRotBlockBehavior : SubsystemRotBlockBehavior
	{
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemItemsScanner.ItemsScanned -= base.ItemsScanned;
			m_subsystemItemsScanner.ItemsScanned += ItemsScanned;
		}

		public new void ItemsScanned(ReadOnlyList<ScannedItemData> items)
		{
			int num = (int)((m_subsystemGameInfo.TotalElapsedGameTime - m_lastRotTime) / 60.0);
			if (num > 0)
			{
				if (m_isRotEnabled)
				{
					for (int i = 0; i < items.Count; i++)
					{
						ScannedItemData item = items[i];
						var block = BlocksManager.Blocks[Terrain.ExtractContents(item.Value)];
						int rotPeriod = block.GetRotPeriod(item.Value);
						if (rotPeriod > 0)
						{
							int num2 = item.Container is ComponentNewChest chestNew && chestNew.Powered ? 4 : 1;
							int num3 = block.GetDamage(item.Value);
							for (int j = 0; j < num; j++)
							{
								if (num3 > 1)
									break;
								if ((j + m_rotStep) % (rotPeriod * num2) == 0)
									num3++;
							}
							if (num3 <= 1)
								m_subsystemItemsScanner.TryModifyItem(item, block.SetDamage(item.Value, num3));
							else
								m_subsystemItemsScanner.TryModifyItem(item, block.GetDamageDestructionValue(item.Value));
						}
					}
				}
				m_rotStep += num;
				m_lastRotTime += num * 60.0;
			}
		}
	}

	public class SubsystemNSoilBlockBehavior : SubsystemSoilBlockBehavior, IUpdateable
	{
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (cellFace.Y < 0 || cellFace.Y > 127 || worldItem.Velocity.Length() < 10f)
				return;
			int value = Terrain.ExtractContents(worldItem.Value);
			if (value == 102)
			{
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						int cellValueFast = SubsystemTerrain.Terrain.GetCellValueFast(cellFace.X + i, cellFace.Y, cellFace.Z + j);
						if (Terrain.ExtractContents(cellValueFast) == 168)
						{
							SubsystemTerrain.ChangeCell(cellFace.X + i, cellFace.Y, cellFace.Z + j, 168 | SoilBlock.SetNitrogen(Terrain.ExtractData(cellValueFast), 3) << 14);
							worldItem.ToRemove = true;
						}
					}
				}
			}
			else if (value == SeedsBlock.Index && SubsystemTerrain.Terrain.GetCellContentsFast(cellFace.X, cellFace.Y + 1, cellFace.Z) == 0)
			{
				value = 0;
				switch (worldItem.Value)
				{
					case 16557:
						value = 20 | FlowerBlock.SetIsSmall(0, true) << 14;
						break;
					case 173:
						value = 19 | TallGrassBlock.SetIsSmall(0, true) << 14;
						break;
					case 49325:
						value = 25 | FlowerBlock.SetIsSmall(0, true) << 14;
						break;
					case 32941:
						value = 24 | FlowerBlock.SetIsSmall(0, true) << 14;
						break;
					case 82093:
						value = 174 | RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0) << 14;
						break;
					case 65709:
						value = 174 | RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0) << 14;
						break;
					case 114861:
						value = 131 | BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, false), 0) << 14;
						break;
					case 98477:
						value = 204 | CottonBlock.SetSize(CottonBlock.SetIsWild(0, false), 0) << 14;
						break;
				}
				if (value != 0)
				{
					SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y + 1, cellFace.Z, value);
					worldItem.ToRemove = true;
				}
			}
		}
	}
}