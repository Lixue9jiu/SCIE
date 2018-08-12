using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemNRotBlockBehavior : SubsystemRotBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			m_subsystemItemsScanner = base.Project.FindSubsystem<SubsystemItemsScanner>(true);
			m_lastRotTime = valuesDictionary.GetValue<double>("LastRotTime");
			m_rotStep = valuesDictionary.GetValue<int>("RotStep");
			m_subsystemItemsScanner.ItemsScanned += ItemsScanned;
			m_isRotEnabled = (m_subsystemGameInfo.WorldSettings.GameMode != 0 && m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Adventure);
		}

		public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			valuesDictionary.SetValue("LastRotTime", m_lastRotTime);
			valuesDictionary.SetValue("RotStep", m_rotStep);
		}

		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			if (m_isRotEnabled)
			{
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				int rotPeriod = block.GetRotPeriod(value);
				if (rotPeriod > 0 && pollPass % rotPeriod == 0)
				{
					int num2 = block.GetDamage(value) + 1;
					value = ((num2 > 1) ? block.GetDamageDestructionValue(value) : block.SetDamage(value, num2));
					base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				}
			}
		}

		private void ItemsScanned(ReadOnlyList<ScannedItemData> items)
		{
			int num = (int)((m_subsystemGameInfo.TotalElapsedGameTime - m_lastRotTime) / 60.0);
			if (num > 0)
			{
				if (m_isRotEnabled)
				{
					foreach (ScannedItemData item in items)
					{
						Block block = BlocksManager.Blocks[Terrain.ExtractContents(item.Value)];
						int rotPeriod = block.GetRotPeriod(item.Value);
						int num2 = 1;
						if (item.Container.ToString() == "Game.ComponentChestNew")
						{
							num2 = 4;
						}
						if (rotPeriod > 0)
						{
							int num3 = block.GetDamage(item.Value);
							for (int i = 0; i < num; i++)
							{
								if (num3 > 1)
								{
									break;
								}
								if ((i + m_rotStep) % (rotPeriod * num2) == 0)
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
				m_lastRotTime += (double)num * 60.0;
			}
		}
	}
}