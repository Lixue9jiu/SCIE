using Engine;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game
{
	public partial class ItemBlock : CubeBlock, IItemBlock
	{
		public static Item DefaultItem = new Item();
		public static Item[] Items = new Item[]
        {
			new RottenEgg(),
			new MetalLine(MetalType.Iron),
			new MetalLine(MetalType.Copper),
			new MetalLine(MetalType.Steel),
			new MetalLine(MetalType.Gold),
			new MetalLine(MetalType.Sliver),
			new MetalLine(MetalType.Platinum),
			new MetalLine(MetalType.Lead),
			new MetalLine(MetalType.Stannary),
			new MetalLine(MetalType.Chromium),
			new MetalLine(MetalType.Aluminum),
			new Rod("RifleBarrel", Color.Gray, "Rifle Barrel are made by Rifling Machine. They are useful for making guns."),
			new ScrapIron(),
			new OreChunk(Matrix.CreateRotationX(4f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(255,215,0), false, MetalType.Gold),
			new OreChunk(Matrix.CreateRotationX(5f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(212,212,212), false, MetalType.Sliver),
			new OreChunk(Matrix.CreateRotationX(6f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(232,232,232), false, MetalType.Platinum),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(3f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(88, 87, 86), false, MetalType.Lead),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(4f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(65, 224, 205), false, MetalType.Zinc),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(5f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(225, 225, 225), false, MetalType.Stannary),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(6f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(227, 23, 13), false, MetalType.Mercury),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(7f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(90, 90, 90), false, MetalType.Chromium),
			new OreChunk(Matrix.CreateRotationX(2f) * Matrix.CreateRotationZ(-1f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(190, 190, 190), false, MetalType.Titanium),
			new OreChunk(Matrix.CreateRotationX(3f) * Matrix.CreateRotationZ(-1f), Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), new Color(120, 120, 120), false, MetalType.Nickel),
			new MetalIngot(MetalType.Steel),
			new MetalIngot(MetalType.Gold),
			new MetalIngot(MetalType.Sliver),
			new MetalIngot(MetalType.Platinum),
			new MetalIngot(MetalType.Lead),
			new MetalIngot(MetalType.Zinc),
			new MetalIngot(MetalType.Nickel),
			new MetalIngot(MetalType.Chromium),
			new MetalIngot(MetalType.Aluminum),
			new MetalIngot(MetalType.Stannary),
			new OrePowder(MetalType.Iron),
			new OrePowder(MetalType.Copper),
			new OrePowder(MetalType.Germanium),
			new OrePowder(MetalType.Gold),
			new OrePowder(MetalType.Sliver),
			new OrePowder(MetalType.Platinum),
			new OrePowder(MetalType.Lead),
			new OrePowder(MetalType.Stannary),
			new OrePowder(MetalType.Zinc),
			new OrePowder(MetalType.Chromium),
			new OrePowder(MetalType.Nickel),
			new OrePowder(MetalType.Aluminum),
			new Plate(MetalType.Steel),
			new Plate(MetalType.Iron),
			new Plate(MetalType.Copper),
			new Plate(MetalType.Gold),
			new Plate(MetalType.Sliver),
			new Plate(MetalType.Lead),
			new Plate(MetalType.Zinc),
			new Plate(MetalType.Stannary),
			new Plate(MetalType.Platinum),
			new Plate(MetalType.Aluminum),
			new SteamBoat(),
			new Train(),
			new Rod("steel", Color.LightGray),
			new Rod("copper", new Color(255, 127, 80)),
			new Rod("gold", new Color(255, 215, 0)),
			new Rod("sliver", new Color(253, 253, 253)),
			new Rod("lead", new Color(88, 87, 86)),
			new Rod("platinum", new Color(253, 253, 253)),
			new Rod("zinc", new Color(232, 232, 232)),
			new Rod("stannary", new Color(232, 232, 232)),
			new Rod("chromium", new Color(90, 90, 90)),
			new Rod("titanium", new Color(253, 253, 253)),
			new Rod("nickel", new Color(253, 253, 253)),
			new Rod("aluminum", new Color(232, 232, 232)),
			new Mould("Models/Gear", "Gear", Matrix.CreateTranslation(0f, 0f, 0f) * 2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "A Gear made of steel, the neccessary part of all the machine during the initial industrial era.", "SteelGear"),
			new Mould("Models/Wheel", "Wheel", Matrix.CreateTranslation(0f, 0f, 0f) * 1.2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "A wheel made of steel, the neccessary part of the steam engine train.", "SteelWheel", 2f),
			new Mould("Models/WheelMould", "WheelMould", Matrix.CreateTranslation(0f, -0.02f, 0f), Matrix.CreateTranslation(2.6f, 1.4f, 0f), "A wheel Mould made of dirt and sand, the neccessary part in making steel wheel.", "SteelWheelMould", 1.6f),
			new Mould("Models/GearMould", "GearMould", Matrix.CreateTranslation(0f, -0.02f, 0f) * 1.6f, Matrix.CreateTranslation(2.6f, 1.4f, 0f), "A Gear Mould made of dirt and sand, the neccessary part in making steel gear.", "SteelGearMould"),
			new Mould("Models/Piston", "Piston", Matrix.CreateTranslation(0f, -0.02f, 0f) * 1.2f, Matrix.CreateTranslation(4f, 3.8f, 0f), "A piston made of iron, copper and steel, the neccessary part of many machine.", "IndustrialPiston", 1.6f),
			new Wire("CopperWire"),
			new Sheet(MetalType.Steel),
			new Sheet(MetalType.Iron),
			new Sheet(MetalType.Copper),
			new Sheet(MetalType.Gold),
			new Sheet(MetalType.Sliver),
			new Sheet(MetalType.Lead),
			new Sheet(MetalType.Zinc),
			new Sheet(MetalType.Stannary),
			new Sheet(MetalType.Platinum),
			new Sheet(MetalType.Aluminum),
			new Mould("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.5f, 0f, 0f), Matrix.CreateTranslation(0.8f, 0.8f, 0f) * Matrix.CreateScale(20f), "IndustrialMagnet", "IndustrialMagnet"),
            new FireBrick()
			new Mould("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.5f, 0.5f, 0f), Matrix.CreateTranslation(0.8f, 0.8f, 0f) * Matrix.CreateScale(20f), "IndustrialMagnet", "IndustrialMagnet"),
			new Mould("Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f), Matrix.CreateTranslation(0.5f, 0.8f, 0f) * Matrix.CreateScale(20f), "CuZnBattery", "CuZnBattery"),
			new Mould("Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f), Matrix.CreateTranslation(0.6f, 0.8f, 0f) * Matrix.CreateScale(20f), "AgZnBattery", "AgZnBattery"),
			new Mould("Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f), Matrix.CreateTranslation(0.7f, 0.8f, 0f) * Matrix.CreateScale(20f), "AuZnBattery", "AuZnBattery"),
			new Mould("Models/Battery", "Battery", Matrix.CreateTranslation(0f, -0.5f, 0f), Matrix.CreateTranslation(0.8f, 0.8f, 0f) * Matrix.CreateScale(20f), "VoltaicBattery", "VoltaicBattery"),
		};
		public static Dictionary<string, int> IdTable;
		static ItemBlock()
		{
            IdTable = new Dictionary<string, int>(Items.Length);
			for (int i = 0; i < Items.Length; i++)
			{
				IdTable.Add(Items[i].GetCraftingId(), Index | i << 14);
			}
		}
		/*public override void Initialize()
		{
			var streamReader = new StreamReader(CustomTextureBlock.GetTargetFile("IndustrialMod.icsv"));
			try
			{
				LoadBlocksData(streamReader.ReadToEnd());
			}
			catch (Exception e)
			{
				Log.Warning("\"IndustrialMod.icsv\": " + e);
			}
			finally
			{
				streamReader.Dispose();
			}
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
		}*/
	}
}
