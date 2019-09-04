using GameEntitySystem;
using System;
using System.Globalization;
using System.Text;
using TemplatesDatabase;

namespace Game
{
	public class ComponentNewChest : ComponentInventoryBase
	{
		public bool Powered;

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
		}
	}

	public partial class Utils
	{
		public static void LoadItems(this ComponentInventoryBase inventory, ValuesDictionary valuesDictionary)
		{
			string s = valuesDictionary.GetValue<string>("Contents", null);
			int i, count = valuesDictionary.GetValue("SlotsCount", 2);
			ComponentInventoryBase.Slot slot;
			if (s != null)
			{
				var arr = s.Split(';');
				count = Math.Max(arr.Length, count);
				for (i = 0; i < count; i++)
				{
					slot = new ComponentInventoryBase.Slot();
					inventory.m_slots.Add(slot);
					s = arr[i];
					if (s.Length > 0)
					{
						int x = s.IndexOf('*');
						slot.Value = int.Parse(x > 0 ? s.Substring(0, x) : s);
						slot.Count = x > 0 ? int.Parse(s.Substring(x + 1)) : 1;
					}
				}
				return;
			}
			inventory.m_slots.Capacity = count;
			ValuesDictionary value2 = valuesDictionary.GetValue<ValuesDictionary>("Slots");
			for (i = 0; i < count; i++)
			{
				slot = new ComponentInventoryBase.Slot();
				inventory.m_slots.Add(slot);
				ValuesDictionary value3 = value2.GetValue<ValuesDictionary>("Slot" + i.ToString(CultureInfo.InvariantCulture), null);
				if (value3 != null)
				{
					slot.Value = value3.GetValue<int>("Contents");
					slot.Count = value3.GetValue<int>("Count");
				}
			}
		}

		public static void SaveItems(this ComponentInventoryBase inventory, ValuesDictionary valuesDictionary)
		{
			var sb = new StringBuilder();
			int i = 0;
			for (; i < inventory.m_slots.Count; i++)
			{
				var slot = inventory.m_slots[i];
				if (slot.Count > 0)
				{
					sb.Append(slot.Value);
					if (slot.Count > 1)
						sb.Append('*').Append(slot.Count);
				}
				sb.Append(';');
			}
			if (i != 0)
				valuesDictionary.SetValue("Contents", sb.ToString(0, sb.Length - 1));
		}
	}
}