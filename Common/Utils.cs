using Engine;
using Engine.Graphics;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TemplatesDatabase;
using static Game.TerrainBrush;

namespace Game
{
	public static partial class Utils
	{
		public static BlockGeometryGenerator BlockGeometryGenerator;
		public static Random Random = new Random();
		public static SubsystemGameInfo SubsystemGameInfo;
		public static SubsystemAudio SubsystemAudio;
		public static SubsystemBodies SubsystemBodies;
		public static SubsystemTerrain SubsystemTerrain;
		public static SubsystemTime SubsystemTime;
		public static SubsystemItemsScanner SubsystemItemsScanner;
		public static SubsystemMovingBlocks SubsystemMovingBlocks;
		public static SubsystemBlockEntities SubsystemBlockEntities;
		public static SubsystemGlow SubsystemGlow;
		public static SubsystemExplosions SubsystemExplosions;
		public static SubsystemCollapsingBlockBehavior SubsystemCollapsingBlockBehavior;
		public static SubsystemPlayers SubsystemPlayers;
		public static SubsystemPickables SubsystemPickables;
		public static SubsystemProjectiles SubsystemProjectiles;
		public static SubsystemWeather SubsystemWeather;
		public static SubsystemLaser SubsystemLaser;
		public static SubsystemParticles SubsystemParticles;
		public static SubsystemSourBlockBehavior SubsystemSour;
		public static Terrain Terrain;
		public static bool LoadedProject;

		public static void Load(Project Project)
		{
			if (LoadedProject)
				return;
			SubsystemGameInfo = Project.FindSubsystem<SubsystemGameInfo>(true);
			SubsystemAudio = Project.FindSubsystem<SubsystemAudio>(true);
			SubsystemBodies = Project.FindSubsystem<SubsystemBodies>(true);
			SubsystemTime = Project.FindSubsystem<SubsystemTime>(true);
			SubsystemItemsScanner = Project.FindSubsystem<SubsystemItemsScanner>(true);
			SubsystemMovingBlocks = Project.FindSubsystem<SubsystemMovingBlocks>(true);
			SubsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(true);
			SubsystemGlow = Project.FindSubsystem<SubsystemGlow>(true);
			SubsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(true);
			SubsystemCollapsingBlockBehavior = Project.FindSubsystem<SubsystemCollapsingBlockBehavior>(true);
			SubsystemPlayers = Project.FindSubsystem<SubsystemPlayers>(true);
			SubsystemPickables = Project.FindSubsystem<SubsystemPickables>(true);
			SubsystemProjectiles = Project.FindSubsystem<SubsystemProjectiles>(true);
			SubsystemWeather = Project.FindSubsystem<SubsystemWeather>(true);
			SubsystemLaser = Project.FindSubsystem<SubsystemLaser>(true);
			SubsystemParticles = Project.FindSubsystem<SubsystemParticles>(true);
			SubsystemSour = Project.FindSubsystem<SubsystemSourBlockBehavior>(true);
			Terrain = (SubsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true)).Terrain;
			BlockGeometryGenerator = new BlockGeometryGenerator(Terrain, SubsystemTerrain, Project.FindSubsystem<SubsystemElectricity>(true), SubsystemTerrain.SubsystemFurnitureBlockBehavior, Project.FindSubsystem<SubsystemMetersBlockBehavior>(true), SubsystemTerrain.SubsystemPalette);
			LoadedProject = true;
		}

		public static void RemoveElementsInChunk(TerrainChunk chunk, IEnumerable<Point3> elements, Action<Point3> action)
		{
			int originX = chunk.Origin.X, originY = chunk.Origin.Y;
			var list = new List<Point3>();
			for (var i = elements.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current;
				if (key.X >= originX && key.X < originX + 16 && key.Z >= originY && key.Z < originY + 16)
					list.Add(key);
			}
			for (originX = 0; originX < list.Count; originX++)
				action(list[originX]);
		}

		/*public static void Fade(TerrainChunk chunk, int? color = null)
		{
			if (!chunk.IsLoaded) return;
			for (int i = 16; i-- > 0;)
			{
				for (int j = 16; i-- > 0;)
				{
					int n = TerrainChunk.CalculateCellIndex(i, 0, j);
					for (int k = 128; k-- > 0;)
					{
						if (BlocksManager.Blocks[Terrain.ExtractContents(chunk.GetCellValueFast(n + k))] is IPaintableBlock block)
							chunk.SetCellValueFast(n, block.Paint(SubsystemTerrain, 0, color));
					}
				}
			}
		}*/

