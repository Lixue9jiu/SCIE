﻿using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
    public class SubsystemFactorioTransportBeltBLockBehavior : SubsystemBlockBehavior, IDrawable
    {
        public SubsystemTime m_subsystemTime;
        public PrimitivesRenderer3D m_primitivesRenderer;
        public DrawBlockEnvironmentData m_drawBlockEnvironmentData = new DrawBlockEnvironmentData();
        public Dictionary<Point3, FTBandItems> m_blocks = new Dictionary<Point3, FTBandItems>();
        public Texture2D[] m_textures;
        public float m_visibilityRange;
        public double m_lastDrawtime = 0;
        public int[] m_drawedTime = new int[4] { 0, 0, 0, 0 };
        public int FTBGeneratedCount = 0;

        public static Vector3[] m_floorOffset = new Vector3[4]
        {
            new Vector3(-0.5f,0f,-0.5f),
            new Vector3(0.5f,0f,-0.5f),
            new Vector3(0.5f,0f,0.5f),
            new Vector3(-0.5f,0f,0.5f)
        };

        public static Vector3[,,] m_slopeOffset = new Vector3[4, 2, 4] {
            {
                {
                    new Vector3(-0.5f,-0.5f,-0.5f),
                    new Vector3(0.5f,-0.5f,-0.5f),
                    new Vector3(0.5f,0.5f,0.5f),
                    new Vector3(-0.5f,0.5f,0.5f)
                },
                {
                    new Vector3(-0.5f,0.5f,-0.5f),
                    new Vector3(0.5f,0.5f,-0.5f),
                    new Vector3(0.5f,-0.5f,0.5f),
                    new Vector3(-0.5f,-0.5f,0.5f)
                }
            },
            {
                {
                    new Vector3(-0.5f,-0.5f,-0.5f),
                    new Vector3(0.5f,0.5f,-0.5f),
                    new Vector3(0.5f,0.5f,0.5f),
                    new Vector3(-0.5f,-0.5f,0.5f)
                },
                {
                    new Vector3(-0.5f,0.5f,-0.5f),
                    new Vector3(0.5f,-0.5f,-0.5f),
                    new Vector3(0.5f,-0.5f,0.5f),
                    new Vector3(-0.5f,0.5f,0.5f)
                }
            },
            {
                {
                    new Vector3(-0.5f,0.5f,-0.5f),
                    new Vector3(0.5f,0.5f,-0.5f),
                    new Vector3(0.5f,-0.5f,0.5f),
                    new Vector3(-0.5f,-0.5f,0.5f)
                },
                {
                    new Vector3(-0.5f,-0.5f,-0.5f),
                    new Vector3(0.5f,-0.5f,-0.5f),
                    new Vector3(0.5f,0.5f,0.5f),
                    new Vector3(-0.5f,0.5f,0.5f)
                }
            },
            {
                {
                    new Vector3(-0.5f,0.5f,-0.5f),
                    new Vector3(0.5f,-0.5f,-0.5f),
                    new Vector3(0.5f,-0.5f,0.5f),
                    new Vector3(-0.5f,0.5f,0.5f)
                },
                {
                    new Vector3(-0.5f,-0.5f,-0.5f),
                    new Vector3(0.5f,0.5f,-0.5f),
                    new Vector3(0.5f,0.5f,0.5f),
                    new Vector3(-0.5f,-0.5f,0.5f)
                }
            }
        };

        public static Vector3[,,] m_cornerItemOffset = new Vector3[8, 2, 5]
        {//rotation, side, process
                {//0
                    {
                        new Vector3(0.5f,0.25f,0.15f),
                        new Vector3(0.2513f,0.25f,0.1005f),
                        new Vector3(0.0404f,0.25f,-0.0404f),
                        new Vector3(-0.1005f,0.25f,-0.2513f),
                        new Vector3(-0.15f,0.25f,-0.5f)
                    },
                    {
                        new Vector3(0.5f,0.25f,-0.15f),
                        new Vector3(0.3661f,0.25f,-0.1503f),
                        new Vector3(0.2525f,0.25f,-0.2525f),
                        new Vector3(0.1503f,0.25f,-0.3661f),
                        new Vector3(0.15f,0.25f,-0.5f)
                    }
                },
                {//1
                    {
                        new Vector3(-0.5f,0.25f,-0.15f),
                        new Vector3(-0.3661f,0.25f,-0.1503f),
                        new Vector3(-0.2525f,0.25f,-0.2525f),
                        new Vector3(-0.1503f,0.25f,-0.3661f),
                        new Vector3(-0.15f,0.25f,-0.5f)
                    },
                    {
                        new Vector3(-0.5f,0.25f,0.15f),
                        new Vector3(-0.2513f,0.25f,0.1005f),
                        new Vector3(-0.0404f,0.25f,-0.0404f),
                        new Vector3(0.1005f,0.25f,-0.2513f),
                        new Vector3(0.15f,0.25f,-0.5f)
                    }
                },
                {//2
                    {
                        new Vector3(0.15f,0.25f,-0.5f),
                        new Vector3(0.1503f,0.25f,-0.3661f),
                        new Vector3(0.2525f,0.25f,-0.2525f),
                        new Vector3(0.3661f,0.25f,-0.1503f),
                        new Vector3(0.5f,0.25f,-0.15f)
                    },
                    {
                        new Vector3(-0.15f,0.25f,-0.5f),
                        new Vector3(-0.1005f,0.25f,-0.2513f),
                        new Vector3(0.0404f,0.25f,-0.0404f),
                        new Vector3(0.2513f,0.25f,0.1005f),
                        new Vector3(0.5f,0.25f,0.15f)
                    }
                },
                {//3
                    {
                        new Vector3(0.15f,0.25f,-0.5f),
                        new Vector3(0.1005f,0.25f,-0.2513f),
                        new Vector3(-0.0404f,0.25f,-0.0404f),
                        new Vector3(-0.2513f,0.25f,0.1005f),
                        new Vector3(-0.5f,0.25f,0.15f)
                    },
                    {
                        new Vector3(-0.15f,0.25f,-0.5f),
                        new Vector3(-0.1503f,0.25f,-0.3661f),
                        new Vector3(-0.2525f,0.25f,-0.2525f),
                        new Vector3(-0.3661f,0.25f,-0.1503f),
                        new Vector3(-0.5f,0.25f,-0.15f)
                    }
                },
                {//4
                    {
                        new Vector3(-0.5f,0.25f,-0.15f),
                        new Vector3(-0.2513f,0.25f,-0.1005f),
                        new Vector3(-0.0404f,0.25f,0.0404f),
                        new Vector3(0.1005f,0.25f,0.2513f),
                        new Vector3(0.15f,0.25f,0.5f)
                    },
                    {
                        new Vector3(-0.5f,0.25f,0.15f),
                        new Vector3(-0.3661f,0.25f,0.1503f),
                        new Vector3(-0.2525f,0.25f,0.2525f),
                        new Vector3(-0.1503f,0.25f,0.3661f),
                        new Vector3(-0.15f,0.25f,0.5f)
                    }
                },
                {//5
                    {
                        new Vector3(0.5f,0.25f,0.15f),
                        new Vector3(0.3661f,0.25f,0.1503f),
                        new Vector3(0.2525f,0.25f,0.2525f),
                        new Vector3(0.1503f,0.25f,0.3661f),
                        new Vector3(0.15f,0.25f,0.5f)
                    },
                    {
                        new Vector3(0.5f,0.25f,-0.15f),
                        new Vector3(0.2513f,0.25f,-0.1005f),
                        new Vector3(0.0404f,0.25f,0.0404f),
                        new Vector3(-0.1005f,0.25f,0.2513f),
                        new Vector3(-0.15f,0.25f,0.5f)
                    }
                },
                {//6
                    {
                        new Vector3(-0.15f,0.25f,0.5f),
                        new Vector3(-0.1005f,0.25f,0.2513f),
                        new Vector3(0.0404f,0.25f,0.0404f),
                        new Vector3(0.2513f,0.25f,-0.1005f),
                        new Vector3(0.5f,0.25f,-0.15f)
                    },
                    {
                        new Vector3(0.15f,0.25f,0.5f),
                        new Vector3(0.1503f,0.25f,0.3661f),
                        new Vector3(0.2525f,0.25f,0.2525f),
                        new Vector3(0.3661f,0.25f,0.1503f),
                        new Vector3(0.5f,0.25f,0.15f)
                    }
                },
                {//7dai
                    {
                        new Vector3(-0.15f,0.25f,0.5f),
                        new Vector3(-0.1503f,0.25f,0.3661f),
                        new Vector3(-0.2525f,0.25f,0.2525f),
                        new Vector3(-0.3661f,0.25f,0.1503f),
                        new Vector3(-0.5f,0.25f,0.15f)
                    },
                    {
                        new Vector3(0.15f,0.25f,0.5f),
                        new Vector3(0.1005f,0.25f,0.2513f),
                        new Vector3(-0.0404f,0.25f,0.0404f),
                        new Vector3(-0.2513f,0.25f,-0.1005f),
                        new Vector3(-0.5f,0.25f,-0.15f)
                    }
                }
        };

        public static int[] m_colorDrawScale = new[] { 16, 8, 4 };

        public override int[] HandledBlocks
        {
            get { return new[] { 400 }; }
        }

        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            string value = valuesDictionary.GetValue<string>("FactorioTransportBelt");
            //this.m_blocks = new DynamicArray<Point3>(HumanReadableConverter.ValuesListFromString<Point3>(';', value));
            m_subsystemTime = Project.FindSubsystem<SubsystemTime>(true);
            m_textures = new Texture2D[3]
            {
                ContentManager.Get<Texture2D>("Textures/Factorio/Transport-belt_sprite"),
                ContentManager.Get<Texture2D>("Textures/Factorio/Fast-transport-belt_sprite"),
                ContentManager.Get<Texture2D>("Textures/Factorio/Express-transport-belt_sprite")
            };
            m_primitivesRenderer = Project.FindSubsystem<SubsystemModelsRenderer>(true).PrimitivesRenderer;
            m_visibilityRange = SettingsManager.VisibilityRange;
        }

        public override void Save(ValuesDictionary valuesDictionary)
        {
            base.Save(valuesDictionary);
            //string value = HumanReadableConverter.ValuesListToString<Point3>(';', this.m_blocks.ToArray<Point3>());
            //valuesDictionary.SetValue<string>("FactorioBeltTransport", value);
        }

        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            var point = new Point3(x, y, z);
            UpdateFTB(value, oldValue, point);
            if (!m_blocks.ContainsKey(point))
            {
                m_blocks.Add(point, new FTBandItems(new FactorioTransportBelt(point, value), ++FTBGeneratedCount));
            }
        }

        public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
        {
            var point = new Point3(x, y, z);
            UpdateFTB(value, value, point);
            if (!m_blocks.ContainsKey(point))
            {
                m_blocks.Add(point, new FTBandItems(new FactorioTransportBelt(point, value), ++FTBGeneratedCount));
            }
        }

        public static Point3[] m_surroundDirections = new Point3[4]
        {
            new Point3(1,0,0),
            new Point3(-1,0,0),
            new Point3(0,0,1),
            new Point3(0,0,-1)
        };

        public static Point3[] m_surroundDownDirections = new Point3[4]
        {
            new Point3(1,-1,0),
            new Point3(-1,-1,0),
            new Point3(0,-1,1),
            new Point3(0,-1,-1)
        };

        public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
        {
            var point = new Point3(x, y, z);
            UpdateFTB(newValue, value, point);
            if (m_blocks.ContainsKey(point))
            {
                m_blocks.Remove(point);
            }
        }

        public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
        {
            var point = new Point3(x, y, z);
            UpdateFTB(value, oldValue, point);
            if (m_blocks.ContainsKey(point))
            {
                m_blocks[point].FTB = new FactorioTransportBelt(point, value);
            }
            else
            {
                m_blocks.Add(point, new FTBandItems(new FactorioTransportBelt(point, value), ++FTBGeneratedCount));
            }
        }

        public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
        {
            /*int value = SubsystemTerrain.Terrain.GetCellValue(x, y, z);
            if (y - 1 == neighborY)
            {
                int cellContents = SubsystemTerrain.Terrain.GetCellContents(x, neighborY, z);
                if (BlocksManager.Blocks[cellContents].IsTransparent)
                    SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
            }*/
        }

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!worldItem.ToRemove && m_blocks.ContainsKey(cellFace.Point))
			{
				int i = worldItem is Pickable p ? p.Count : 1;
				if (i == 0) return;
				while (true)
				{
					int j = 0;
					for (; j < 5; j++)
					{
						var items = m_blocks[cellFace.Point].items;
						if (items[0, j].value == 0)
						{
							items[0, j] = new Item(worldItem.Value);
							break;
						}
						else if(items[1, j].value == 0)
						{
							items[1, j] = new Item(worldItem.Value);
							break;
						}
					}
					if (j != 5)
						i--;
					else return;
					if (i == 0)
					{
						worldItem.ToRemove = true;
						return;
					}
				}
			}
		}

		public void Draw(Camera camera, int drawOrder)
        {
            try
            {
                if (drawOrder == 10)
                {
                    double nowTime = m_subsystemTime.GameTime;
                    if (nowTime - m_lastDrawtime >= 0.01)
                    {
                        m_drawedTime[3]++;
                        m_drawedTime[2]++;
                        if (m_drawedTime[3] == 2)
                        {
                            m_drawedTime[1]++;
                        }
                        else if (m_drawedTime[3] > 3)
                        {
                            m_drawedTime[0]++;
                            m_drawedTime[1]++;
                            m_drawedTime[3] = 0;
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            if (m_drawedTime[i] > 15)
                            {
                                m_drawedTime[i] = 0;
                            }
                        }
                        m_lastDrawtime = nowTime;
                    }
                    foreach (FTBandItems block in m_blocks.Values)
                    {
                        FactorioTransportBelt FTB = block.FTB;
                        var FTBdisplayPosition = new Vector3(FTB.position.X + 0.5f, FTB.position.Y + (FTB.slopeType.HasValue ? 0.5f : 0.01f), FTB.position.Z + 0.5f);
                        if (Vector3.Distance(FTBdisplayPosition, camera.ViewPosition) < m_visibilityRange + 10)
                        {
                            TexturedBatch3D FTBtexturedBatch3D = m_primitivesRenderer.TexturedBatch(m_textures[FTB.color], true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
                            var FTBdisplayVertices = new Vector3[4];
                            for (int i = 0; i < 4; i++)
                            {
                                FTBdisplayVertices[i] = Vector3.Transform(FTBdisplayPosition + (FTB.slopeType.HasValue ? m_slopeOffset[FTB.rotation, FTB.slopeType.Value ? 1 : 0, i] : m_floorOffset[i]), camera.ViewMatrix);
                            }
                            Vector4 FTBtextureVertices = FactorioTransportBeltBlock.m_texCoords[FTB.cornerType.HasValue ? FTB.cornerType.Value + 4 : FTB.rotation, m_drawedTime[FTB.color]];
                            FTBtexturedBatch3D.QueueQuad(FTBdisplayVertices[0], FTBdisplayVertices[1], FTBdisplayVertices[2], FTBdisplayVertices[3], new Vector2(FTBtextureVertices.X, FTBtextureVertices.Y), new Vector2(FTBtextureVertices.Z, FTBtextureVertices.Y), new Vector2(FTBtextureVertices.Z, FTBtextureVertices.W), new Vector2(FTBtextureVertices.X, FTBtextureVertices.W), Color.White);
                            //Log.Information(FTB.color + " " + m_drawedTime[FTB.color] + " " + m_drawedTime[3]);
                            for (int side = 0; side <= 1; side++)
                            {
                                for (int process = 4; process >= 0; process--)
                                {
                                    Item item = block.items[side, process];
                                    if (item.value != 0)
                                    {
										Point3 direction = FactorioTransportBeltBlock.RotationToDirection(FTB.rotation);
                                        var matrix = new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, FTBdisplayPosition.X, FTBdisplayPosition.Y, FTBdisplayPosition.Z, 1f);
                                        if (FTB.cornerType.HasValue && process < 4)
                                        {
                                            matrix.Translation += m_cornerItemOffset[FTB.cornerType.Value, side, process];
                                            if (!item.stop && !block.justAddedItem.Contains(side * 5 + process))
                                            {
                                                matrix.Translation += (m_cornerItemOffset[FTB.cornerType.Value, side, process + 1] - m_cornerItemOffset[FTB.cornerType.Value, side, process]) * (m_drawedTime[2] % m_colorDrawScale[FTB.color]) / m_colorDrawScale[FTB.color];
                                            }
                                        }
                                        else
                                        {
                                            if (FTB.slopeType.HasValue)
                                            {
                                                if (FTB.slopeType.Value)
                                                {
                                                    matrix.M42 += process * 0.25f - 0.25f;
                                                }
                                                else
                                                {
                                                    matrix.M42 += 0.5f - process * 0.25f;
                                                }
                                            }
                                            else
                                            {
                                                matrix.M42 += 0.25f;
                                            }
											matrix.Translation += (process * 0.25f - 0.5f + (item.stop || block.justAddedItem.Contains(side * 5 + process) ? 0 : (m_drawedTime[2] % m_colorDrawScale[FTB.color] * 0.25f / m_colorDrawScale[FTB.color]))) * new Vector3(direction) + (side == 1 ? 0.15f : (-0.15f)) * new Vector3(FactorioTransportBeltBlock.RotationToDirection(TurnRight(FTB.rotation)));
                                        }
										BlocksManager.Blocks[Terrain.ExtractContents(item.value)].DrawBlock(m_primitivesRenderer, item.value, Color.White, 0.18f, ref matrix, m_drawBlockEnvironmentData);
                                        if ((m_drawedTime[FTB.color] & 3) == 3 && m_drawedTime[3] == 3)
                                        {
                                            item.stop = false;
                                            if (process >= 3)
                                            {
                                                Point3 frontPosition = FTB.position + direction;
                                                if (FTB.slopeType.HasValue && FTB.slopeType.Value)
                                                {
                                                    frontPosition += Point3.UnitY;
                                                }
                                                else if (!m_blocks.ContainsKey(frontPosition))
                                                {
                                                    frontPosition -= Point3.UnitY;
                                                }
                                                if (m_blocks.ContainsKey(frontPosition))
                                                {
                                                    FTBandItems frontBlock = m_blocks[frontPosition];
                                                    if (frontBlock.FTB.cornerType.HasValue || FTB.rotation == frontBlock.FTB.rotation)
                                                    {
                                                        if (frontBlock.items[side, 0].value == 0)
                                                        {
                                                            frontBlock.items[side, 0] = new Item(item.value);
                                                            if (frontBlock.drawIndex > block.drawIndex) { frontBlock.justAddedItem.Add(side * 5); }
                                                            block.items[side, process] = new Item();
                                                        }
                                                        else
                                                        {
                                                            block.items[side, process].stop = true;
                                                        }
                                                    }
                                                    else if (frontBlock.FTB.rotation != TurnBack(FTB.rotation))
                                                    {
                                                        if (process == 3)
                                                        {
                                                            if (block.items[side, process + 1].value == 0)
                                                            {
                                                                block.items[side, process + 1] = new Item(item.value);
                                                                block.items[side, process] = new Item();
                                                            }
                                                            else
                                                            {
                                                                block.items[side, process].stop = true;
                                                            }
                                                        }
                                                        else if (frontBlock.FTB.rotation == TurnLeft(FTB.rotation))
                                                        {
                                                            int frontPorcess = side == 0 ? 3 : 2;
                                                            if (frontBlock.items[0, frontPorcess].value == 0)
                                                            {
                                                                frontBlock.items[0, frontPorcess] = new Item(item.value);
                                                                if (frontBlock.drawIndex > block.drawIndex) { frontBlock.justAddedItem.Add(frontPorcess); }
                                                                block.items[side, process] = new Item();
                                                            }
                                                            else
                                                            {
                                                                block.items[side, process].stop = true;
                                                            }
                                                        }
                                                        else if (frontBlock.FTB.rotation == TurnRight(FTB.rotation))
                                                        {
                                                            int frontPorcess = side == 0 ? 2 : 3;
                                                            if (frontBlock.items[1, frontPorcess].value == 0)
                                                            {
                                                                frontBlock.items[1, frontPorcess] = new Item(item.value);
                                                                if (frontBlock.drawIndex > block.drawIndex) { frontBlock.justAddedItem.Add(5 + frontPorcess); }
                                                                block.items[side, process] = new Item();
                                                            }
                                                            else
                                                            {
                                                                block.items[side, process].stop = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            block.items[side, process].stop = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        block.items[side, process].stop = true;
                                                    }
                                                }
                                                else if (process == 3 && block.items[side, process + 1].value == 0)
                                                {
                                                    if (!block.justAddedItem.Contains(side * 5 + process))
                                                    {
                                                        block.items[side, 4] = new Item(item.value);
                                                        block.items[side, process] = new Item();
                                                    }
                                                }
                                                else
                                                {
													var p = new Vector3(FTB.position) + new Vector3(0.5f);
													SubsystemTerrain.m_subsystemPickables.AddPickable(block.items[side, process].value, 0, p, null, null);
                                                    block.items[side, process].value = 0;
                                                    //block.items[side, process].stop = true;
                                                }
                                            }
                                            else if (block.items[side, process + 1].value == 0)
                                            {
                                                if (!block.justAddedItem.Contains(side * 5 + process))
                                                {
                                                    block.items[side, process + 1] = new Item(item.value);
                                                    block.items[side, process] = new Item();
                                                }
                                            }
                                            else
                                            {
												//Log.Information(block.justAddedItem.Contains(side * 5 + process));
												block.items[side, process].stop = true;
                                            }
                                        }
                                    }
                                }
                            }
                            /*if (block.justAddedItem.Count > 0)
                            {
                                string a = "";
                                foreach(int i in block.justAddedItem)
                                {
                                    a += i / 5 + "," + i % 5 + ";";
                                }
                                Log.Information(a);
                            }*/
                            block.justAddedItem.Clear();
                        }
                    }
                    m_primitivesRenderer.Flush(camera.ViewProjectionMatrix, false, int.MaxValue);
                }
            }
            catch (Exception e)
            {
                Log.Warning("0: " + e.ToString());
            }
        }

        public int[] DrawOrders
        {
            get { return new[] { 10 }; }
        }

        public void UpdateFTB(int newValue, int oldValue, Point3 position)
        {
            try
            {
                bool isUpdated = newValue != oldValue;
                var list_positionsShouldBeUpdated = new List<Point3>();
                var oldFTB = new FactorioTransportBelt(position, oldValue);
                var newFTB = new FactorioTransportBelt(position, newValue);
                if (newFTB.isFTB)
                {
                    FactorioTransportBelt updatedFTB = UpdateFTBAccordingToNeighbours(newFTB);
                    int updatedData = updatedFTB.Data;
                    if (updatedData != Terrain.ExtractData(newValue))
                    {
                        SubsystemTerrain.ChangeCell(position.X, position.Y, position.Z, Terrain.ReplaceData(newValue, updatedData), true);
                        isUpdated = true;
                        if (updatedFTB.slopeType.HasValue)
                        {
                            list_positionsShouldBeUpdated.Add(position + Point3.UnitY + (updatedFTB.slopeType.Value ? FactorioTransportBeltBlock.RotationToDirection(updatedFTB.rotation) : FactorioTransportBeltBlock.RotationToDirection(TurnBack(updatedFTB.rotation))));
                        }
                    }
                }
                if (isUpdated)
                {
                    if (newValue != oldValue && oldFTB.isFTB)
                    {
                        if (oldFTB.slopeType.HasValue)
                        {
                            Point3 tempPosition = position + Point3.UnitY + (oldFTB.slopeType.Value ? FactorioTransportBeltBlock.RotationToDirection(oldFTB.rotation) : FactorioTransportBeltBlock.RotationToDirection(TurnBack(oldFTB.rotation)));
                            if (!list_positionsShouldBeUpdated.Contains(tempPosition))
                            {
                                list_positionsShouldBeUpdated.Add(tempPosition);
                            }
                        }
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        list_positionsShouldBeUpdated.Add(position + m_surroundDirections[i]);
                        list_positionsShouldBeUpdated.Add(position + m_surroundDownDirections[i]);
                    }
                    foreach (Point3 point3 in list_positionsShouldBeUpdated)
                    {
                        int tempValue = SubsystemTerrain.Terrain.GetCellValue(point3.X, point3.Y, point3.Z);
                        UpdateFTB(tempValue, tempValue, point3);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }
        }

        public FactorioTransportBelt UpdateFTBAccordingToNeighbours(FactorioTransportBelt FTB)
        {
            if (FTB.isFTB)
            {
                int turnBack = TurnBack(FTB.rotation);
                var backFTB = FactorioTransportBelt.GetFromPosition(FTB.position + FactorioTransportBeltBlock.RotationToDirection(turnBack), SubsystemTerrain.Terrain);
                if (!backFTB.isFTB)
                {
                    backFTB = FactorioTransportBelt.GetFromPosition(backFTB.position - Point3.UnitY, SubsystemTerrain.Terrain);
                }
                bool flag = false;
                if (!backFTB.isFTB)
                {
                    flag = true;
                    FactorioTransportBelt.GetFromPosition(backFTB.position + new Point3(0, 2, 0), SubsystemTerrain.Terrain);
                }
                if (backFTB.isFTB && backFTB.rotation == FTB.rotation)
                {
                    var frontUpFTB = FactorioTransportBelt.GetFromPosition(FTB.position + FactorioTransportBeltBlock.RotationToDirection(FTB.rotation) + Point3.UnitY, SubsystemTerrain.Terrain);
                    if (frontUpFTB.isFTB && frontUpFTB.rotation != turnBack)
                    {
                        FTB.slopeType = flag ? null : (bool?)true;
                        FTB.cornerType = null;
                        return FTB;
                    }
                    else
                    {
                        FTB.slopeType = flag ? (bool?)false : null;
                        FTB.cornerType = null;
                        return FTB;
                    }
                }
                else
                {
                    int turnLeft = TurnLeft(FTB.rotation);
                    int turnRight = TurnRight(FTB.rotation);
                    var leftFTB = FactorioTransportBelt.GetFromPosition(FTB.position + FactorioTransportBeltBlock.RotationToDirection(turnLeft), SubsystemTerrain.Terrain);
                    if (!leftFTB.isFTB)
                    {
                        leftFTB = FactorioTransportBelt.GetFromPosition(leftFTB.position - Point3.UnitY, SubsystemTerrain.Terrain);
                    }
                    var rightFTB = FactorioTransportBelt.GetFromPosition(FTB.position + FactorioTransportBeltBlock.RotationToDirection(turnRight), SubsystemTerrain.Terrain);
                    if (!rightFTB.isFTB)
                    {
                        rightFTB = FactorioTransportBelt.GetFromPosition(rightFTB.position - Point3.UnitY, SubsystemTerrain.Terrain);
                    }
                    if (leftFTB.isFTB && leftFTB.rotation == turnRight)
                    {
                        if (rightFTB.isFTB && rightFTB.rotation == turnLeft)
                        {
                            FTB.slopeType = null;
                            FTB.cornerType = null;
                            return FTB;
                        }
                        else
                        {
                            FTB.slopeType = null;
                            FTB.cornerType = FactorioTransportBeltBlock.m_rotations2CornerType[FTB.rotation, 0];
                            return FTB;
                        }
                    }
                    else if (rightFTB.isFTB && rightFTB.rotation == turnLeft)
                    {
                        FTB.slopeType = null;
                        FTB.cornerType = FactorioTransportBeltBlock.m_rotations2CornerType[FTB.rotation, 1];
                        return FTB;
                    }
                }
            }
            FTB.slopeType = null;
            FTB.cornerType = null;
            return FTB;
        }

        public static int TurnLeft(int rotation)
        {
            switch (rotation)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 3;
                case 3: return 0;
                default: return (rotation + 1) & 3;
            }
        }

        public static int TurnRight(int rotation)
        {
            switch (rotation)
            {
                case 0: return 3;
                case 1: return 0;
                case 2: return 1;
                case 3: return 2;
                default: return (rotation + 3) & 3;
            }
        }

        public static int TurnBack(int rotation)
        {
            switch (rotation)
            {
                case 0: return 2;
                case 1: return 3;
                case 2: return 0;
                case 3: return 1;
                default: return (rotation + 2) & 3;
            }
        }

        public class FactorioTransportBelt
        {
            public bool isFTB = true;
            public Point3 position;
            public int color;
            public int rotation;
            public int? cornerType;
            public bool? slopeType;

            public FactorioTransportBelt(Point3 position, int value)
            {
                this.position = position;
                if (Terrain.ExtractContents(value) == FactorioTransportBeltBlock.Index)
                {
                    int data = Terrain.ExtractData(value);
                    color = FactorioTransportBeltBlock.GetColor(data);
                    rotation = FactorioTransportBeltBlock.GetRotation(data);
                    cornerType = FactorioTransportBeltBlock.GetCornerType(data);
                    slopeType = FactorioTransportBeltBlock.GetSlopeType(data);
                }
                else
                {
                    isFTB = false;
                }
            }

            public static FactorioTransportBelt GetFromPosition(Point3 position, Terrain terrain)
            {
                return new FactorioTransportBelt(position, terrain.GetCellValue(position.X, position.Y, position.Z));
            }

            public int Data
            {
                get { return FactorioTransportBeltBlock.SetCornerType(FactorioTransportBeltBlock.SetSlopeType(FactorioTransportBeltBlock.SetRotation(FactorioTransportBeltBlock.SetColor(0, color), rotation), slopeType), cornerType); }
            }

           /* public int Value
            {
                get { return Terrain.MakeBlockValue(FactorioTransportBeltBlock.Index, 0, Data); }
            }

            public FactorioTransportBelt(Point3 position, int color, int rotation, int? cornerType, bool? isSlope)
            {
                this.position = position;
                this.color = color;
                this.rotation = rotation;
                this.cornerType = cornerType;
                slopeType = isSlope;
            }*/
        }

        public struct Item
        {
            public int value;
            public bool stop;

            public Item(int val)
            {
                value = val;
                stop = false;
            }
        }

        public class FTBandItems
        {
            public FactorioTransportBelt FTB;
            public Item[,] items = new Item[2, 5]; //side, process
            public List<int> justAddedItem = new List<int>();
            public int drawIndex = 0;

            public FTBandItems(FactorioTransportBelt ftb, int DrawIndex)
            {
                FTB = ftb;
                drawIndex = DrawIndex;
                //items[0, 0] = new Item(9);
                //items[1, 1] = new Item(10);
            }

            public FTBandItems(FactorioTransportBelt ftb, Item[,] itemss)
            {
                FTB = ftb;
                items = itemss;
            }
        }
    }
}