using Engine;
using System.Collections.Generic;
using System.Text;
using TemplatesDatabase;
using System;

using Engine.Graphics;


namespace Game
{
	public class SubsystemSky2 : SubsystemSky, IDrawable, IUpdateable
	{
		public float CalculateLightIntensity2(float timeOfDay,Camera camera)
		{
			float light = 1;
			//Utils.SubsystemPlayers.Project.
			if (camera.ViewPosition.Y>1000f)
			{
				light = (1000f / camera.ViewPosition.Y)* (1000f / camera.ViewPosition.Y)* (1000f / camera.ViewPosition.Y);
			}
			if (camera.ViewPosition.Y > 2000f)
			{
				light = 0f;
			}
			if (timeOfDay <= 0.2f || timeOfDay > 0.8f)
			{
				return 0f;
			}
			if (timeOfDay > 0.2f && timeOfDay <= 0.3f)
			{
				return (timeOfDay - 0.2f) / (71f / (226f * (float)Math.PI))*light;
			}
			if (timeOfDay > 0.3f && timeOfDay <= 0.7f)
			{
				return 1f*light;
			}
			return (1f - (timeOfDay - 0.7f) / 0.100000024f)*light;
		}

		public new void DrawStars(Camera camera)
		{
			float globalPrecipitationIntensity = camera.ViewPosition.Y>1000f ? 0f : m_subsystemWeather.GlobalPrecipitationIntensity;
			float timeOfDay = m_subsystemTimeOfDay.TimeOfDay;
			if (m_starsVertexBuffer == null || m_starsIndexBuffer == null)
			{
				Utilities.Dispose(ref m_starsVertexBuffer);
				Utilities.Dispose(ref m_starsIndexBuffer);
				m_starsVertexBuffer = new VertexBuffer(m_starsVertexDeclaration, 600);
				m_starsIndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, 900);
				FillStarsBuffers();
			}
			Display.DepthStencilState = DepthStencilState.DepthRead;
			Display.RasterizerState = RasterizerState.CullNoneScissor;
			float num = MathUtils.Sqr((1f - CalculateLightIntensity2(timeOfDay,camera)) * (1f - globalPrecipitationIntensity));
			if (num > 0.01f)
			{
				Display.BlendState = BlendState.Additive;
				m_shaderTextured.Transforms.World.set_Item(0, Matrix.CreateRotationZ(-2f * timeOfDay * (float)Math.PI) * Matrix.CreateTranslation(camera.ViewPosition) * camera.ViewProjectionMatrix);
				m_shaderTextured.Color = new Vector4(1f, 1f, 1f, num);
				m_shaderTextured.Texture = ContentManager.Get<Texture2D>("Textures/Star");
				m_shaderTextured.SamplerState = SamplerState.LinearClamp;
				Display.DrawIndexed(PrimitiveType.TriangleList, m_shaderTextured, m_starsVertexBuffer, m_starsIndexBuffer, 0, m_starsIndexBuffer.IndicesCount);
			}
		}


