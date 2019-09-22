using Chemistry;
using Engine;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public abstract class ComponentMachine : ComponentInventoryBase, ICraftingMachine
	{
		public float m_fireTimeRemaining;
		protected int m_furnaceSize;
		protected ComponentBlockEntity m_componentBlockEntity;
		protected bool m_updateSmeltingRecipe;

		public virtual int FuelSlotIndex => SlotsCount - 2;

		public virtual int ResultSlotIndex => 0;
		public virtual int RemainsSlotIndex => SlotsCount - 1;

		public int SlotIndex { get => -1; set => throw new NotImplementedException(); }

		public int UpdateOrder => 0;

		public float HeatLevel;

		public float SmeltingProgress;

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex == FuelSlotIndex)
			{
				var block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
				if ((block is IFuel fuel ? fuel.GetHeatLevel(value) : block.FuelHeatLevel) < 1f)
					return 0;
			}
			int v = GetSlotValue(slotIndex);
			if ((v == 262384 || v == 786672 || v == 1048816 || v == 1310960 || v == WaterBlock.Index || v == MagmaBlock.Index) && value != v && v != 0)
			{
				return 0;
			}
			return base.GetSlotCapacity(slotIndex, value);
		}

		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			if (GetSlotCount(slotIndex) != 0 && (GetSlotValue(slotIndex) != value || GetSlotCount(slotIndex) + count > GetSlotCapacity(slotIndex, value)))
				return;
			base.AddSlotItems(slotIndex, value, count);
			m_updateSmeltingRecipe = true;
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			int v = GetSlotValue(slotIndex);
			if (v == 262384 || v == 786672 || v == 1048816 || v == 1310960 || v == WaterBlock.Index || v == MagmaBlock.Index)
			{
				return 0;
			}
			return base.RemoveSlotItems(slotIndex, count);
		}

		public int RemoveSlotItems2(int slotIndex, int count)
		{
			return base.RemoveSlotItems(slotIndex, count);
		}

		public override void ProcessSlotItems(int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			IItem item = Item.Block.GetItem(ref value);
			if (slotIndex == FuelSlotIndex && item is FuelCylinder fuel)
			{
				HeatLevel = fuel.GetHeatLevel(value);
				m_fireTimeRemaining = fuel.GetFuelFireDuration(value);
				processedValue = fuel.GetDamageDestructionValue();
				processedCount = 1;
				return;
			}
			base.ProcessSlotItems(slotIndex, value, count, processCount, out processedValue, out processedCount);
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>();
			m_updateSmeltingRecipe = true;
			m_fireTimeRemaining = valuesDictionary.GetValue("FireTimeRemaining", 0f);
			HeatLevel = valuesDictionary.GetValue("HeatLevel", 0f);
		}

		public CraftingRecipe GetRecipe() => null;

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
			if (m_fireTimeRemaining > 0f)
				valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			if (HeatLevel > 0f)
				valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}

		public int FindSmeltingRecipe(Dictionary<int, int> result, int value)
		{
			if (value == 0)
				return 0;
			var e = result.GetEnumerator();
			while (e.MoveNext())
			{
				int index = FindAcquireSlotForItem(this, e.Current.Key);
				if (index < 0)
					return 0;
			}
			return value;
		}

		public new void DropAllItems(Vector3 position)
		{
			SubsystemPickables subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(throwOnError: true);
			for (int i = 0; i < SlotsCount; i++)
			{
				int slotCount = GetSlotCount(i);
				int v = GetSlotValue(i);
				if (slotCount > 0 && !(v == 262384 || v == 786672 || v == 1048816 || v == 1310960 || v == WaterBlock.Index || v == MagmaBlock.Index))
				{
					int slotValue = GetSlotValue(i);
					int count = RemoveSlotItems(i, slotCount);
					Vector3 value = m_random.UniformFloat(5f, 10f) * Vector3.Normalize(new Vector3(m_random.UniformFloat(-1f, 1f), m_random.UniformFloat(1f, 2f), m_random.UniformFloat(-1f, 1f)));
					subsystemPickables.AddPickable(slotValue, count, position, value, null);
				}
			}
		}

		/*public static float GetFuelHeatLevel(int value)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			return block is IFuel fuel ? fuel.GetHeatLevel(value) : block.FuelHeatLevel;
		}*/

		public int FindSmeltingRecipe(Dictionary<int, int> result, ReactionSystem system, Condition condition = Condition.H | Condition.l, ushort t = 2200)
		{
			result.Clear();
			int value, count;
			for (int i = 0; i < m_furnaceSize; i++)
			{
				if (GetSlotCount(i) <= 0) continue;
				var item = ChemicalBlock.Get(GetSlotValue(i));
				if (item != null)
				{
					system.Add(item.GetDispersionSystem(), GetSlotCount(i));
					value = item.GetDamageDestructionValue();
					if (value != 0)
					{
						result.TryGetValue(value, out count);
						result[value] = count + GetSlotCount(i);
					}
				}
			}
			Equation equation = system.React(condition, t);
			if (equation == null)
				return 0;
			system.Normalize();
			Dictionary<Compound, int>.Enumerator e;
			for (e = equation.Reactants.GetEnumerator(); e.MoveNext();)
			{
				result[ChemicalBlock.Get(e.Current.Key.ToString())] = -e.Current.Value / 1000;
			}
			for (e = system.GetEnumerator(); e.MoveNext();)
			{
				int val = ChemicalBlock.Get(e.Current.Key.ToString());
				if (ChemicalBlock.Get(val) == null)
					return 0;
				value = ChemicalBlock.Get(val).GetDamageDestructionValue();

				if (value != 0)
				{
					result.TryGetValue(value, out count);
					count -= e.Current.Value;
					if (count < 0)
						continue;
					if (count > 0)
						result[value] = count;
				}
				value = e.Current.Value;
				if (value > 999)
					value /= 1000;
				if (value > 200)
					value = 1;
				result[val] = value;
			}
			return equation.GetHashCode();
		}
	}

	public abstract class ComponentPMach : ComponentSMachine, IUpdateable
	{
		protected int m_count;
		protected float m_speed;
		protected string m_smeltingRecipe;

		//protected int m_music;

		protected string m_smeltingRecipe2;

		public override int ResultSlotIndex => SlotsCount - 1;

		public void Update(float dt)
		{
			if (HeatLevel > 0f)
			{
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
				if (m_fireTimeRemaining == 0f)
					HeatLevel = 0f;
			}
			if (m_updateSmeltingRecipe)
			{
				m_updateSmeltingRecipe = false;
				m_smeltingRecipe2 = FindSmeltingRecipe();
				if (m_smeltingRecipe2 != m_smeltingRecipe)
				{
					m_smeltingRecipe = m_smeltingRecipe2;
					SmeltingProgress = 0f;
					//m_music = 0;
				}
			}
			if (m_smeltingRecipe2 != null)
			{
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				m_smeltingRecipe = ComponentEngine.IsPowered(Utils.Terrain, coordinates.X, coordinates.Y, coordinates.Z) ? m_smeltingRecipe2 : null;
			}
			if (m_smeltingRecipe == null)
			{
				HeatLevel = 0f;
				m_fireTimeRemaining = 0f;
				//m_music = -1;
			}
			else
				m_fireTimeRemaining = 100f;
			if (m_fireTimeRemaining <= 0f)
			{
				m_smeltingRecipe = null;
				SmeltingProgress = 0f;
				//m_music = -1;
			}
			if (m_smeltingRecipe != null)
			{
				SmeltingProgress = MathUtils.Min(SmeltingProgress + m_speed * dt, 1f);
				if (SmeltingProgress >= 1f)
				{
					for (int l = 0; l < m_furnaceSize; l++)
						if (m_slots[l].Count > 0)
							m_slots[l].Count--;
					m_slots[ResultSlotIndex].Value = ItemBlock.IdTable[m_smeltingRecipe];
					m_slots[ResultSlotIndex].Count += m_count;
					m_smeltingRecipe = null;
					SmeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_furnaceSize = SlotsCount - 1;
			//m_speed = valuesDictionary.GetValue("Speed", 0.1f);
			//m_count = valuesDictionary.GetValue("Count", 1);
			m_speed = 0.1f;
			m_count = 1;
		}

		protected virtual string FindSmeltingRecipe()
		{
			return null;
		}
	}

	public class ComponentSMachine : ComponentMachine
	{
		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => -1;

		public override int FuelSlotIndex => -1;
	}

	public class ComponentSorter : ComponentSMachine
	{
	}

	public class ComponentRCore : ComponentSMachine, IUpdateable
	{
		//public override float m_fireTimeRemaining;
		//public float HeatLevel;
		public float veloc;

		public void Update(float dt)
		{
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.2, 0.0))
				
			{
				int veo = 0;
				if (m_updateSmeltingRecipe)
				{
					
					m_fireTimeRemaining = 0f;
					veloc = 1f;
					for (int i = 0; i < SlotsCount; i++)
					{
						//if ()
						int va1 = GetSlotValue(i);

						if (Terrain.ExtractContents(va1) == FuelRodBlock.Index)
						{
							if (FuelRodBlock.GetType(va1) == RodType.UFuelRod)
							{
								m_fireTimeRemaining += 2;
								veo += 0;
							}
							if (FuelRodBlock.GetType(va1) == RodType.ControlRod)
							{
								m_fireTimeRemaining -= 2;
								veloc -= 0.2f;
							}
							if (FuelRodBlock.GetType(va1) == RodType.CarbonRod)
							{
								m_fireTimeRemaining += 1;
								veloc += 0.2f;
							}
						}
					}
				}
				veloc = MathUtils.Max(veloc,0);
				m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining );
				HeatLevel = MathUtils.Max(0f, HeatLevel + m_fireTimeRemaining * 0.5f -0.1f);
				//HeatLevel += m_fireTimeRemaining*5;
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				if (HeatLevel>3000)
				Utils.SubsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, LargeGunpowderKegBlock.Index);
				if (m_fireTimeRemaining>0)
				for (int i = 0; i < SlotsCount; i++)
				{
					//if ()
					int va1 = GetSlotValue(i);
					if (Terrain.ExtractContents(va1) == FuelRodBlock.Index)
					{
						if (Utils.Random.Bool(0.006f*veloc))
						{
							RemoveSlotItems(i, 1);
							if (BlocksManager.DamageItem(va1, 1) !=0)
							{
								AddSlotItems(i, BlocksManager.DamageItem(va1, 1), 1);
							}
						}
					}
					else
					{
						RemoveSlotItems(i, 1);
					}
				}
			}
		}
	}




	public class ComponentHChanger : ComponentMachine, IUpdateable
	{
		public override int RemainsSlotIndex => -1;

		public override int ResultSlotIndex => -1;

		public override int FuelSlotIndex => -1;

		public int Pressure;

		public int Output;

		public void Update(float dt)
		{
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.5, 0.0))
			{
				Pressure = 0;
				Output = 0;
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				var point = CellFace.FaceToPoint3(Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)) >> 15);
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z, v;
				int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4, num5), 0);
				//int cellContents = Terrain.ExtractContents(cellValue);Terrain.ExtractData(value) >> 15
				if (4 == Terrain.ExtractData(cellValue) >> 10)
				{
					var point3 = new Point3(num3, num4, num5);
					var entity = Utils.GetBlockEntity(point3);
					ComponentRCore component3 = entity.Entity.FindComponent<ComponentRCore>();
					if (entity != null && component3 != null)
					{
						Pressure = (int)(component3.HeatLevel/100);


						if (Pressure > 0)
						{

							
								for (int i = 0; i < 8; i++)
								{
									int slotValue = GetSlotValue(i);
									int num = Terrain.ExtractContents(slotValue);
									if (slotValue == WaterBlock.Index)
									{
										if (Utils.SubsystemTime.PeriodicGameTimeEvent((5*30/Pressure/10)/1f, 0.0))
										m_slots[i].Count--;
										Output += Pressure;
										component3.HeatLevel -= 2*Output;
										break;
										
									}

								
							}
						}


					}
				
				}

			}

		}
	}



	public class ComponentTurbine : ComponentMachine, IUpdateable
	{

		public int Pressure;

		public int Output;
		public bool Powered;

		public void Update(float dt)
		{
			if (Utils.SubsystemTime.PeriodicGameTimeEvent(0.5, 0.0))
			{
				Pressure = 0;
				Output = 0;
				Point3 coordinates = m_componentBlockEntity.Coordinates;
				var point = CellFace.FaceToPoint3(Terrain.ExtractData(Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z)) >> 15);
				int num3 = coordinates.X - point.X;
				int num4 = coordinates.Y - point.Y;
				int num5 = coordinates.Z - point.Z, v;
				int cellValue = Terrain.ReplaceLight(Utils.Terrain.GetCellValue(num3, num4, num5), 0);
				//int cellContents = Terrain.ExtractContents(cellValue);Terrain.ExtractData(value) >> 15
				if (ElementBlock.Block.GetDevice(num3, num4, num5, cellValue) is Hchanger)
				{
					var point3 = new Point3(num3, num4, num5);
					var entity = Utils.GetBlockEntity(point3);
					ComponentHChanger component3 = entity.Entity.FindComponent<ComponentHChanger>();
					Output = component3.Output;

				}
				Powered = Output > 0;

			}

		}
	}


}