		public static void PaintSelective(this TerrainChunk chunk, Cell[] cells, int x, int y, int z, int src = BasaltBlock.Index)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			for (int i = 0; i < cells.Length; i++)
			{
				Cell cell = cells[i];
				int y2 = cell.Y + y;
				if (y2 >= 0 && y2 < 128)
				{
					int index = TerrainChunk.CalculateCellIndex(cell.X + x & 15, y2, cell.Z + z & 15);
					if (src == chunk.GetCellValueFast(index))
						chunk.SetCellValueFast(index, cell.Value);
				}
			}
		}

		public static void PaintFastSelective(this TerrainChunk chunk, Cell[] cells, int x, int y, int z, int onlyInBlock)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			int val = Terrain.ReplaceContents(onlyInBlock, BasaltBlock.Index);
			for (int i = 0; i < cells.Length; i++)
			{
				Cell cell = cells[i];
				int y2 = cell.Y + y;
				if (y2 >= 0 && y2 < 128)
				{
					int index = TerrainChunk.CalculateCellIndex(cell.X + x & 15, y2, cell.Z + z & 15);
					if (Terrain.ExtractContents(chunk.GetCellValueFast(index)) == Terrain.ExtractContents(onlyInBlock))
					{
						//SubsystemMineral.StoreItemData(cell.Value);
						chunk.SetCellValueFast(index, val);
					}
				}
			}
		}

		public static void PaintMaskSelective(this TerrainChunk chunk, Cell[] cells, int x, int y, int z, int mask = BasaltBlock.Index)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			for (int i = 0; i < cells.Length; i++)
			{
				Cell cell = cells[i];
				int y2 = cell.Y + y;
				if (y2 >= 0 && y2 < 128)
				{
					int index = TerrainChunk.CalculateCellIndex(cell.X + x & 15, y2, cell.Z + z & 15);
					y2 = chunk.GetCellValueFast(index);
					if (Terrain.ExtractContents(mask) == Terrain.ExtractContents(y2))
					{
						//SubsystemMineral.StoreItemData(cell.Value);
						chunk.SetCellValueFast(index, y2 | mask);
					}
				}
			}
		}

		public static int GetDirectionXZ(ComponentMiner componentMiner)
		{
			Vector3 forward = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			float max = MathUtils.Max(num, num2, num3, num4);
			if (num == max) return 2;
			if (num2 == max) return 3;
			if (num3 == max) return 0;
			if (num4 == max) return 1;
			return 0;
		}

		public static int GetDirectionXYZ(ComponentMiner componentMiner)
		{
			Vector3 forward = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
			float num = Vector3.Dot(forward, Vector3.UnitZ),
				  num2 = Vector3.Dot(forward, Vector3.UnitX),
				  num3 = Vector3.Dot(forward, -Vector3.UnitZ),
				  num4 = Vector3.Dot(forward, -Vector3.UnitX),
				  num5 = Vector3.Dot(forward, Vector3.UnitY),
				  num6 = Vector3.Dot(forward, -Vector3.UnitY),
				  num7 = MathUtils.Min(MathUtils.Min(num, num2, num3), MathUtils.Min(num4, num5, num6));
			if (num == num7) return 0;
			if (num2 == num7) return 1;
			if (num3 == num7) return 2;
			if (num4 == num7) return 3;
			if (num5 == num7) return 4;
			if (num6 == num7) return 5;
			return 0;
		}

		[MethodImpl((MethodImplOptions)0x100)]
		public static ComponentBlockEntity GetBlockEntity(Point3 p)
		{
			SubsystemBlockEntities.m_blockEntities.TryGetValue(p, out ComponentBlockEntity entity);
			return entity;
		}

		public static void AppendMesh(this BlockMesh blockMesh, string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, Color color)
		{
			var model = ContentManager.Get<Model>(modelName);
			blockMesh.AppendModelMeshPart(model.FindMesh(meshName).MeshParts[0], BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(meshName).ParentBone) * boneTransform, false, false, false, false, color);
			if (tcTransform != Matrix.Identity)
				blockMesh.TransformTextureCoordinates(tcTransform);
		}

		[MethodImpl((MethodImplOptions)0x100)]
		public static int GetColor(int value)
		{
			return Terrain.ExtractData(value) & 0xF;
		}

		[MethodImpl((MethodImplOptions)0x100)]
		public static int SetColor(int data, int color)
		{
			return (data & -16) | (color & 0xF);
		}

		public static void CreateBlockEntity(this Project Project, string name, Point3 p)
		{
			var vd = new ValuesDictionary();
			vd.PopulateFromDatabaseObject(Project.GameDatabase.Database.FindDatabaseObject(name, Project.GameDatabase.EntityTemplateType, true));
			vd.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", p);
			Project.AddEntity(Project.CreateEntity(vd));
		}

		public static int[] GetCreativeValues(int BlockIndex)
		{
			var arr = new int[16];
			for (int i = 0; i < 16; i++)
				arr[i] = BlockIndex | SetColor(0, i) << 14;
			return arr;
		}
	}
}