		public Color CalculateSkyColor2(Vector3 direction, float timeOfDay, float precipitationIntensity, int temperature,Camera camera)
		{
			direction = Vector3.Normalize(direction);
			Vector2 vector = Vector2.Normalize(new Vector2(direction.X, direction.Z));
			float s = CalculateLightIntensity2(timeOfDay,camera);
			float f = (float)temperature / 15f;
			Vector3 v = new Vector3(0.65f, 0.68f, 0.7f) * s;
			Vector3 v2 = Vector3.Lerp(new Vector3(0.28f, 0.38f, 0.52f), new Vector3(0.15f, 0.3f, 0.56f), f);
			Vector3 v3 = Vector3.Lerp(new Vector3(0.7f, 0.79f, 0.88f), new Vector3(0.64f, 0.77f, 0.91f), f);
			Vector3 v4 = Vector3.Lerp(v2, v, precipitationIntensity) * s;
			Vector3 v5 = Vector3.Lerp(v3, v, precipitationIntensity) * s;
			Vector3 v6 = new Vector3(1f, 0.3f, -0.2f);
			Vector3 v7 = new Vector3(1f, 0.3f, -0.2f);
			if (m_lightningStrikePosition.HasValue)
			{
				v4 = Vector3.Max(new Vector3(m_lightningStrikeBrightness), v4);
			}
			float num = MathUtils.Lerp(CalculateDawnGlowIntensity(timeOfDay)*MathUtils.Clamp(1000f/camera.ViewPosition.Y,0f,1f), 0f, precipitationIntensity);
			float num2 = MathUtils.Lerp(CalculateDuskGlowIntensity(timeOfDay) * MathUtils.Clamp(1000f / camera.ViewPosition.Y, 0f, 1f), 0f, precipitationIntensity);
			float f2 = MathUtils.Saturate((direction.Y - 0.1f) / 0.4f);
			float s2 = num * MathUtils.Sqr(MathUtils.Saturate(0f - vector.X));
			float s3 = num2 * MathUtils.Sqr(MathUtils.Saturate(vector.X));
			return new Color(Vector3.Lerp(v5 + v6 * s2 + v7 * s3, v4, f2));
		}

