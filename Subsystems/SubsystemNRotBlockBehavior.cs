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

		protected new void ItemsScanned(ReadOnlyList<ScannedItemData> items)
		{
			int num = (int)((m_subsystemGameInfo.TotalElapsedGameTime - m_lastRotTime) / 60.0);
			if (num > 0)
			{
				if (m_isRotEnabled)
				{
					for (int i = 0; i < items.Count; i++)
					{
						ScannedItemData item = items[i];
						Block block = BlocksManager.Blocks[Terrain.ExtractContents(item.Value)];
						int rotPeriod = block.GetRotPeriod(item.Value);
						if (rotPeriod > 0)
						{
							int num2 = item.Container is ComponentChestNew chestNew && chestNew.Powered ? 4 : 1;
							int num3 = block.GetDamage(item.Value);
							for (int j = 0; j < num; j++)
							{
								if (num3 > 1)
								{
									break;
								}
								if ((j + m_rotStep) % (rotPeriod * num2) == 0)
								{
									num3++;
								}
							}
							if (num3 <= 1)
							{
								m_subsystemItemsScanner.TryModifyItem(item, block.SetDamage(item.Value, num3));
							}
							else
							{
								m_subsystemItemsScanner.TryModifyItem(item, block.GetDamageDestructionValue(item.Value));
							}
						}
					}
				}
				m_rotStep += num;
				m_lastRotTime += num * 60.0;
			}
		}
	}
}
