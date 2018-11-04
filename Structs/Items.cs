﻿using Engine;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game
{
	public partial class ItemBlock : CubeBlock, IItemBlock
	{
		public static Dictionary<string, int> IdTable;

		public static Item[] Items = new Item[]
		{
			new RottenEgg(),
			new MetalLine(Materials.Iron),
			new MetalLine(Materials.Copper),
			new MetalLine(Materials.Steel),
			new MetalLine(Materials.Gold),
			new MetalLine(Materials.Silver),
			new MetalLine(Materials.Platinum),
			new MetalLine(Materials.Lead),
			new MetalLine(Materials.Stannary),
			new MetalLine(Materials.Chromium),
			new MetalLine(Materials.Aluminum),
			new MetalLine(Materials.FeAlCrAlloy),
			new Resistor(Materials.FeAlCrAlloy),
			new Rod(Color.Gray)
			{
				DefaultDisplayName = "RifleBarrel",
				DefaultDescription = "Rifle Barrel are made by Rifling Machine. They are useful for making guns."
			},
			new ScrapIron(),
			new OreChunk(Matrix.CreateRotationX(4f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(32 % 16 / 16f, 32 / 16 / 16f, 0f), Color.White, false, Materials.Gold),
			new OreChunk(Matrix.CreateRotationX(5f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(33 % 16 / 16f, 33 / 16 / 16f, 0f), Color.White, false, Materials.Silver),
			new OreChunk(Matrix.CreateRotationX(6f) * Matrix.CreateRotationZ(2f), Matrix.CreateTranslation(34 % 16 / 16f, 34 / 16 / 16f, 0f), Color.White, false, Materials.Platinum),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(3f), Matrix.CreateTranslation(35 % 16 / 16f, 35 / 16 / 16f, 0f), Color.LightGray, false, Materials.Lead),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(4f), Matrix.CreateTranslation(36 % 16 / 16f, 36 / 16 / 16f, 0f), new Color(65, 224, 205), false, Materials.Zinc),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(5f), Matrix.CreateTranslation(37 % 16 / 16f, 37 / 16 / 16f, 0f), new Color(225, 225, 225), false, Materials.Stannary),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(6f), Matrix.CreateTranslation(38 % 16 / 16f, 38 / 16 / 16f, 0f), Color.White, false, Materials.Mercury),
			new OreChunk(Matrix.CreateRotationX(1f) * Matrix.CreateRotationZ(7f), Matrix.CreateTranslation(39 % 16 / 16f, 39 / 16 / 16f, 0f), Color.White, false, Materials.Chromium),
			new OreChunk(Matrix.CreateRotationX(2f) * Matrix.CreateRotationZ(-1f), Matrix.CreateTranslation(40 % 16 / 16f, 40 / 16 / 16f, 0f), new Color(190, 190, 190), false, Materials.Titanium),
			new OreChunk(Matrix.CreateRotationX(3f) * Matrix.CreateRotationZ(-1f), Matrix.CreateTranslation(41 % 16 / 16f, 41 / 16 / 16f, 0f), Color.White, false, Materials.Nickel),
			new MetalIngot(Materials.Steel),
			new MetalIngot(Materials.Gold),
			new MetalIngot(Materials.Silver),
			new MetalIngot(Materials.Platinum),
			new MetalIngot(Materials.Lead),
			new MetalIngot(Materials.Zinc),
			new MetalIngot(Materials.Nickel),
			new MetalIngot(Materials.Chromium),
			new MetalIngot(Materials.Aluminum),
			new MetalIngot(Materials.Stannary),
			new MetalIngot(Materials.FeAlCrAlloy),
			new Powder(Materials.Iron),
			new Powder(Materials.Copper),
			new Powder(Materials.Germanium),
			new Powder(Materials.Gold),
			new Powder(Materials.Silver),
			new Powder(Materials.Platinum),
			new Powder(Materials.Lead),
			new Powder(Materials.Stannary),
			new Powder(Materials.Zinc),
			new Powder(Materials.Chromium),
			new Powder(Materials.Nickel),
			new Powder(Materials.Aluminum),
			new CoalPowder("Coal", new Color(28, 28, 28)),
			new CoalPowder("CokeCoal", Color.DarkGray, 2000f, 100f, "Coke Coal Powder looks like silver powder obtained by crushing coke coal. It can be used to be fuel or the reductant agent in the industrial field."),
			new Plate(Materials.Steel),
			new Plate(Materials.Iron),
			new Plate(Materials.Copper),
			new Plate(Materials.Gold),
			new Plate(Materials.Silver),
			new Plate(Materials.Lead),
			new Plate(Materials.Zinc),
			new Plate(Materials.Stannary),
			new Plate(Materials.Platinum),
			new Plate(Materials.Aluminum),
			new SteamBoat(),
			new Train(),
			new Rod(Materials.Steel, Color.LightGray),
			new Rod(Materials.Copper, new Color(255, 127, 80)),
			new Rod(Materials.Gold, new Color(255, 215, 0)),
			new Rod(Materials.Silver, new Color(253, 253, 253)),
			new Rod(Materials.Lead, new Color(88, 87, 86)),
			new Rod(Materials.Platinum, new Color(253, 253, 253)),
			new Rod(Materials.Zinc, new Color(232, 232, 232)),
			new Rod(Materials.Stannary, new Color(232, 232, 232)),
			new Rod(Materials.Chromium, new Color(90, 90, 90)),
			new Rod(Materials.Titanium, new Color(253, 253, 253)),
			new Rod(Materials.Nickel, new Color(253, 253, 253)),
			new Rod(Materials.Aluminum, new Color(232, 232, 232)),
			new Mould("Models/Gear", "Gear", Matrix.CreateTranslation(0f, 0f, 0f) * 2f * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(4f, 3.8f, 0f), "A gear made of steel, the neccessary part of all the machine during the initial industrial era.", "SteelGear"),
			new Mould("Models/Wheel", "Wheel", Matrix.CreateTranslation(0f, 0f, 0f) * 1.2f * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(4f, 3.8f, 0f), "A wheel made of steel, the neccessary part of the steam engine train.", "SteelWheel", 2f),
			new Mould("Models/WheelMould", "WheelMould", Matrix.CreateTranslation(0f, -0.02f, 0f) * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(2.6f, 1.4f, 0f), "A wheel mould made of dirt and sand, the neccessary part in making steel wheel.", "SteelWheelMould", 1.6f),
			new Mould("Models/GearMould", "GearMould", Matrix.CreateTranslation(0f, -0.02f, 0f) * 1.6f * Matrix.CreateTranslation(new Vector3(0.5f)), Matrix.CreateTranslation(2.6f, 1.4f, 0f), "A gear mould made of dirt and sand, the neccessary part in making steel gear.", "SteelGearMould"),
			new Mould("Models/Piston", "Piston", Matrix.CreateTranslation(0f, -0.02f, 0f) * 1.2f * Matrix.CreateTranslation(0.5f, 0.3f, 0.5f), Matrix.CreateTranslation(4f, 3.8f, 0f), "A piston made of iron, copper and steel, the neccessary part of many machine.", "IndustrialPiston", 1.6f),
			new Wire("CopperWire"),
			new Sheet(Materials.Steel),
			new Sheet(Materials.Iron),
			new Sheet(Materials.Copper),
			new Sheet(Materials.Gold),
			new Sheet(Materials.Silver),
			new Sheet(Materials.Lead),
			new Sheet(Materials.Zinc),
			new Sheet(Materials.Stannary),
			new Sheet(Materials.Platinum),
			new Sheet(Materials.Aluminum),
			new Mould("Models/Battery", "Battery", Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.5f, 0.5f, 0f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f) * Matrix.CreateScale(20f), "IndustrialMagnet", "IndustrialMagnet"),
			new RefractoryBrick(),
			new CokeCoal(),
			new Fan(Materials.Steel),
			new Fan(Materials.Aluminum),
			new Carriage(),
			new Airship(),
			new Sheet(Materials.Titanium),
			new CoalPowder("Sulphur", Color.Yellow, 1500f, 30f, "Sulphur is powder obtained by crushing sulphur chunk."),
			new Powder("Ashes", new Color(25, 25, 25)),
			new Powder("Slag", new Color(25, 25, 25)),
			new Powder("Sawdust", Color.LightYellow),
			new Powder("Brickbat", Color.DarkRed),
			new Powder("Broken Glass", Color.White),
			new Mould("Models/Cylinder", "obj1", Matrix.CreateScale(70f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "Cylinder", "Cylinder", 1.5f),
			new Mould("Models/Cylinder", "obj1", Matrix.CreateScale(70f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "LPG", "LPG", 1.5f),
			new Mould("Models/Cylinder", "obj1", Matrix.CreateScale(70f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "LNG", "LNG", 1.5f),
			new Mould("Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "He", "He", 1.5f),
			new Mould("Models/Cylinder", "obj1", Matrix.CreateScale(40f, 80f, 40f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), Matrix.CreateTranslation(9f / 16f, -7f / 16f, 0f), "Ar", "Ar", 1.5f),
			new Powder("Crude Salt", Color.White),
			new Powder("Refined Salt", Color.White),
			new Powder("Yeast", Color.White),
			new Powder("Coarse Silicon", Color.DarkGray),
			new Powder("Alum", Color.White),
			new Powder("Quartz Powder", Color.White),
			new Powder("Lichenin", Color.DarkGreen),
			new GranulatedItem("Talcum", Color.White),
			new Powder("Talcum Powder", Color.White),
			new Powder("Si", Color.DarkGray),
			new Sheet(Color.DarkGray)
			{
				DefaultDisplayName = "Polycrystalline Silicon",
				DefaultDescription = "Polycrystalline Silicon"
			},
			new Sheet(Color.DarkGray)
			{
				DefaultDisplayName = "Monocrystalline Silicon",
				DefaultDescription = "Monocrystalline Silicon"
			},
		};
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