		public new void DrawSkydome(Camera camera)
		{
			if (!m_skyDomes.TryGetValue(camera.View, out SkyDome value))
			{
				value = new SkyDome();
				m_skyDomes.Add(camera.View, value);
			}
			if (value.VertexBuffer == null || value.IndexBuffer == null)
			{
				Utilities.Dispose(ref value.VertexBuffer);
				Utilities.Dispose(ref value.IndexBuffer);
				value.VertexBuffer = new VertexBuffer(m_skyVertexDeclaration, value.Vertices.Length);
				value.IndexBuffer = new IndexBuffer(IndexFormat.SixteenBits, value.Indices.Length);
				FillSkyIndexBuffer(value);
				value.LastUpdateTimeOfDay = null;
			}
			int x = Terrain.ToCell(camera.ViewPosition.X);
			int z = Terrain.ToCell(camera.ViewPosition.Z);
			float globalPrecipitationIntensity = m_subsystemWeather.GlobalPrecipitationIntensity;
			float timeOfDay = m_subsystemTimeOfDay.TimeOfDay;
			int temperature = m_subsystemTerrain.Terrain.GetTemperature(x, z);
			if (!value.LastUpdateTimeOfDay.HasValue || MathUtils.Abs(timeOfDay - value.LastUpdateTimeOfDay.Value) > 0.001f || !value.LastUpdatePrecipitationIntensity.HasValue || MathUtils.Abs(globalPrecipitationIntensity - value.LastUpdatePrecipitationIntensity.Value) > 0.02f || ((globalPrecipitationIntensity == 0f || globalPrecipitationIntensity == 1f) && value.LastUpdatePrecipitationIntensity.Value != globalPrecipitationIntensity) || m_lightningStrikeBrightness != value.LastUpdateLightningStrikeBrightness || !value.LastUpdateTemperature.HasValue || temperature != value.LastUpdateTemperature)
			{
				value.LastUpdateTimeOfDay = timeOfDay;
				value.LastUpdatePrecipitationIntensity = globalPrecipitationIntensity;
				value.LastUpdateLightningStrikeBrightness = m_lightningStrikeBrightness;
				value.LastUpdateTemperature = temperature;
				FillSkyVertexBuffer2(value, timeOfDay, globalPrecipitationIntensity, temperature,camera);
			}
			Display.DepthStencilState = DepthStencilState.DepthRead;
			Display.RasterizerState = RasterizerState.CullNoneScissor;
			Display.BlendState = BlendState.Opaque;
			m_shaderFlat.Transforms.m_world[0] = Matrix.CreateTranslation(camera.ViewPosition) * camera.ViewProjectionMatrix;
			m_shaderFlat.Color = Vector4.One;
			Display.DrawIndexed(PrimitiveType.TriangleList, m_shaderFlat, value.VertexBuffer, value.IndexBuffer, 0, value.IndexBuffer.IndicesCount);
		}
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_EarthTexture = ContentManager.Get<Texture2D>("Textures/Earth");
		}
		public Texture2D m_EarthTexture;
		public void FillSkyVertexBuffer2(SkyDome skyDome, float timeOfDay, float precipitationIntensity, int temperature, Camera camera)
		{
			for (int i = 0; i < 8; i++)
			{
				float x = (float)Math.PI / 2f * MathUtils.Sqr((float)i / 7f);
				for (int j = 0; j < 10; j++)
				{
					int num = j + i * 10;
					float x2 = (float)Math.PI * 2f * (float)j / 10f;
					float num2 = 900f * MathUtils.Cos(x);
					skyDome.Vertices[num].Position.X = num2 * MathUtils.Sin(x2);
					skyDome.Vertices[num].Position.Z = num2 * MathUtils.Cos(x2);
					skyDome.Vertices[num].Position.Y = 900f * MathUtils.Sin(x) - ((i == 0) ? 225f : 0f);
					skyDome.Vertices[num].Color = CalculateSkyColor2(skyDome.Vertices[num].Position, timeOfDay, precipitationIntensity, temperature,camera);
				}
			}
			skyDome.VertexBuffer.SetData(skyDome.Vertices, 0, skyDome.Vertices.Length);
		}


		public void DrawEarth(Camera camera)
		{
			float globalPrecipitationIntensity = 0f;
			float timeOfDay = 12f;
			float f = MathUtils.Max(CalculateDawnGlowIntensity(timeOfDay), CalculateDuskGlowIntensity(timeOfDay));
			float num = 2f * timeOfDay * (float)Math.PI;
			float angle = num;
			float num2 = MathUtils.Lerp(90f, 160f, f);
			float num3 = MathUtils.Lerp(60f, 80f, f);
			Color c = Color.Lerp(new Color(255, 255, 255), new Color(255, 255, 160), f);
			Color white = new Color(255, 255, 255);
			white *= 1f - 0f;
			c *= MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			white *= MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			//TexturedBatch3D batch = m_primitivesRenderer3d.TexturedBatch(m_glowTexture, useAlphaTest: false, 0, DepthStencilState.DepthRead, null, BlendState.Additive);
			TexturedBatch3D batch2 = m_primitivesRenderer3d.TexturedBatch(m_EarthTexture, useAlphaTest: false, 1, DepthStencilState.DepthRead, null, BlendState.AlphaBlend);
			//batch2. = 2;
			float ang = Vector2.Angle(new Vector2(1f,1f),new Vector2(camera.ViewPosition.X, camera.ViewPosition.Z))/ camera.ViewPosition.Y;
			//TexturedBatch3D batch3 = m_primitivesRenderer3d.TexturedBatch(m_moonTextures[MoonPhase], useAlphaTest: false, 1, DepthStencilState.DepthRead, null, BlendState.AlphaBlend);
			//QueueCelestialBody(batch, camera.ViewPosition, color, 900f, 3.5f * num2, num);
			//QueueCelestialBody(batch, camera.ViewPosition, color2, 900f, 3.5f * num3, angle);
			//QueueCelestialBody(batch2, camera.ViewPosition, c, 900f, num2, num);
			if (camera.ViewPosition.Y <= 1000f && camera.ViewPosition.Y>200f)
			{
				QueueCelestialBody(batch2, camera.ViewPosition, white, camera.ViewPosition.Y/100f, num3*1000f/camera.ViewPosition.Y*4f, angle - ang);
			}
			//	QueueCelestialBody(batch2, camera.ViewPosition, white, camera.ViewPosition.Y / 400f, num3, angle - ang);
			if (camera.ViewPosition.Y > 1000f)
			QueueCelestialBody(batch2, camera.ViewPosition, white, camera.ViewPosition.Y/400f, num3, angle- ang);
		}

		public new void Draw(Camera camera, int drawOrder)
		{
			if (drawOrder == m_drawOrders[0])
			{
				ViewUnderWaterDepth = 0f;
				ViewUnderMagmaDepth = 0f;
				Vector3 viewPosition = camera.ViewPosition;
				int x = Terrain.ToCell(viewPosition.X);
				int y = Terrain.ToCell(viewPosition.Y);
				int z = Terrain.ToCell(viewPosition.Z);
				FluidBlock surfaceFluidBlock;
				float? surfaceHeight = m_subsystemFluidBlockBehavior.GetSurfaceHeight(x, y, z, out surfaceFluidBlock);
				if (surfaceHeight.HasValue)
				{
					if (surfaceFluidBlock is WaterBlock)
					{
						ViewUnderWaterDepth = surfaceHeight.Value + 0.1f - viewPosition.Y;
					}
					else if (surfaceFluidBlock is MagmaBlock)
					{
						ViewUnderMagmaDepth = surfaceHeight.Value + 1f - viewPosition.Y;
					}
				}
				if (ViewUnderWaterDepth > 0f)
				{
					int humidity = m_subsystemTerrain.Terrain.GetHumidity(x, z);
					int temperature = m_subsystemTerrain.Terrain.GetTemperature(x, z);
					Color c = BlockColorsMap.WaterColorsMap.Lookup(temperature, humidity);
					float num = MathUtils.Lerp(1f, 0.5f, (float)humidity / 15f);
					float num2 = MathUtils.Lerp(1f, 0.2f, MathUtils.Saturate(0.075f * (ViewUnderWaterDepth - 2f)));
					float num3 = MathUtils.Lerp(0.33f, 1f, SkyLightIntensity);
					m_viewFogRange.X = 0f;
					m_viewFogRange.Y = MathUtils.Lerp(4f, 10f, num * num2 * num3);
					m_viewFogColor = Color.MultiplyColorOnly(c, 0.66f * num2 * num3);
					m_viewIsSkyVisible = false;
				}
				else if (ViewUnderMagmaDepth > 0f)
				{
					m_viewFogRange.X = 0f;
					m_viewFogRange.Y = 0.1f;
					m_viewFogColor = new Color(255, 80, 0);
					m_viewIsSkyVisible = false;
				}
				else
				{
					int temperature2 = m_subsystemTerrain.Terrain.GetTemperature(Terrain.ToCell(viewPosition.X), Terrain.ToCell(viewPosition.Z));
					float num4 = MathUtils.Lerp(0.5f, 0f, m_subsystemWeather.GlobalPrecipitationIntensity);
					float num5 = MathUtils.Lerp(1f, 0.8f, m_subsystemWeather.GlobalPrecipitationIntensity);
					m_viewFogRange.X = VisibilityRange * num4;
					m_viewFogRange.Y = VisibilityRange * num5;
					m_viewFogColor = CalculateSkyColor2(new Vector3(camera.ViewDirection.X, 0f, camera.ViewDirection.Z), m_subsystemTimeOfDay.TimeOfDay, m_subsystemWeather.GlobalPrecipitationIntensity, temperature2,camera);
					m_viewIsSkyVisible = true;
				}
				if (!DrawSkyEnabled || !m_viewIsSkyVisible || SettingsManager.SkyRenderingMode == SkyRenderingMode.Disabled)
				{
					GameViewWidget gameViewWidget = camera.View.GameWidget.GameViewWidget;
					FlatBatch2D flatBatch2D = m_primitivesRenderer2d.FlatBatch(-1, DepthStencilState.None, RasterizerState.CullNoneScissor, BlendState.Opaque);
					int count = flatBatch2D.TriangleVertices.Count;
					flatBatch2D.QueueQuad(Vector2.Zero, gameViewWidget.ActualSize, 0f, m_viewFogColor);
					flatBatch2D.TransformTriangles(camera.WidgetMatrix, count);
					m_primitivesRenderer2d.Flush();
				}
			}
			else if (drawOrder == m_drawOrders[1])
			{
				if (DrawSkyEnabled && m_viewIsSkyVisible && SettingsManager.SkyRenderingMode != SkyRenderingMode.Disabled)
				{
					DrawSkydome(camera);
					DrawStars(camera);
					DrawSunAndMoon(camera);
					DrawClouds(camera);
					DrawEarth(camera);
					m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix);
				}
			}
			else
			{
				//if (camera.ViewPosition.Y<1000)
				DrawLightning(camera);
				m_primitivesRenderer3d.Flush(camera.ViewProjectionMatrix);
			}
		}
		public new void DrawSunAndMoon(Camera camera)
		{
			float globalPrecipitationIntensity = camera.ViewPosition.Y > 1000f ? 0f : m_subsystemWeather.GlobalPrecipitationIntensity;
			float timeOfDay = m_subsystemTimeOfDay.TimeOfDay;
			float f = MathUtils.Max(CalculateDawnGlowIntensity(timeOfDay), CalculateDuskGlowIntensity(timeOfDay));
			float num = 2f * timeOfDay * (float)Math.PI;
			float angle = num + (float)Math.PI;
			float num2 = MathUtils.Lerp(90f, 160f, f);
			float num3 = MathUtils.Lerp(60f, 80f, f);
			Color c = Color.Lerp(new Color(255, 255, 255), new Color(255, 255, 160), f);
			Color white = Color.White;
			white *= 1f - SkyLightIntensity;
			c *= MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			white *= MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			Color color = c * 0.6f * MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			Color color2 = c * 0.2f * MathUtils.Lerp(1f, 0f, globalPrecipitationIntensity);
			TexturedBatch3D batch = m_primitivesRenderer3d.TexturedBatch(m_glowTexture, useAlphaTest: false, 0, DepthStencilState.DepthRead, null, BlendState.Additive);
			TexturedBatch3D batch2 = m_primitivesRenderer3d.TexturedBatch(m_sunTexture, useAlphaTest: false, 1, DepthStencilState.DepthRead, null, BlendState.AlphaBlend);
			TexturedBatch3D batch3 = m_primitivesRenderer3d.TexturedBatch(m_moonTextures[MoonPhase], useAlphaTest: false, 1, DepthStencilState.DepthRead, null, BlendState.AlphaBlend);
			QueueCelestialBody(batch, camera.ViewPosition, color, 900f, 3.5f * num2, num);
			QueueCelestialBody(batch, camera.ViewPosition, color2, 900f, 3.5f * num3, angle);
			QueueCelestialBody(batch2, camera.ViewPosition, c, 900f, num2, num);
			QueueCelestialBody(batch3, camera.ViewPosition, white, 900f, num3, angle);
		}

		public new void DrawClouds(Camera camera)
		{
			if (SettingsManager.SkyRenderingMode == SkyRenderingMode.NoClouds || camera.ViewPosition.Y>1000f)
			{
				return;
			}
			float globalPrecipitationIntensity = m_subsystemWeather.GlobalPrecipitationIntensity;
			float num = MathUtils.Lerp(0.03f, 1f, MathUtils.Sqr(SkyLightIntensity)) * MathUtils.Lerp(1f, 0.2f, globalPrecipitationIntensity);
			m_cloudsLayerColors[0] = Color.White * (num * 0.75f);
			m_cloudsLayerColors[1] = Color.White * (num * 0.66f);
			m_cloudsLayerColors[2] = ViewFogColor;
			m_cloudsLayerColors[3] = Color.Transparent;
			double gameTime = m_subsystemTime.GameTime;
			Vector3 viewPosition = camera.ViewPosition;
			Vector2 v = new Vector2((float)MathUtils.Remainder(0.0020000000949949026 * gameTime - (double)(viewPosition.X / 1000f * 1.5f), 1.0) + viewPosition.X / 1000f * 1.5f, (float)MathUtils.Remainder(0.0020000000949949026 * gameTime - (double)(viewPosition.Z / 1000f * 1.5f), 1.0) + viewPosition.Z / 1000f * 1.5f);
			TexturedBatch3D texturedBatch3D = m_primitivesRenderer3d.TexturedBatch(m_cloudsTexture, useAlphaTest: false, 2, DepthStencilState.DepthRead, null, BlendState.AlphaBlend, SamplerState.LinearWrap);
			DynamicArray<VertexPositionColorTexture> triangleVertices = texturedBatch3D.TriangleVertices;
			DynamicArray<ushort> triangleIndices = texturedBatch3D.TriangleIndices;
			int count = triangleVertices.Count;
			int num2 = triangleVertices.Count;
			int num3 = triangleIndices.Count;
			triangleVertices.Count += 49;
			triangleIndices.Count += 216;
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					int num4 = j - 3;
					int num5 = i - 3;
					int num6 = MathUtils.Max(MathUtils.Abs(num4), MathUtils.Abs(num5));
					float num7 = m_cloudsLayerRadii[num6];
					float num8 = (num6 > 0) ? (num7 / MathUtils.Sqrt(num4 * num4 + num5 * num5)) : 0f;
					float num9 = (float)num4 * num8;
					float num10 = (float)num5 * num8;
					float y = MathUtils.Lerp(250f, 50f, num7 * num7);

					Vector3 vector = new Vector3(viewPosition.X + num9 * 1000f, y, viewPosition.Z + num10 * 1000f); //height
					Vector2 texCoord = new Vector2(vector.X, vector.Z) / 1000f * 1.5f - v;
					Color color = m_cloudsLayerColors[num6];
					texturedBatch3D.TriangleVertices.Array[num2++] = new VertexPositionColorTexture(vector, color, texCoord);
					if (j > 0 && i > 0)
					{
						ushort num12 = (ushort)(count + j + i * 7);
						ushort num13 = (ushort)(count + (j - 1) + i * 7);
						ushort num14 = (ushort)(count + (j - 1) + (i - 1) * 7);
						ushort num15 = (ushort)(count + j + (i - 1) * 7);
						if ((num4 <= 0 && num5 <= 0) || (num4 > 0 && num5 > 0))
						{
							texturedBatch3D.TriangleIndices.Array[num3++] = num12;
							texturedBatch3D.TriangleIndices.Array[num3++] = num13;
							texturedBatch3D.TriangleIndices.Array[num3++] = num14;
							texturedBatch3D.TriangleIndices.Array[num3++] = num14;
							texturedBatch3D.TriangleIndices.Array[num3++] = num15;
							texturedBatch3D.TriangleIndices.Array[num3++] = num12;
						}
						else
						{
							texturedBatch3D.TriangleIndices.Array[num3++] = num12;
							texturedBatch3D.TriangleIndices.Array[num3++] = num13;
							texturedBatch3D.TriangleIndices.Array[num3++] = num15;
							texturedBatch3D.TriangleIndices.Array[num3++] = num13;
							texturedBatch3D.TriangleIndices.Array[num3++] = num14;
							texturedBatch3D.TriangleIndices.Array[num3++] = num15;
						}
					}
				}
			}
			bool drawCloudsWireframe = DrawCloudsWireframe;
		}

	}



	public class SubsystemWeather2 : SubsystemWeather, IDrawable, IUpdateable
	{
		public bool space;
		public new void Draw(Camera camera, int drawOrder)
		{
			space = camera.ViewPosition.Y>300f ? true : false;
			int num = (SettingsManager.VisibilityRange >= 512) ? 9 : ((SettingsManager.VisibilityRange >= 256) ? 8 : ((SettingsManager.VisibilityRange < 128) ? 6 : 7));
			int num2 = num * num;
			Dictionary<Point2, Shaft> activeShafts = GetActiveShafts(camera.View);
			byte b = (byte)(255f * MathUtils.Lerp(0.1f, 1f, m_subsystemSky.SkyLightIntensity));
			m_snowColor = new Color(b, b, b);
			byte b2 = (byte)(255f * MathUtils.Lerp(0.1f, 1f, m_subsystemSky.SkyLightIntensity));
			m_rainColor = new Color(b2, b2, b2);
			Vector2 vector = new Vector2(camera.ViewPosition.X, camera.ViewPosition.Z);
			Point2 point = Terrain.ToCell(vector);
			Vector2? value = null;
			m_lastShaftsUpdatePositions.TryGetValue(camera.View, out value);
			if (!value.HasValue || Vector2.DistanceSquared(value.Value, vector) > 1f)
			{
				m_lastShaftsUpdatePositions[camera.View] = vector;
				m_toRemove.Clear();
				foreach (Shaft value3 in activeShafts.Values)
				{
					if (MathUtils.Sqr((float)value3.Point.X + 0.5f - vector.X) + MathUtils.Sqr((float)value3.Point.Y + 0.5f - vector.Y) > (float)num2 + 1f)
					{
						m_toRemove.Add(value3);
					}
				}
				foreach (Shaft item in m_toRemove)
				{
					FreeShaft(item);
					activeShafts.Remove(item.Point);
				}
				for (int i = point.X - num; i <= point.X + num; i++)
				{
					for (int j = point.Y - num; j <= point.Y + num; j++)
					{
						if (MathUtils.Sqr((float)i + 0.5f - vector.X) + MathUtils.Sqr((float)j + 0.5f - vector.Y) <= (float)num2)
						{
							Point2 point2 = new Point2(i, j);
							if (!activeShafts.ContainsKey(point2))
							{
								Shaft value2 = AllocateShaft(camera.View, point2);
								activeShafts.Add(point2, value2);
							}
						}
					}
				}
			}
			if (m_subsystemTime.PeriodicGameTimeEvent(1.0, 0.0) || !m_lastShaftsUpdateLightIntensity.HasValue || MathUtils.Abs(m_lastShaftsUpdateLightIntensity.Value - m_subsystemSky.SkyLightIntensity) > 0.1f)
			{
				m_lastShaftsUpdateLightIntensity = m_subsystemSky.SkyLightIntensity;
				foreach (Shaft value4 in activeShafts.Values)
				{
					UpdateShaft(value4);
				}
			}
		}

		public new void Update(float dt)
		{
			if (m_subsystemGameInfo.TotalElapsedGameTime > m_precipitationEndTime)
			{
				if (m_precipitationEndTime == 0.0)
				{
					m_precipitationStartTime = m_subsystemGameInfo.TotalElapsedGameTime + (double)(60f * m_random.UniformFloat(3f, 6f));
					m_lightningIntensity = m_random.UniformFloat(0.33f, 0.66f);
				}
				else
				{
					m_precipitationStartTime = m_subsystemGameInfo.TotalElapsedGameTime + (double)(60f * m_random.UniformFloat(5f, 45f));
					m_lightningIntensity = ((m_random.UniformFloat(0f, 1f) < 0.5f) ? m_random.UniformFloat(0.33f, 1f) : 0f);
				}
				m_precipitationEndTime = m_precipitationStartTime + (double)(60f * m_random.UniformFloat(3f, 6f));
			}
			float num = (float)MathUtils.Max(0.0, MathUtils.Min(m_subsystemGameInfo.TotalElapsedGameTime - m_precipitationStartTime, m_precipitationEndTime - m_subsystemGameInfo.TotalElapsedGameTime));
			m_globalPrecipitationIntensity = (m_subsystemGameInfo.WorldSettings.AreWeatherEffectsEnabled && !space ? MathUtils.Saturate(num * 0.04f) : 0f);
			if (space)
				return;
			if (m_globalPrecipitationIntensity == 1f && m_subsystemTime.PeriodicGameTimeEvent(1.0, 0.0))
			{
				TerrainChunk[] allocatedChunks = m_subsystemTerrain.Terrain.AllocatedChunks;
				for (int i = 0; i < allocatedChunks.Length; i++)
				{
					TerrainChunk terrainChunk = allocatedChunks[m_random.UniformInt(0, allocatedChunks.Length - 1)];
					if (terrainChunk.State < TerrainChunkState.InvalidVertices1 || !m_random.Bool(m_lightningIntensity * 0.0002f))
					{
						continue;
					}
					int num2 = terrainChunk.Origin.X + m_random.UniformInt(0, 15);
					int num3 = terrainChunk.Origin.Y + m_random.UniformInt(0, 15);
					Vector3? vector = null;
					for (int j = num2 - 8; j < num2 + 8; j++)
					{
						for (int k = num3 - 8; k < num3 + 8; k++)
						{
							int topHeight = m_subsystemTerrain.Terrain.GetTopHeight(j, k);
							if (!vector.HasValue || (float)topHeight > vector.Value.Y)
							{
								vector = new Vector3(j, topHeight, k);
							}
						}
					}
					if (vector.HasValue)
					{
						m_subsystemSky.MakeLightningStrike(vector.Value);
						return;
					}
				}
			}
			if (Time.PeriodicEvent(0.5, 0.0))
			{
				float num4 = 0f;
				if (m_globalPrecipitationIntensity > 0f)
				{
					float num5 = 0f;
					foreach (Vector3 listenerPosition in m_subsystemAudio.ListenerPositions)
					{
						int num6 = Terrain.ToCell(listenerPosition.X) - 5;
						int num7 = Terrain.ToCell(listenerPosition.Z) - 5;
						int num8 = Terrain.ToCell(listenerPosition.X) + 5;
						int num9 = Terrain.ToCell(listenerPosition.Z) + 5;
						for (int l = num6; l <= num8; l++)
						{
							for (int m = num7; m <= num9; m++)
							{
								PrecipitationShaftInfo precipitationShaftInfo = GetPrecipitationShaftInfo(l, m);
								if (precipitationShaftInfo.Type == PrecipitationType.Rain && precipitationShaftInfo.Intensity > 0f)
								{
									Vector3 vector2 = default(Vector3);
									vector2.X = (float)l + 0.5f;
									vector2.Y = MathUtils.Max(precipitationShaftInfo.YLimit, listenerPosition.Y);
									vector2.Z = (float)m + 0.5f;
									float num10 = vector2.X - listenerPosition.X;
									float num11 = 8f * (vector2.Y - listenerPosition.Y);
									float num12 = vector2.Z - listenerPosition.Z;
									float distance = MathUtils.Sqrt(num10 * num10 + num11 * num11 + num12 * num12);
									num5 += m_subsystemAudio.CalculateVolume(distance, 1.5f) * precipitationShaftInfo.Intensity;
								}
							}
						}
					}
					num4 = MathUtils.Max(num4, num5);
				}
				m_targetRainSoundVolume = MathUtils.Saturate(1.5f * num4 / m_rainVolumeFactor);
			}
			m_rainSound.Volume = MathUtils.Saturate(MathUtils.Lerp(m_rainSound.Volume, SettingsManager.SoundsVolume * m_targetRainSoundVolume, 5f * dt));
			if (m_rainSound.Volume > AudioManager.MinAudibleVolume)
			{
				m_rainSound.Play();
			}
			else
			{
				m_rainSound.Pause();
			}
		}

	}



}