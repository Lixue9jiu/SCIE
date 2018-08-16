using Engine;
using Engine.Graphics;
namespace Game
{
	public abstract class Generator : Device
	{
		public int Voltage;
		public bool State;
		protected Generator(int voltage) : base(ElementType.Supply)
		{
			Voltage = voltage;
		}
		public override void Simulate(ref int voltage)
		{
			if (State)
			{
				voltage += Voltage;
			}
		}
	}
	public class SmallGenerator : Generator
	{
		public SmallGenerator() : base(12)
		{
		}
	}
	public class WireElement : Element
	{
		readonly BlockMesh m_standaloneBlockMesh = ((WireBlock)BlocksManager.Blocks[WireBlock.Index]).m_standaloneBlockMesh;
		public WireElement() : base(ElementType.Connector)
		{
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? paintColor = GetColor(Terrain.ExtractData(value));
			Color color2 = paintColor.HasValue ? (color * SubsystemPalette.GetColor(environmentData, paintColor)) : (1.25f * WireBlock.WireColor * color);
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color2, 2f * size, ref matrix, environmentData);
		}
		public override int GetWeight(int voltage)
		{
			return 1;
		}
		public static int? GetColor(int data)
		{
			return null;
		}
		public static int SetColor(int data, int? color)
		{
			return data;
		}
	}
	public class Wire : Device
	{
		public override int GetWeight(int voltage)
		{
			return 1;
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
		public static void GenerateWireVertices(BlockGeometryGenerator generator, int value, int x, int y, int z, int mountingFace, float centerBoxSize, Vector2 centerOffset, TerrainGeometrySubset subset)
		{
			var terrain = generator.Terrain;
			if (!(SubsystemEnergy.GetElement(terrain.GetCellValueFast(x, y, z)) is Device device)) return;
			Color color = WireBlock.WireColor;
			int num = Terrain.ExtractContents(value);
			if (num == ElementBlock.Index)
			{
				int? color2 = ElementBlock.GetColor(Terrain.ExtractData(value));
				if (color2.HasValue)
					color = SubsystemPalette.GetColor(generator, color2);
			}
			float num3 = LightingManager.LightIntensityByLightValue[Terrain.ExtractLight(value)];
			Vector3 v = new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f) - 0.5f * CellFace.FaceToVector3(mountingFace);
			Vector3 vector = CellFace.FaceToVector3(mountingFace);
			Vector2 v2 = new Vector2(0.9376f, 0.0001f);
			Vector2 v3 = new Vector2(0.03125f, 0.00550781237f);
			Point3 point = CellFace.FaceToPoint3(mountingFace);
			int cellContents = terrain.GetCellContents(x - point.X, y - point.Y, z - point.Z);
			bool flag = cellContents == 2 || cellContents == 7 || cellContents == 8 || cellContents == 6 || cellContents == 62 || cellContents == 72;
			Vector3 v4 = CellFace.FaceToVector3(SubsystemElectricity.GetConnectorFace(mountingFace, ElectricConnectorDirection.Top));
			Vector3 vector2 = CellFace.FaceToVector3(SubsystemElectricity.GetConnectorFace(mountingFace, ElectricConnectorDirection.Left)) * centerOffset.X + v4 * centerOffset.Y;
			int num4 = 0;
			var paths = new DynamicArray<ElectricConnectionPath>();
			//SubsystemEnergy.GetAllConnectedNeighbors(device, mountingFace, paths);
			foreach (ElectricConnectionPath tmpConnectionPath in paths)
			{
				if ((num4 & (1 << tmpConnectionPath.ConnectorFace)) == 0)
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(mountingFace, 0, tmpConnectionPath.ConnectorFace);
					if (centerOffset != Vector2.Zero || connectorDirection != ElectricConnectorDirection.In)
					{
						num4 |= 1 << tmpConnectionPath.ConnectorFace;
						Color color3 = color;
						if (num != ElementBlock.Index)
						{
							int cellValue = terrain.GetCellValue(x + tmpConnectionPath.NeighborOffsetX, y + tmpConnectionPath.NeighborOffsetY, z + tmpConnectionPath.NeighborOffsetZ);
							if (Terrain.ExtractContents(cellValue) == ElementBlock.Index)
							{
								int? color4 = ElementBlock.GetColor(Terrain.ExtractData(cellValue));
								if (color4.HasValue)
								{
									color3 = SubsystemPalette.GetColor(generator, color4);
								}
							}
						}
						Vector3 vector3 = (connectorDirection != ElectricConnectorDirection.In) ? CellFace.FaceToVector3(tmpConnectionPath.ConnectorFace) : (-Vector3.Normalize(vector2));
						Vector3 vector4 = Vector3.Cross(vector, vector3);
						float s = (centerBoxSize >= 0f) ? MathUtils.Max(0.03125f, centerBoxSize / 2f) : (centerBoxSize / 2f);
						float num5 = (connectorDirection == ElectricConnectorDirection.In) ? 0.03125f : 0.5f;
						float num6 = (connectorDirection == ElectricConnectorDirection.In) ? 0f : ((tmpConnectionPath.ConnectorFace == tmpConnectionPath.NeighborFace) ? (num5 + 0.03125f) : ((tmpConnectionPath.ConnectorFace != CellFace.OppositeFace(tmpConnectionPath.NeighborFace)) ? num5 : (num5 - 0.03125f)));
						Vector3 vector5 = v - vector4 * 0.03125f + vector3 * s + vector2;
						Vector3 vector6 = v - vector4 * 0.03125f + vector3 * num5;
						Vector3 vector7 = v + vector4 * 0.03125f + vector3 * num5;
						Vector3 vector8 = v + vector4 * 0.03125f + vector3 * s + vector2;
						Vector3 vector9 = v + vector * 0.03125f + vector3 * (centerBoxSize / 2f) + vector2;
						Vector3 vector10 = v + vector * 0.03125f + vector3 * num6;
						if (flag && centerBoxSize == 0f)
						{
							Vector3 v5 = 0.25f * BlockGeometryGenerator.GetRandomWireOffset(0.5f * (vector5 + vector8), vector);
							vector5 += v5;
							vector8 += v5;
							vector9 += v5;
						}
						Vector2 vector11 = v2 + v3 * new Vector2(MathUtils.Max(0.0625f, centerBoxSize), 0f);
						Vector2 vector12 = v2 + v3 * new Vector2(num5 * 2f, 0f);
						Vector2 vector13 = v2 + v3 * new Vector2(num5 * 2f, 1f);
						Vector2 vector14 = v2 + v3 * new Vector2(MathUtils.Max(0.0625f, centerBoxSize), 1f);
						Vector2 vector15 = v2 + v3 * new Vector2(centerBoxSize, 0.5f);
						Vector2 vector16 = v2 + v3 * new Vector2(num6 * 2f, 0.5f);
						int num7 = Terrain.ExtractLight(terrain.GetCellValue(x + tmpConnectionPath.NeighborOffsetX, y + tmpConnectionPath.NeighborOffsetY, z + tmpConnectionPath.NeighborOffsetZ));
						float num8 = LightingManager.LightIntensityByLightValue[num7];
						float num9 = 0.5f * (num3 + num8);
						float num10 = LightingManager.CalculateLighting(-vector4);
						float num11 = LightingManager.CalculateLighting(vector4);
						float num12 = LightingManager.CalculateLighting(vector);
						float num13 = num10 * num3;
						float num14 = num10 * num9;
						float num15 = num11 * num9;
						float num16 = num11 * num3;
						float num17 = num12 * num3;
						float num18 = num12 * num9;
						Color color5 = new Color((byte)((float)(int)color3.R * num13), (byte)((float)(int)color3.G * num13), (byte)((float)(int)color3.B * num13));
						Color color6 = new Color((byte)((float)(int)color3.R * num14), (byte)((float)(int)color3.G * num14), (byte)((float)(int)color3.B * num14));
						Color color7 = new Color((byte)((float)(int)color3.R * num15), (byte)((float)(int)color3.G * num15), (byte)((float)(int)color3.B * num15));
						Color color8 = new Color((byte)((float)(int)color3.R * num16), (byte)((float)(int)color3.G * num16), (byte)((float)(int)color3.B * num16));
						Color color9 = new Color((byte)((float)(int)color3.R * num17), (byte)((float)(int)color3.G * num17), (byte)((float)(int)color3.B * num17));
						Color color10 = new Color((byte)((float)(int)color3.R * num18), (byte)((float)(int)color3.G * num18), (byte)((float)(int)color3.B * num18));
						int count = subset.Vertices.Count;
						subset.Vertices.Count += 6;
						TerrainVertex[] array = subset.Vertices.Array;
						BlockGeometryGenerator.SetupVertex(vector5.X, vector5.Y, vector5.Z, color5, vector11.X, vector11.Y, ref array[count]);
						BlockGeometryGenerator.SetupVertex(vector6.X, vector6.Y, vector6.Z, color6, vector12.X, vector12.Y, ref array[count + 1]);
						BlockGeometryGenerator.SetupVertex(vector7.X, vector7.Y, vector7.Z, color7, vector13.X, vector13.Y, ref array[count + 2]);
						BlockGeometryGenerator.SetupVertex(vector8.X, vector8.Y, vector8.Z, color8, vector14.X, vector14.Y, ref array[count + 3]);
						BlockGeometryGenerator.SetupVertex(vector9.X, vector9.Y, vector9.Z, color9, vector15.X, vector15.Y, ref array[count + 4]);
						BlockGeometryGenerator.SetupVertex(vector10.X, vector10.Y, vector10.Z, color10, vector16.X, vector16.Y, ref array[count + 5]);
						int count2 = subset.Indices.Count;
						subset.Indices.Count += ((connectorDirection == ElectricConnectorDirection.In) ? 15 : 12);
						ushort[] array2 = subset.Indices.Array;
						array2[count2] = (ushort)count;
						array2[count2 + 1] = (ushort)(count + 5);
						array2[count2 + 2] = (ushort)(count + 1);
						array2[count2 + 3] = (ushort)(count + 5);
						array2[count2 + 4] = (ushort)count;
						array2[count2 + 5] = (ushort)(count + 4);
						array2[count2 + 6] = (ushort)(count + 4);
						array2[count2 + 7] = (ushort)(count + 2);
						array2[count2 + 8] = (ushort)(count + 5);
						array2[count2 + 9] = (ushort)(count + 2);
						array2[count2 + 10] = (ushort)(count + 4);
						array2[count2 + 11] = (ushort)(count + 3);
						if (connectorDirection == ElectricConnectorDirection.In)
						{
							array2[count2 + 12] = (ushort)(count + 2);
							array2[count2 + 13] = (ushort)(count + 1);
							array2[count2 + 14] = (ushort)(count + 5);
						}
					}
				}
			}
			if (centerBoxSize == 0f && (num4 != 0 || num == 133))
			{
				for (int i = 0; i < 6; i++)
				{
					if (i != mountingFace && i != CellFace.OppositeFace(mountingFace) && (num4 & (1 << i)) == 0)
					{
						Vector3 vector17 = CellFace.FaceToVector3(i);
						Vector3 v6 = Vector3.Cross(vector, vector17);
						Vector3 vector18 = v - v6 * 0.03125f + vector17 * 0.03125f;
						Vector3 vector19 = v + v6 * 0.03125f + vector17 * 0.03125f;
						Vector3 vector20 = v + vector * 0.03125f;
						if (flag)
						{
							Vector3 v7 = 0.25f * BlockGeometryGenerator.GetRandomWireOffset(0.5f * (vector18 + vector19), vector);
							vector18 += v7;
							vector19 += v7;
							vector20 += v7;
						}
						Vector2 vector21 = v2 + v3 * new Vector2(0.0625f, 0f);
						Vector2 vector22 = v2 + v3 * new Vector2(0.0625f, 1f);
						Vector2 vector23 = v2 + v3 * new Vector2(0f, 0.5f);
						float num19 = LightingManager.CalculateLighting(vector17) * num3;
						float num20 = LightingManager.CalculateLighting(vector) * num3;
						Color color11 = new Color((byte)((float)(int)color.R * num19), (byte)((float)(int)color.G * num19), (byte)((float)(int)color.B * num19));
						Color color12 = new Color((byte)((float)(int)color.R * num20), (byte)((float)(int)color.G * num20), (byte)((float)(int)color.B * num20));
						int count3 = subset.Vertices.Count;
						subset.Vertices.Count += 3;
						var array3 = subset.Vertices.Array;
						BlockGeometryGenerator.SetupVertex(vector18.X, vector18.Y, vector18.Z, color11, vector21.X, vector21.Y, ref array3[count3]);
						BlockGeometryGenerator.SetupVertex(vector19.X, vector19.Y, vector19.Z, color11, vector22.X, vector22.Y, ref array3[count3 + 1]);
						BlockGeometryGenerator.SetupVertex(vector20.X, vector20.Y, vector20.Z, color12, vector23.X, vector23.Y, ref array3[count3 + 2]);
						int count4 = subset.Indices.Count;
						subset.Indices.Count += 3;
						ushort[] array4 = subset.Indices.Array;
						array4[count4] = (ushort)count3;
						array4[count4 + 1] = (ushort)(count3 + 2);
						array4[count4 + 2] = (ushort)(count3 + 1);
					}
				}
			}
		}
	}
	public abstract class Battery : Element
	{
		public int Voltage;
		public int RemainCount;
		protected Battery(int voltage) : base(ElementType.Container)
		{
			Voltage = voltage;
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage == 0)
			{
				voltage = Voltage;
				RemainCount--;
			}
			else if(voltage >= Voltage)
				RemainCount++;
		}
	}
	public class Battery12V : Battery
	{
		public Battery12V() : base(12)
		{
		}
	}
	public class ElectricFurnace : Device
	{
	}
	public class Fridge : Device
	{
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face != 4 && face != 5)
			{
				switch (Terrain.ExtractData(value))
				{
					case 0:
						if (face == 0)
						{
							return 106;
						}
						return 107;
					case 1:
						if (face == 1)
						{
							return 106;
						}
						return 107;
					case 2:
						if (face == 2)
						{
							return 106;
						}
						return 107;
					default:
						if (face == 3)
						{
							return 106;
						}
						return 107;
				}
			}
			return 107;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				data = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				data = 1;
			}
			BlockPlacementData result = default(BlockPlacementData);
			result.Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, ElementBlock.Index), data);
			result.CellFace = raycastResult.CellFace;
			return result;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return "Freezer";
		}
		public override string GetDescription(int value)
		{
			return "A Freezer is a good place to protect food that can delay the decay of it. It will hold up to 16 stacks of items.";
		}
	}
	public abstract class Diode : Element
	{
		public int MaxVoltage;
		protected Diode() : base(ElementType.Connector)
		{
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage < 0)
			{
				if (voltage > -MaxVoltage)
					voltage = 0;
				else MaxVoltage = 0;
			}
		}
	}
	public class DiodeDevice : Diode
	{
	}
}
