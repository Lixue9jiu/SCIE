using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class Gunpowder : OreChunk, IFuel
	{
		public Gunpowder() : base(Matrix.CreateScale(0.75f) * Matrix.CreateRotationX(4f) * Matrix.CreateRotationZ(3f), Matrix.CreateScale(1f) * Matrix.CreateTranslation(0.0625f, 0.875f, 0f), Color.White, false, Materials.Steel)
		{
			DefaultDisplayName = "Gunpowder";
			DefaultDescription = "A substance capable of burning in an oxygenless environment because it contains its own source of oxygen, saltpeter. When burning, gunpowder emits large amounts of hot gases. Will explode with devastating force if confined in a small volume, such as a keg, and ignited.";
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
		}
		public override string GetCategory(int value)
		{
			return "Items";
		}
		public float GetHeatLevel(int value)
		{
			return 1650f;
		}
		public float GetFuelFireDuration(int value)
		{
			return 0.1f;
		}
	}
	public class PureGunpowder : Powder, IFuel
	{
		public readonly float ExplosionPressure;
		public PureGunpowder(string name, Color color, float ep = 100f) : base(name, color)
		{
			ExplosionPressure = ep;
		}

		public override float GetExplosionPressure(int value)
		{
			return ExplosionPressure;
		}
		public override bool GetExplosionIncendiary(int value)
		{
			return true;
		}
		public float GetHeatLevel(int value)
		{
			return 18.5f * ExplosionPressure;
		}
		public float GetFuelFireDuration(int value)
		{
			return 0.1f;
		}
	}
	public class GunpowderBlock : ItemBlock
	{
		public new const int Index = 109;
		public new static Item[] Items = new Item[]
		{
			new Gunpowder(),
			new PureGunpowder("Pure Gunpowder", new Color(24, 24, 24)),
			new PureGunpowder("TNT", Color.Yellow, 150f),
			new PureGunpowder("Amatol", Color.Black, 180f),
			new PureGunpowder("Amodyn", new Color(48, 48, 48), 110f),
			new PureGunpowder("Amogel", Color.Gray, 90f),
			new PureGunpowder("Gelignite", Color.DarkYellow, 100f),
			new PureGunpowder("Lyddite", Color.Gray),
			new PureGunpowder("Roburite", new Color(72, 72, 72), 90f),
			new PureGunpowder("Xyloidine", Color.DarkYellow, 90f),
		};

		static GunpowderBlock()
		{
			var list = new List<Item>(Items);
			list.AddRange(Mine.Mines);
			Items = list.ToArray();
			for (int i = 0; i < Items.Length; i++)
				IdTable.Add(Items[i].GetCraftingId(), Index | i << 14);
		}

		public override IItem GetItem(ref int value)
		{
			return Terrain.ExtractContents(value) != Index ? base.GetItem(ref value) : Items[Terrain.ExtractData(value)];
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[Items.Length];
			int value = Index;
			for (int i = 0; i < Items.Length; i++)
			{
				arr[i] = value;
				value += 1 << 14;
			}
			return arr;
		}
	}
}