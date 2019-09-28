using Engine;

namespace Game
{
	public class SubsystemMusket5BlockBehavior : SubsystemMusketBlockBehavior
	{
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			componentPlayer.ComponentGui.ModalPanelWidget = componentPlayer.ComponentGui.ModalPanelWidget == null ? new Musket2Widget(inventory, slotIndex) : null;
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
					int slotValue = inventory.GetSlotValue(activeSlotIndex);
					int slotCount = inventory.GetSlotCount(activeSlotIndex);
					int num = Terrain.ExtractContents(slotValue);
					int data = Terrain.ExtractData(slotValue);
					int num2 = slotValue;
					int num3 = 0;
					if (num == Musket5Block.Index && slotCount > 0)
					{
						if (!m_aimStartTimes.TryGetValue(componentMiner, out double value))
						{
							value = m_subsystemTime.GameTime;
							m_aimStartTimes[componentMiner] = value;
						}
						float num4 = (float)(m_subsystemTime.GameTime - value);
						float num5 = (float)MathUtils.Remainder(m_subsystemTime.GameTime, 1000.0);
						//direction = Vector3.Normalize(direction + v);
						
						switch (state)
						{
							case AimState.InProgress:
								{
									if (num4 >= 30f)
									{
										componentMiner.ComponentCreature.ComponentCreatureSounds.PlayMoanSound();
										return true;
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
									componentMiner.ComponentCreature.ComponentCreatureModel.AimHandAngleOrder = 1.4f;
									componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemOffsetOrder = new Vector3(-0.08f, -0.08f, 0.07f);
									componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemRotationOrder = new Vector3(-1.7f, 0f, 0f);
									break;
								}
							case AimState.Cancelled:
								m_subsystemAudio.PlaySound("Audio/HammerUncock", 1f, m_random.UniformFloat(-0.1f, 0.1f), 0f, 0f);
								m_aimStartTimes.Remove(componentMiner);
								break;
							case AimState.Completed:
								{
									
									int dur = GetDamage(slotValue);
									if (dur > 3889)
										return false;
									Vector3 eyePosition = componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition;
									Matrix matrix = componentMiner.ComponentCreature.ComponentBody.Matrix;
									Vector3 vectorp = eyePosition + matrix.Right * 0.3f - matrix.Up * 0.2f;
									ComponentNPlayer miner2 = componentMiner.Entity.FindComponent<ComponentNPlayer>();
									var result = componentMiner.PickTerrainForDigging(vectorp, direction);
									var body = componentMiner.PickBody(vectorp, direction);
									if (miner2 != null)
									{
										//componentMiner.ComponentPlayer?.ComponentGui.DisplaySmallMessage((99999999).ToString(), blinking: true, playNotificationSound: false);
										result = miner2.PickTerrainForDigging2(vectorp, direction, 100f);
										body = miner2.PickBody2(vectorp, direction, 100f);
									}
									//componentMiner.ComponentPlayer?.ComponentGui.DisplaySmallMessage((11111111).ToString(), blinking: true, playNotificationSound: false);
									float end = 100f; 
									if (body.HasValue && (!result.HasValue || body.Value.Distance < result.Value.Distance))
									{
										ComponentCreature cre1 = body.Value.ComponentBody.Entity.FindComponent<ComponentCreature>();
										if (cre1 != null)
										{
											int nnn = (int)cre1.Entity.FindComponent<ComponentHealth>()?.AttackResilience / 8;
											for (int zz = 0; zz <= nnn; zz++)
												Utils.SubsystemPickables.AddPickable(ItemBlock.IdTable["CoalPowder"], 1, cre1.ComponentBody.Position, null, null);
											Utils.SubsystemParticles.AddParticleSystem(new KillParticleSystem(Utils.SubsystemTerrain, cre1.ComponentBody.Position, 1f));
											end = MathUtils.Abs(Vector3.Distance(cre1.ComponentBody.Position,vectorp));
											//end = vectorp + direction * 100f;
											if (cre1.Entity.FindComponent<ComponentNPlayer>() != null)
											{
												cre1.Entity.FindComponent<ComponentHealth>()?.Injure(1f, null, false, "Killed by Laser");
											}
											else
												Project.RemoveEntity(cre1.Entity, true);
										}
									}
									else if (result.HasValue)
									{
										Point3 point = result.Value.CellFace.Point;
										int value2 = Utils.Terrain.GetCellValueFast(point.X, point.Y, point.Z);
										int value3 = Utils.Terrain.GetCellContentsFast(point.X, point.Y, point.Z);
										if (value != 0)
										{
											var block = BlocksManager.Blocks[value3];
											
												if (block.IsPlaceable)
												{
													end = MathUtils.Abs(Vector3.Distance(new Vector3(point), vectorp));
												if (block.ExplosionResilience >= 1000f)
													{
														
													}
													else if (block.DefaultExplosionPressure > 0)
													{
														Utils.SubsystemExplosions.TryExplodeBlock(point.X, point.Y, point.Z, value2);
													}
													else if (block.FuelFireDuration > 0)
													{
														int v1 = (int)block.FuelFireDuration / 10;
														for (int zz = 0; zz <= v1; zz++)
															Utils.SubsystemPickables.AddPickable(ItemBlock.IdTable["CoalPowder"], 1, new Vector3(point.X, point.Y, point.Z), null, null);
														Utils.SubsystemTerrain.DestroyCell(5, point.X, point.Y, point.Z, 0, true, false);
													}
													else
														Utils.SubsystemTerrain.DestroyCell(5, point.X, point.Y, point.Z, 0, false, false);
													
												}
										}
									}
									num3 = 100;
									if (num3 > 0)
										componentMiner.DamageActiveTool(num3);
									//num2 = Terrain.MakeBlockValue(Terrain.ExtractContents(num2), 0, Musket5Block.SetHammerState(Terrain.ExtractData(num2), false));
									Utils.SubsystemLaser.MakeLightningStrike(new Vector3(vectorp.X, vectorp.Y, vectorp.Z) + 1.1f*direction, new Vector3(vectorp.X, vectorp.Y, vectorp.Z) + end * direction);
									m_aimStartTimes.Remove(componentMiner);
									break;
								}
						}
					}
					
				}
			}
			return false;
		}
		public int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) >> 4) & 4095;
		}
		public override int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value)
		{
			return 0;
		}

	}
}