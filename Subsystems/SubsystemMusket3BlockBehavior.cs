using Engine;

namespace Game
{
	public class SubsystemMusket3BlockBehavior : SubsystemMusket2BlockBehavior
	{
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			componentPlayer.ComponentGui.ModalPanelWidget = componentPlayer.ComponentGui.ModalPanelWidget == null ? new Musket2Widget(inventory, slotIndex, Musket3Block.Index) : null;
			return true;
		}

		public override bool OnAim(Vector3 start, Vector3 direction, ComponentMiner componentMiner, AimState state)
		{
			IInventory inventory = componentMiner.Inventory;
			if (inventory != null)
			{
				int activeSlotIndex = inventory.ActiveSlotIndex;
				if (activeSlotIndex >= 0)
				{
					int slotValue = inventory.GetSlotValue(activeSlotIndex),
						slotCount = inventory.GetSlotCount(activeSlotIndex);
					int num = Terrain.ExtractContents(slotValue);
					//int data = Terrain.ExtractData(slotValue);
					int num2 = slotValue;
					Vector3 dir = direction;
					if (num == Musket3Block.Index && slotCount > 0)
					{
						if (!m_aimStartTimes.TryGetValue(componentMiner, out double value))
						{
							value = m_subsystemTime.GameTime;
							m_aimStartTimes[componentMiner] = value;
						}
						float num4 = (float)(m_subsystemTime.GameTime - value);
						float num5 = (float)MathUtils.Remainder(m_subsystemTime.GameTime, 1000.0);
						Vector3 v = (float)((componentMiner.ComponentCreature.ComponentBody.IsSneaking ? 0.00999999977648258 : 0.0299999993294477) + 0.200000002980232 * MathUtils.Saturate((num4 - 6.5f) / 40f)) * new Vector3
						{
							X = SimplexNoise.OctavedNoise(num5, 2f, 3, 2f, 0.5f),
							Y = SimplexNoise.OctavedNoise(num5 + 100f, 2f, 3, 2f, 0.5f),
							Z = SimplexNoise.OctavedNoise(num5 + 200f, 2f, 3, 2f, 0.5f)
						};
						if (num4 > 1f)
						{
							if (0.2f * num4 < 1.2f)
							{
								direction.Y += 0.1f * (num4 - 1f);
							}
							else
							{
								direction.Y += 0.5f;
							}
						}

						direction = Vector3.Normalize(direction + v * 2f);
						switch (state)
						{
							case AimState.InProgress:
								{
									if (num4 > 0.3f && !Musket2Block.GetHammerState(Terrain.ExtractData(num2)))
									{
										num2 = Terrain.MakeBlockValue(num, 0, Musket2Block.SetHammerState(Terrain.ExtractData(num2), true));
										m_subsystemAudio.PlaySound("Audio/HammerCock", 1f, m_random.UniformFloat(-0.1f, 0.1f), 0f, 0f);
									}
									ComponentFirstPersonModel componentFirstPersonModel = componentMiner.Entity.FindComponent<ComponentFirstPersonModel>();
									if (componentFirstPersonModel != null)
									{
										ComponentPlayer componentPlayer2 = componentMiner.ComponentPlayer;
										if (componentPlayer2 != null)
											componentPlayer2.ComponentGui.ShowAimingSights(start, direction);

										componentFirstPersonModel.ItemOffsetOrder = new Vector3(-0.21f, 0.15f, 0.08f);
										componentFirstPersonModel.ItemRotationOrder = new Vector3(-0.7f, 0f, 0f);
									}

									if (m_subsystemTime.PeriodicGameTimeEvent(0.17, 0) && Musket3Block.GetBulletNum(Terrain.ExtractData(inventory.GetSlotValue(activeSlotIndex))) > 0)
									{
										//if (componentMiner.ComponentCreature.ComponentBody.ImmersionFactor > 0.4f)
										//	m_subsystemAudio.PlaySound("Audio/MusketMisfire", 1f, m_random.UniformFloat(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 3f, true);
										//else
										{
											Vector3 eyePosition = componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition;
											Matrix matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
											Vector3 v2 = eyePosition + matrix.Right * 0.3f;
											matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
											Vector3 vector2 = v2 - matrix.Up * 0.2f;
											var vector3 = Vector3.Normalize(vector2 + direction * 10f - vector2);
											var vector4 = Vector3.Normalize(Vector3.Cross(vector3, Vector3.UnitY));
											var v3 = Vector3.Normalize(Vector3.Cross(vector3, vector4));
											int num6 = 1;
											var vector = new Vector3(0.01f, 0.01f, 0.05f);
											for (int i = 0; i < num6; i++)
											{
												int value2 = Terrain.MakeBlockValue(214, 0, BulletBlock.SetBulletType(0, BulletBlock.BulletType.CBullet));
												Vector3 v4 = m_random.UniformFloat(0f - vector.X, vector.X) * vector4 + m_random.UniformFloat(0f - vector.Y, vector.Y) * v3 + m_random.UniformFloat(0f - vector.Z, vector.Z) * vector3;
												Projectile projectile = m_subsystemProjectiles.FireProjectile(value2, vector2, 280f * (vector3 + v4), Vector3.Zero, componentMiner.ComponentCreature);
												if (projectile != null)
													projectile.ProjectileStoppedAction = ProjectileStoppedAction.Disappear;
											}
											//m_subsystemAudio.Dispose();
											m_subsystemAudio.PlaySound("Audio/MusketFire", 1f, m_random.UniformFloat(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 10f, true);
											//m_subsystemParticles.AddParticleSystem(new BurntDebrisParticleSystem(base.SubsystemTerrain, (vector2 + 0.3f * vector3)));
											//fireParticleSystem = new FireParticleSystem(vector2 + 1.3f * vector3, 0.3f, 3f);
											//m_subsystemParticles.AddParticleSystem(fireParticleSystem);m_random.UniformFloat(-0.1f, 0.1f)
											m_subsystemParticles.AddParticleSystem(new GunSmokeParticleSystem2(SubsystemTerrain, vector2 + 1.3f * dir, dir));
											m_subsystemNoise.MakeNoise(vector2, 1f, 40f);

											int value23 = Terrain.ExtractData(inventory.GetSlotValue(activeSlotIndex));
											int num44 = Musket3Block.GetBulletNum(value23);
											componentMiner.ComponentPlayer?.ComponentGui.DisplaySmallMessage((num44-1).ToString(), blinking: true, playNotificationSound: false);
											inventory.RemoveSlotItems(activeSlotIndex, 1);
											inventory.AddSlotItems(activeSlotIndex, Terrain.MakeBlockValue(Musket3Block.Index, 0, Musket3Block.SetBulletNum(num44 - 1)), 1);
											if (Utils.Random.Bool(0.1f))
												componentMiner.DamageActiveTool(1);
											//componentMiner.ComponentCreature.ComponentBody.ApplyImpulse(-1f * vector3);
										}
									}
									componentMiner.ComponentCreature.ComponentCreatureModel.AimHandAngleOrder = 1.4f;
									componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemOffsetOrder = new Vector3(-0.08f, -0.08f, 0.07f);
									componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemRotationOrder = new Vector3(-1.7f, 0f, 0f);
									break;
								}
							case AimState.Cancelled:
								if (Musket2Block.GetHammerState(Terrain.ExtractData(num2)))
								{
									num2 = Terrain.MakeBlockValue(num, 0, Musket2Block.SetHammerState(Terrain.ExtractData(num2), false));
									m_subsystemAudio.PlaySound("Audio/HammerUncock", 1f, m_random.UniformFloat(-0.1f, 0.1f), 0f, 0f);
								}
								m_aimStartTimes.Remove(componentMiner);
								break;
							case AimState.Completed:
								{
									if (Musket2Block.GetHammerState(Terrain.ExtractData(num2)))
									{
										num2 = Terrain.MakeBlockValue(num, 0, Musket2Block.SetHammerState(Terrain.ExtractData(num2), false));
										m_subsystemAudio.PlaySound("Audio/HammerUncock", 1f, m_random.UniformFloat(-0.1f, 0.1f), 0f, 0f);
									}
									m_aimStartTimes.Remove(componentMiner);
									break;
								}
						}
					}
					if (num2 != slotValue)
					{
						inventory.RemoveSlotItems(activeSlotIndex, 1);
						inventory.AddSlotItems(activeSlotIndex, num2, 1);
					}
				}
			}
			return false;
		}

		public override int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value)
		{
			return Terrain.ExtractContents(value) != Bullet2Block.Index || Musket3Block.GetBulletNum(Terrain.ExtractData(inventory.GetSlotValue(slotIndex))) > 62
				? 0
				: inventory.GetSlotCount(slotIndex);
		}

		public override void ProcessInventoryItem(IInventory inventory, int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			processedValue = value;
			processedCount = count;
			int value22 = Terrain.ExtractData(inventory.GetSlotValue(slotIndex));
			int num = Musket3Block.GetBulletNum(value22);
			if (processCount + num < 64)
			{
				var loadState = Musket2Block.GetLoadState(Terrain.ExtractData(inventory.GetSlotValue(slotIndex)));
				processedValue = 0;
				processedCount = 0;
				inventory.RemoveSlotItems(slotIndex, 1);

				inventory.AddSlotItems(slotIndex, Terrain.MakeBlockValue(Musket3Block.Index, 0, Musket3Block.SetBulletNum(num + processCount)), 1);
			}
		}
	}
}