using Engine;
using Engine.Graphics;
using GameEntitySystem;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public partial class Comparer : IComparer<Point2>, IComparer<IChemicalItem>
	{
		public int Compare(Point2 x, Point2 y)
		{
			return x.X != y.X ? x.X.CompareTo(y.X) : x.Y.CompareTo(y.Y);
		}

		public int Compare(IChemicalItem x, IChemicalItem y)
		{
			return x.GetDispersionSystem().GetHashCode() - y.GetDispersionSystem().GetHashCode();
		}
	}

	partial class Utils
	{
		public static TerrainGeometrySubsets GTV(int x, int z, TerrainGeometrySubsets geometry)
		{
			var chunk = Terrain.GetChunkAtCell(x, z);
			if (chunk == null) return geometry;
			bool flag = SubsystemItemBlockBehavior.Data.TryGetValue(chunk.Coords, out geometry);
			if (!flag)
			{
				var tgs = new TerrainGeometrySubset(new DynamicArray<TerrainVertex>(), new DynamicArray<ushort>());
				geometry = new TerrainGeometrySubsets
				{
					SubsetOpaque = tgs,
					SubsetAlphaTest = tgs,
					OpaqueSubsetsByFace = new[]
					{
						tgs, tgs, tgs,
						tgs, tgs, tgs
					},
					AlphaTestSubsetsByFace = new[]
					{
						tgs, tgs, tgs,
						tgs, tgs, tgs
					}
				};
				SubsystemItemBlockBehavior.Data.Add(chunk.Coords, geometry);
			}
			return geometry;
		}

		public static void GenerateChunkVertices(TerrainUpdater updater, TerrainChunk chunk, int x1, int z1, int x2, int z2)
		{
			if (chunk.ThreadState == TerrainChunkState.InvalidVertices1)
				SubsystemItemBlockBehavior.Data.Remove(chunk.Coords);
			Terrain m_terrain = updater.m_terrain;
			TerrainChunk chunkAtCoords = m_terrain.GetChunkAtCoords(chunk.Coords.X - 1, chunk.Coords.Y - 1);
			TerrainChunk chunkAtCoords2 = m_terrain.GetChunkAtCoords(chunk.Coords.X, chunk.Coords.Y - 1);
			TerrainChunk chunkAtCoords3 = m_terrain.GetChunkAtCoords(chunk.Coords.X + 1, chunk.Coords.Y - 1);
			TerrainChunk chunkAtCoords4 = m_terrain.GetChunkAtCoords(chunk.Coords.X - 1, chunk.Coords.Y);
			TerrainChunk chunkAtCoords5 = m_terrain.GetChunkAtCoords(chunk.Coords.X + 1, chunk.Coords.Y);
			TerrainChunk chunkAtCoords6 = m_terrain.GetChunkAtCoords(chunk.Coords.X - 1, chunk.Coords.Y + 1);
			TerrainChunk chunkAtCoords7 = m_terrain.GetChunkAtCoords(chunk.Coords.X, chunk.Coords.Y + 1);
			TerrainChunk chunkAtCoords8 = m_terrain.GetChunkAtCoords(chunk.Coords.X + 1, chunk.Coords.Y + 1);
			if (chunkAtCoords4 == null)
			{
				x1 = MathUtils.Max(x1, 1);
			}
			if (chunkAtCoords2 == null)
			{
				z1 = MathUtils.Max(z1, 1);
			}
			if (chunkAtCoords5 == null)
			{
				x2 = MathUtils.Min(x2, 15);
			}
			if (chunkAtCoords7 == null)
			{
				z2 = MathUtils.Min(z2, 15);
			}
			for (int i = x1; i < x2; i++)
			{
				for (int j = z1; j < z2; j++)
				{
					switch (i)
					{
						case 0:
							if ((j == 0 && chunkAtCoords == null) || (j == 15 && chunkAtCoords6 == null))
							{
								continue;
							}
							break;
						case 15:
							if ((j == 0 && chunkAtCoords3 == null) || (j == 15 && chunkAtCoords8 == null))
							{
								continue;
							}
							break;
					}
					int num = i + chunk.Origin.X;
					int num2 = j + chunk.Origin.Y;
					int bottomHeightFast = chunk.GetBottomHeightFast(i, j);
					int bottomHeight = m_terrain.GetBottomHeight(num - 1, num2);
					int bottomHeight2 = m_terrain.GetBottomHeight(num + 1, num2);
					int bottomHeight3 = m_terrain.GetBottomHeight(num, num2 - 1);
					int bottomHeight4 = m_terrain.GetBottomHeight(num, num2 + 1);
					int x3 = MathUtils.Min(bottomHeightFast - 1, MathUtils.Min(bottomHeight, bottomHeight2, bottomHeight3, bottomHeight4));
					int topHeightFast = chunk.GetTopHeightFast(i, j);
					int num3 = MathUtils.Max(x3, 1);
					topHeightFast = MathUtils.Min(topHeightFast, 126);
					int num4 = TerrainChunk.CalculateCellIndex(i, 0, j);
					for (int k = num3; k <= topHeightFast; k++)
					{
						int cellValueFast = chunk.GetCellValueFast(num4 + k);
						int num5 = Terrain.ExtractContents(cellValueFast);
						if (num5 != 0)
						{
							BlocksManager.Blocks[num5].GenerateTerrainVertices(updater.m_subsystemTerrain.BlockGeometryGenerator, chunk.Geometry, cellValueFast, num, k, num2);
							chunk.GeometryMinY = MathUtils.Min(chunk.GeometryMinY, k);
							chunk.GeometryMaxY = MathUtils.Max(chunk.GeometryMaxY, k);
						}
					}
				}
			}
		}
	}

	public class SubsystemItemBlockBehavior : SubsystemThrowableBlockBehavior, IDrawable
	{
		public static int[] m_drawOrders = { 0 };
		public int[] DrawOrders => m_drawOrders;

		public static SortedMultiCollection<Point2, TerrainGeometrySubsets> Data;

		public SubsystemItemBlockBehavior()
		{
			TerrainUpdater.GenerateChunkVertices1 = Utils.GenerateChunkVertices;
			Data = new SortedMultiCollection<Point2, TerrainGeometrySubsets>(16, new Comparer());
		}

		public void Draw(Camera camera, int drawOrder)
		{
			var arr = Data.m_array;
			int len = Data.Count;
			for (int i = 0; i < len; i++)
			{
				var c = arr[i].Value;
				if (c == null) continue;
				Display.BlendState = BlendState.Opaque;
				Display.DepthStencilState = DepthStencilState.Default;
				Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
				var m_shader = Utils.SubsystemMovingBlocks.m_shader;
				var m_subsystemSky = Utils.SubsystemMovingBlocks.m_subsystemSky;
				m_shader.GetParameter("u_texture").SetValue(ItemBlock.Texture);
				m_shader.GetParameter("u_samplerState").SetValue(SamplerState.PointClamp);
				m_shader.GetParameter("u_fogColor").SetValue(new Vector3(m_subsystemSky.ViewFogColor));
				m_shader.GetParameter("u_fogStartInvLength").SetValue(new Vector2(m_subsystemSky.ViewFogRange.X, 1f / (m_subsystemSky.ViewFogRange.Y - m_subsystemSky.ViewFogRange.X)));
				m_shader.GetParameter("u_worldViewProjectionMatrix").SetValue(camera.ViewProjectionMatrix);
				m_shader.GetParameter("u_viewPosition").SetValue(camera.ViewPosition);
				Display.DrawUserIndexed(PrimitiveType.TriangleList, m_shader, TerrainVertex.VertexDeclaration, c.SubsetOpaque.Vertices.Array, 0, c.SubsetOpaque.Vertices.Count, c.SubsetOpaque.Indices.Array, 0, c.SubsetOpaque.Indices.Count);
			}
		}

		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			Data.Remove(chunk.Coords);
		}

		public override int[] HandledBlocks => new int[] { 90, GunpowderBlock.Index, RottenMeatBlock.Index, ItemBlock.Index, TankBlock.Index };

		public override bool OnAim(Vector3 start, Vector3 direction, ComponentMiner componentMiner, AimState state)
		{
			int value = componentMiner.ActiveBlockValue;
			if (!(BlocksManager.Blocks[Terrain.ExtractContents(value)] is ItemBlock block))
				return false;
			var item = block.GetItem(ref value);
			return Terrain.ExtractContents(value) != GunpowderBlock.Index && (item is OreChunk || item is Mould && (value != ItemBlock.IdTable["Telescope"]) && (value != ItemBlock.IdTable["Screwdriver"]) || item is Brick || value == ItemBlock.IdTable["ScrapIron"]) && base.OnAim(start, direction, componentMiner, state);
		}

		public override bool OnUse(Vector3 start, Vector3 direction, ComponentMiner componentMiner)
		{
			int activeBlockValue = componentMiner.ActiveBlockValue;
			var result = componentMiner.PickTerrainForDigging(start, direction);
			Entity entity;
			Vector3 position;
			BodyRaycastResult? body;
			if (activeBlockValue == ItemBlock.IdTable["Wrench"])
			{
				body = componentMiner.PickBody(start, direction);
				Matrix matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
				position = matrix.Translation + 1f * matrix.Forward + Vector3.UnitY;
				if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance))
				{
					entity = body.Value.ComponentBody.Entity;
					if (entity.FindComponent<ComponentTrain>() != null || entity.FindComponent<ComponentBoatI>() != null)
					{
						for (var i = entity.FindComponents<IInventory>().GetEnumerator(); i.MoveNext();)
							i.Current.DropAllItems(position);
						Utils.SubsystemPickables.AddPickable(ItemBlock.IdTable[entity.ValuesDictionary.DatabaseObject.Name.Length == 8 ? "Minecart" : entity.ValuesDictionary.DatabaseObject.Name], 1, position, null, null);
						Project.RemoveEntity(entity, true);
						return true;
					}
				}
			}
			else if ((activeBlockValue == ItemBlock.IdTable["Train"] || activeBlockValue == ItemBlock.IdTable["Minecart"] || activeBlockValue == ItemBlock.IdTable["ETrain"]) && result.HasValue && Terrain.ExtractContents(result.Value.Value) == RailBlock.Index)
			{
				position = new Vector3(result.Value.CellFace.Point) + new Vector3(0.5f);
				entity = DatabaseManager.CreateEntity(Project, activeBlockValue == ItemBlock.IdTable["Minecart"] ? "Carriage" : activeBlockValue == ItemBlock.IdTable["Train"] ? "Train" : "ETrain", true);

				var rotation = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
				entity.FindComponent<ComponentTrain>(true).SetDirection(RailBlock.IsDirectionX(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value)))
					? rotation.Z < 0 ? 0 : 2
					: rotation.X < 0 ? 1 : 3);
				entity.FindComponent<ComponentBody>(true).Position = position;
				entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
				Project.AddEntity(entity);
				var componentTrain = entity.FindComponent<ComponentTrain>(true);
				if (activeBlockValue == ItemBlock.IdTable["Minecart"])
				{
					componentTrain.Update(0);
					var train = componentTrain.FindNearestTrain();
					if (train != null)
						componentTrain.ParentBody = train;
				}
				componentMiner.RemoveActiveTool(1);
				Utils.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
				return true;
			}
			else if (activeBlockValue == ItemBlock.IdTable["Telescope"])
			{
				//ScreensManager.SwitchScreen(new RecipaediaScreen2());
				var view = componentMiner.ComponentPlayer.View;
				view.ActiveCamera = view.ActiveCamera is TelescopeCamera ? view.FindCamera<FppCamera>(true) : (Camera)new TelescopeCamera(view);
				return true;
			}
			/*else if (activeBlockValue == ItemBlock.IdTable["Minecart"])
			{
				entity = DatabaseManager.CreateEntity(Project, "Carriage", true);
				body = componentMiner.PickBody(start, direction);
				if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance))
				{
					body = componentMiner.PickBody(start, direction);
					var componentTrain = entity.FindComponent<ComponentTrain>(true);
					var train = body.Value.ComponentBody.Entity.FindComponent<ComponentTrain>();
					if (train != null)
						componentTrain.ParentBody = train;
				}
				else if (result.HasValue)
					position = result.Value.RaycastStart + Vector3.Normalize(result.Value.RaycastEnd - result.Value.RaycastStart) * result.Value.Distance; ;
				var rotation = componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation.ToForwardVector();
				entity.FindComponent<ComponentTrain>(true).SetDirection(RailBlock.IsDirectionX(RailBlock.GetRailType(Terrain.ExtractData(result.Value.Value)))
					? rotation.Z < 0 ? 0 : 2
					: rotation.X < 0 ? 1 : 3);
				goto put;
			}*/
			if (activeBlockValue == ItemBlock.IdTable["基因查看器"] || Terrain.ExtractContents(activeBlockValue) == TankBlock.Index)
			{
				body = componentMiner.PickBody(start, direction);
				if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance))
				{
					var cv = body.Value.ComponentBody.Entity.FindComponent<ComponentVariant>();
					if (cv != null)
					{
						if (Terrain.ExtractContents(activeBlockValue) == TankBlock.Index)
						{
							int data = Terrain.ExtractData(activeBlockValue);
							if (data != 0)
							{
								float v = TankBlock.GetFactor(data);
								ref float val = ref cv.Genome.DominantGenes[(int)TankBlock.GetTrait(data)];
								if (v == 1f)
									val += 0.1f;
								else
									val *= v;
								cv.OnEntityAdded();
								return true;
							}
						}
						GameWidget gameWidget = componentMiner.ComponentPlayer.View.GameWidget;
						DialogsManager.ShowDialog(gameWidget, new MessageDialog("Result", cv.Genome.ToString(), "OK", null, null)
						{
							Size = gameWidget.ActualSize * 0.8f
						});
					}
					return true;
				}
			}
			else if (result.HasValue)
			{
				position = result.Value.RaycastStart + Vector3.Normalize(result.Value.RaycastEnd - result.Value.RaycastStart) * result.Value.Distance;
				if (activeBlockValue == ItemBlock.IdTable["SteamBoat"])
				{
					entity = DatabaseManager.CreateEntity(Project, "SteamBoat", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0f);
					goto put;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Icebreaker"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Icebreaker", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0f);
					goto put;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Airship"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Airship", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					goto put;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Airship"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Airship", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					goto put;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Plane"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Plane", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					goto put;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Tank"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Tank", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					//entity.FindComponent<ComponentCar>(true).Load;
					goto put;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Car"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Car", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					goto put;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Tractor"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Tractor", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					goto put;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Digger"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Digger", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					goto put;
				}
				else if (activeBlockValue == ItemBlock.IdTable["Pavior"])
				{
					entity = DatabaseManager.CreateEntity(Project, "Pavior", true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					goto put;
				}
				else if (BlocksManager.Blocks[Terrain.ExtractContents(activeBlockValue)] is ItemBlock item && item.GetItem(ref activeBlockValue) is Mine mine)
				{
					entity = DatabaseManager.CreateEntity(Project, "Mine", new ValuesDictionary
					{
						{ "Mine", new ValuesDictionary { { "Type", mine.MineType } } }
					}, true);
					entity.FindComponent<ComponentFrame>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, m_random.UniformFloat(0f, 6.283185f));
					var componentMine = entity.FindComponent<ComponentMine>(true);
					componentMine.ExplosionPressure = mine.ExplosionPressure;
					componentMine.Delay = mine.Delay;
					goto put;
				}
			}
			IInventory inventory = componentMiner.Inventory;
			TerrainRaycastResult? result2;
			if (Terrain.ExtractContents(activeBlockValue) == 90)
			{
				result2 = componentMiner.PickTerrainForGathering(start, direction);
				if (result2.HasValue)
				{
					CellFace cellFace = result2.Value.CellFace;
					int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z), 0);
					if (cellValue != (RottenMeatBlock.Index | 1 << 4 << 14))
						return false;
					inventory.RemoveSlotItems(inventory.ActiveSlotIndex, inventory.GetSlotCount(inventory.ActiveSlotIndex));
					if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						inventory.AddSlotItems(inventory.ActiveSlotIndex, RottenMeatBlock.Index | 2 << 4 << 14, 1);
					Utils.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, false, false);
					return true;
				}
			}
			if (activeBlockValue == (RottenMeatBlock.Index | 2 << 4 << 14))
			{
				result2 = componentMiner.PickTerrainForInteraction(start, direction);
				if (result2.HasValue && componentMiner.Place(result2.Value, RottenMeatBlock.Index | 1 << 4 << 14))
				{
					inventory.RemoveSlotItems(inventory.ActiveSlotIndex, 1);
					if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						inventory.AddSlotItems(inventory.ActiveSlotIndex, Terrain.ReplaceContents(activeBlockValue, 90), 1);
					return true;
				}
			}
			if (activeBlockValue == (RottenMeatBlock.Index | 6 << 4 << 14))
			{
				result2 = componentMiner.PickTerrainForInteraction(start, direction);
				if (result2.HasValue && componentMiner.Place(result2.Value, RottenMeatBlock.Index | 6 << 4 << 14))
				{
					inventory.RemoveSlotItems(inventory.ActiveSlotIndex, 1);
					if (inventory.GetSlotCount(inventory.ActiveSlotIndex) == 0)
						inventory.AddSlotItems(inventory.ActiveSlotIndex, Terrain.ReplaceContents(activeBlockValue, 90), 1);
					return true;
				}
			}
			return false;
		put:
			entity.FindComponent<ComponentBody>(true).Position = position;
			entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0f;
			Project.AddEntity(entity);
			componentMiner.RemoveActiveTool(1);
			Utils.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, position, 3f, true);
			return true;
		}

		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			if (cellFace.Face != 4)
				return;
			int x = cellFace.X,
				y = cellFace.Y,
				z = cellFace.Z;
			if (Terrain.ReplaceLight(Utils.Terrain.GetCellValueFast(x, y, z), 0) == ItemBlock.IdTable["Springboard"])
			{
				componentBody.ApplyImpulse(new Vector3 { Y = -velocity });
			}
		}
	}
}