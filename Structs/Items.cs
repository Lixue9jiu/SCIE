using Engine;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Game
{
	public partial class ItemBlock : CubeBlock, IItemBlock
	{
		public override void Initialize()
		{
			DefaultItem = new Item();
            Items = new Item[]
            {
                new IronLine(),
                new CopperLine(),
                new SteelLine(),
                new GoldOreChunk(),
                new SliverOreChunk(),
                new PlatinumOreChunk()
            };
            IdTable = new Dictionary<string, int>(Items.Length);
			int i;
			for (i = 0; i < Items.Length; i++)
			{
				IdTable.Add(Items[i].GetType().ToString().Substring(5), i);
			}
           
            /*var streamReader = new StreamReader(CustomTextureBlock.GetTargetFile("IndustrialMod.icsv"));
			try
			{
				LoadBlocksData(streamReader.ReadToEnd());
			}
			catch (Exception e)
			{
				Log.Warning(string.Format("\"IndustrialMod.icsv\": {0}", e));
			}
			finally
			{
				streamReader.Dispose();
			}*/
            CraftingRecipesManager.DecodeResult1 = DecodeResult;
			base.Initialize();
		}
		public static void LoadBlocksData(string data)
		{
			string[] array = data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = null;
			for (int i = 0; i < array.Length; i++)
			{
				string[] array3 = array[i].Split(';');
				if (i == 0)
				{
					array2 = new string[array3.Length - 1];
					Array.Copy(array3, 1, array2, 0, array3.Length - 1);
					continue;
				}
				if (array3.Length != array2.Length + 1)
				{
					throw new InvalidOperationException(string.Format("Not enough field values for block \"{0}\".", (array3.Length != 0) ? array3[0] : "unknown"));
				}
				string typeName = array3[0];
				if (string.IsNullOrEmpty(typeName))
					continue;
				Item item;
				if (IdTable.TryGetValue(typeName, out int index))
				{
					item = Items[index];
				}
				else
				{
					throw new InvalidOperationException(string.Format("Block \"{0}\" not found when loading block data.", typeName));
				}
				Dictionary<string, FieldInfo> dictionary = new Dictionary<string, FieldInfo>();
				foreach (FieldInfo runtimeField in Items[i].GetType().GetRuntimeFields())
				{
					if (runtimeField.IsPublic && !runtimeField.IsStatic)
					{
						dictionary.Add(runtimeField.Name, runtimeField);
					}
				}
				for (int j = 1; j < array3.Length; j++)
				{
					string text = array2[j - 1];
					string text2 = array3[j];
					if (!string.IsNullOrEmpty(text2))
					{
						if (!dictionary.TryGetValue(text, out FieldInfo value))
						{
							throw new InvalidOperationException(string.Format("Field \"{0}\" not found or not accessible when loading block data.", text));
						}
						object obj = null;
						if (text2[0] == '#')
						{
							string refTypeName = text2.Substring(1);
							if (string.IsNullOrEmpty(refTypeName))
							{
								obj = Terrain.MakeBlockValue(Index, 0, i);
							}
							else
							{
								Block block2 = BlocksManager.FindBlockByTypeName(refTypeName, false);
								if (block2 == null)
								{
									if (IdTable.TryGetValue(refTypeName, out int n))
									{
										obj = Terrain.MakeBlockValue(Index, 0, n);
									}
									else
									{
										throw new InvalidOperationException(string.Format("Reference block \"{0}\" not found when loading block data.", refTypeName));
									}
								}
								else
								{
									obj = block2.BlockIndex;
								}
							}
						}
						else
						{
							obj = HumanReadableConverter.ConvertFromString(value.FieldType, text2);
						}
						value.SetValue(item, obj);
					}
				}
			}
		}
	}
}
