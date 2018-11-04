﻿using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class Battery : DeviceBlock, IBlockBehavior, IHarvestingItem, IEquatable<Battery>
	{
		public int Factor = 1;
		public readonly BoundingBox[] m_collisionBoxes;
		public readonly BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public int RemainCount = 500;
		public Battery(int voltage, string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, string description = "", string name = "") : base(voltage, ElementType.Connector | ElementType.Container)
		{
			DefaultDisplayName = name;
			DefaultDescription = description;
			var blockMesh = Utils.CreateMesh(modelName, meshName, boneTransform, tcTransform, Color.LightGray);
			m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
			m_collisionBoxes = new BoundingBox[] { blockMesh.CalculateBoundingBox() };
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), null, geometry.SubsetOpaque);
			WireDevice.GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * SubsystemPalette.GetColor(environmentData, PaintableItemBlock.GetColor(Terrain.ExtractData(value))), size, ref matrix, environmentData);
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.ReplaceLight(oldValue, 0),
				Count = 1
			});
		}
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return m_collisionBoxes;
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage == 0 && RemainCount > 0)
			{
				voltage = Voltage;
				RemainCount--;
			}
			else if (voltage >= Voltage)
			{
				voltage -= Voltage;
				RemainCount += Factor;
			}
		}
		public bool Equals(Battery other)
		{
			return base.Equals(other) && Factor == other.Factor;
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return true;
		}
		public void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			if (RemainCount <= 0)
			{
				dropValue.Value = Terrain.ReplaceData(Terrain.ReplaceLight(blockValue, 0), Terrain.ExtractData(blockValue) | 16384);
			}
		}
		public void OnBlockAdded(SubsystemTerrain terrain, int value, int oldValue)
		{
			if ((Terrain.ExtractData(value) & 16384) != 0)
			{
				RemainCount = 0;
			}
		}
		public void OnBlockRemoved(SubsystemTerrain terrain, int value, int newValue)
		{
		}
	}
	public class Fridge : InteractiveEntityDevice<ComponentNewChest>
	{
		public Fridge() : base("Freezer", 2000)
		{
		}
		public override void Simulate(ref int voltage)
		{
			Component.Powered = voltage >= 110;
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (Terrain.ExtractData(value) >> 15)
			{
				case 0: return face == 0 ? 106 : 107;
				case 1: return face == 1 ? 106 : 107;
				case 2: return face == 2 ? 106 : 107;
			}
						return face == 3 ? 106 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(0, componentMiner, value, raycastResult);
		}
		public override string GetDescription(int value)
		{
			return "A freezer is a good place to protect food that can delay the decay of it. It will hold up to 16 stacks of items.";
		}
		public override Widget GetWidget(IInventory inventory, ComponentNewChest component)
		{
			return new NewChestWidget(inventory, component);
		}
	}

	public class Magnetizer : InteractiveEntityDevice<ComponentMagnetizer>
	{
		public Magnetizer() : base("Magnetizer", 1000)
		{
		}
		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 12)
			{
				voltage -= 12;
			}
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (Terrain.ExtractData(value) >> 15)
			{
				case 0: return face == 0 ? 239 : 107;
				case 1: return face == 1 ? 239 : 107;
				case 2: return face == 2 ? 239 : 107;
			}
						return face == 3 ? 239 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(2, componentMiner, value, raycastResult);
		}
		public override string GetDescription(int value)
		{
			return "A magnetizer is a device to create industrial magnet by melting steel ingot in a stronge magnetic field provided by wire.";
		}
		public override Widget GetWidget(IInventory inventory, ComponentMagnetizer component)
		{
			return new StoveWidget(inventory, component, "Widgets/MagnetizerWidget");
		}
	}

	public class Separator : InteractiveEntityDevice<ComponentSeperator>
	{
		public Separator() : base("Seperator", 2000)
		{
		}
		/*public override Device Create(Point3 p)
		{
			var other = (Separator)base.Create(p);
			return other;
		}*/
		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 120)
			{
				voltage -= 120;
			}
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (Terrain.ExtractData(value) >> 15)
			{
				case 0: return face == 0 ? 240 : 107;
				case 1: return face == 1 ? 240 : 107;
				case 2: return face == 2 ? 240 : 107;
			}
						return face == 3 ? 240 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(3, componentMiner, value, raycastResult);
		}
		public override string GetDescription(int value)
		{
			return "A separator is a device to separate matarial by high frequency rotation, it is a shrinking version of centrifuge.";
		}
		public override Widget GetWidget(IInventory inventory, ComponentSeperator component)
		{
			return new SeperatorWidget(inventory, component);
		}
	}

	public class AirBlower : FixedDevice, IBlockBehavior
	{
		public int Level = -1;
		public AirBlower() : base(3000)
		{
		}
		public override void Simulate(ref int voltage)
		{
			int level;
			if (voltage < 20)
			{
				level = 0;
			}
			else
			{
				level = voltage < 20 ? 0 : (voltage - 20) / 40;
				voltage -= level * 40 + 20;
			}
			if (level == Level)
				return;
			var p = Point;

			//SubsystemTerrain.ChangeCell(p.X, p.Y, p.Z, , true);
			var chunk = Utils.Terrain.GetChunkAtCell(p.X, p.Z);
			if (chunk != null)
			{
				if (chunk.State < TerrainChunkState.InvalidLight)
				{
					Level = -1;
				}
				else
				{
					p.X &= 15;
					p.Z &= 15;
					int i;
					for (i = 1; i <= level && chunk.GetCellContentsFast(p.X, p.Y + i, p.Z) == 0; i++)
					{
						chunk.SetCellValueFast(p.X, p.Y + i, p.Z, RottenMeatBlock.Index | 3 << 4 << 14);
					}
					for (; i < 8 && p.Y < 127; i++)
					{
						if (Terrain.ReplaceLight(chunk.GetCellValueFast(p.X, ++p.Y, p.Z), 0) == (RottenMeatBlock.Index | 3 << 4 << 14))
							chunk.SetCellValueFast(p.X, p.Y, p.Z, 0);
					}
					Level = level;
				}
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			return face == 4 || face == 5 ? 107 : 220;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(4, componentMiner, value, raycastResult);
		}
		public override string GetDescription(int value)
		{
			return "AirBlower is a device to transfer air into some big machine that need a large amount of hot air.";
		}
		public void OnBlockAdded(SubsystemTerrain terrain, int value, int oldValue)
		{
		}
		public void OnBlockRemoved(SubsystemTerrain terrain, int value, int newValue)
		{
			Level = -1;
			value = 0;
			Simulate(ref value);
		}
	}

	public class EFurnace : InteractiveEntityDevice<ComponentElectricFurnace>, IElectricElementBlock
	{
		public EFurnace() : base("ElectricFurnace", 6000)
		{
		}
		/*public override Device Create(Point3 p)
		{
			var other = (EFurnace)base.Create(p);
			return other;
		}*/
		public override void Simulate(ref int voltage)
		{
			if (Component.Powered = voltage >= 300)
			{
				voltage -= 300;
			}
		}
		public override void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			base.OnBlockAdded(subsystemTerrain, value, oldValue);
			Component.Powered = false;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (Terrain.ExtractData(value) >> 15)
			{
				case 0: return face == 0 ? 164 : 107;
				case 1: return face == 1 ? 164 : 107;
				case 2: return face == 2 ? 164 : 107;
			}
						return face == 3 ? 164 : 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return GetPlacementValue(6, componentMiner, value, raycastResult);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return "Electric Resistance Furnace";
		}
		public override string GetDescription(int value)
		{
			return "Electric Resistance Furnace is a device that can heat item by heating resistor, it can reach a high temperature but a large amount heat is needed";
		}
		public override Widget GetWidget(IInventory inventory, ComponentElectricFurnace component)
		{
			return new ElectricFurnaceWidget(inventory, component);
		}
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new CraftingMachineElectricElement(subsystemElectricity, new Point3(x, y, z));
		}

		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			return face == 4 || face == 5 ? (ElectricConnectorType?)ElectricConnectorType.Input : null;
		}

		public int GetConnectionMask(int value)
		{
			int? color = PaintableItemBlock.GetColor(Terrain.ExtractData(value));
			return color.HasValue ? 1 << color.Value : 2147483647;
		}
	}
}