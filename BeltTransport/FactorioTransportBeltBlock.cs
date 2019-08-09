using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
    public class FactorioTransportBeltBlock : FlatBlock
    {
        public const int Index = 400;
        public Texture2D[] m_textures;

		public static int[,] m_cornerType2Rotations = new int[8, 3] {
            //{OriginalRotation, TailRotation, TailBackRotation}
            {0,1,3 },
            {0,3,1 },
            {3,2,0 },
            {1,2,0 },
            {2,3,1 },
            {2,1,3 },
            {3,0,2 },
            {1,0,2 }
        };

        public static int?[,] m_rotations2CornerType = new int?[4, 2] {
            //{OriginalRotation, IsTailOnRight(true 1; false 0)}
            {1,0},
            {7,3},
            {5,4},
            {2,6}
        };

        public static string[] m_displayNames = new string[3]
        {
            "Factorio ",
            "Factorio Fast ",
            "Factorio Express "
        };

        public override void Initialize()
        {
            m_textures = new Texture2D[3]
            {
                ContentManager.Get<Texture2D>("Textures/Factorio/Transport-belt_sprite"),
                ContentManager.Get<Texture2D>("Textures/Factorio/Fast-transport-belt_sprite"),
                ContentManager.Get<Texture2D>("Textures/Factorio/Express-transport-belt_sprite")
            };
            for (int y = 0; y < m_texRowCount; y++)
            {
                for (int x = 0; x < m_texColCount; x++)
                {
                    m_texCoords[y, x] = XYToTextureCoords(x, y);
                }
			}
            base.Initialize();
		}

        /*public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
            //generator.GenerateCubeVertices(this, value, x, y, z, 0.009f, 0.009f, 0.009f, 0.009f, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, -1, geometry.OpaqueSubsetsByFace);
        }*/

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            DrawFactorioBeltTransportBlock(primitivesRenderer, value, size, ref matrix, m_textures[GetColor(Terrain.ExtractData(value))], environmentData);
        }

        public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
        {
            return new BlockDebrisParticleSystem(subsystemTerrain, position, 0f, DestructionDebrisScale, Color.White, GetFaceTextureSlot(4, value));
        }

        public override IEnumerable<int> GetCreativeValues()
		{
			return new[] { Terrain.ReplaceData(BlockIndex, SetColor(0, 0)), Terrain.ReplaceData(BlockIndex, SetColor(0, 1)), Terrain.ReplaceData(BlockIndex, SetColor(0, 2)) };
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
			return m_displayNames[GetColor(Terrain.ExtractData(value))] + base.GetDisplayName(subsystemTerrain, value);
        }

        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            showDebris = false;
            int color = GetColor(Terrain.ExtractData(oldValue));
            dropValues.Add(new BlockDropValue
            {
                Value = Terrain.MakeBlockValue(BlockIndex, 0, SetColor(0, color)),
                Count = 1
            });
        }

        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
            float num = Vector3.Dot(forward, Vector3.UnitZ);
            float num2 = Vector3.Dot(forward, Vector3.UnitX);
            float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
            float num4 = Vector3.Dot(forward, -Vector3.UnitX);
            int rotation = 0;
            if (num == MathUtils.Max(num, num2, num3, num4))
            {
                rotation = 2;
            }
            else if (num2 == MathUtils.Max(num, num2, num3, num4))
            {
                rotation = 3;
            }
            else if (num3 == MathUtils.Max(num, num2, num3, num4))
            {
                rotation = 0;
            }
            else if (num4 == MathUtils.Max(num, num2, num3, num4))
            {
                rotation = 1;
            }
            int data = Terrain.ExtractData(value);
            return new BlockPlacementData
            {
                Value = Terrain.MakeBlockValue(BlockIndex, 0, SetRotation(data, rotation)),
                CellFace = raycastResult.CellFace
            };
		}

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			//value = Terrain.ExtractData(value);
			return RailBlock.boundingBoxes[/*GetSlopeType(value).HasValue ? 9 - GetRotation(value) : */0];
		}

		//0 z-; 1 x-; 2 z+; 3 x+
		public static int GetRotation(int data)
        {
            return data & 3;
        }

        public static int SetRotation(int data, int rotation)
        {
            return (data & -4) | (rotation & 3);
        }

        public static Point3 RotationToDirection(int rotation)
        {
            return CellFace.FaceToPoint3((rotation + 2) % 4);
        }

        public static int GetColor(int data)
        {
            return data >> 3 & 3;
        }

        public static int SetColor(int data, int color)
        {
            return (data & -25) | ((color & 3) << 3);
        }

        public static bool? GetSlopeType(int data)
        {
			return (data & 4) != 0 ? new bool?((data & 512) != 0) : null;
		}

		public static int SetSlopeType(int data, bool? slopeType)
        {
			return slopeType != null ? (data & -513) | 4 | (slopeType.Value ? 1 : 0) << 9 : data & -5;
		}

		public static int? GetCornerType(int data)
        {
			return (data & 32) != 0 ? data >> 6 & 7 : (int?)null;
		}

		public static int SetCornerType(int data, int? cornertype)
        {
			return cornertype != null ? (data & -481) | 32 | (cornertype.Value & 7) << 6 : data & -481;
		}

		public static int m_texRowCount = 12;
        public static int m_texColCount = 16;
        public static Vector4[,] m_texCoords = new Vector4[m_texRowCount, m_texColCount];

        public static Vector4 XYToTextureCoords(int x, int y)
        {
            float x1 = x / (float)m_texColCount;
            float y1 = y / (float)m_texRowCount;
            float z = (x + 1f) / m_texColCount;
            float w = (y + 1f) / m_texRowCount;
            return new Vector4(x1, y1, z, w);
        }

        public static void DrawFactorioBeltTransportBlock(PrimitivesRenderer3D primitivesRenderer, int value, float size, ref Matrix matrix, Texture2D texture, DrawBlockEnvironmentData environmentData)
        {
            environmentData = environmentData ?? BlocksManager.m_defaultEnvironmentData;
            Vector3 translation = matrix.Translation;
            Vector3 vector;
            Vector3 v;
            if (environmentData.BillboardDirection != null)
            {
                vector = Vector3.Normalize(Vector3.Cross(environmentData.BillboardDirection.Value, Vector3.UnitY));
                v = -Vector3.Normalize(Vector3.Cross(environmentData.BillboardDirection.Value, vector));
            }
            else
            {
                vector = matrix.Right;
                v = matrix.Up;
            }
            Vector3 p = translation + 0.85f * size * (-vector - v);
            Vector3 vector2 = translation + 0.85f * size * (vector - v);
            Vector3 vector3 = translation + 0.85f * size * (-vector + v);
            Vector3 p2 = translation + 0.85f * size * (vector + v);
            if (environmentData.ViewProjectionMatrix != null)
            {
                Matrix value2 = environmentData.ViewProjectionMatrix.Value;
                Vector3.Transform(ref p, ref value2, out p);
                Vector3.Transform(ref vector2, ref value2, out vector2);
                Vector3.Transform(ref vector3, ref value2, out vector3);
                Vector3.Transform(ref p2, ref value2, out p2);
            }
            //int data = Terrain.ExtractData(value);
            Vector4 vector4;
            //int color = GetColor(Terrain.ExtractData(value));
            vector4 = m_texCoords[1, 0];
            TexturedBatch3D texturedBatch3D = primitivesRenderer.TexturedBatch(texture, true, 0, null, RasterizerState.CullCounterClockwiseScissor, null, SamplerState.PointClamp);
            texturedBatch3D.QueueQuad(p, vector3, p2, vector2, new Vector2(vector4.X, vector4.W), new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.Y), new Vector2(vector4.Z, vector4.W), Color.White);
            texturedBatch3D.QueueQuad(p, vector2, p2, vector3, new Vector2(vector4.X, vector4.W), new Vector2(vector4.Z, vector4.W), new Vector2(vector4.Z, vector4.Y), new Vector2(vector4.X, vector4.Y), Color.White);
        }
    }
}