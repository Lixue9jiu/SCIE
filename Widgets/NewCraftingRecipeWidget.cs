using Engine;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Game
{
	public class CraftingRecipeSlotWidget : CanvasWidget
	{
		public BlockIconWidget m_blockIconWidget;

		public LabelWidget m_labelWidget;

		public string m_ingredient;

		public int m_resultValue;

		public int m_resultCount;

		public CraftingRecipeSlotWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/CraftingRecipeSlot");
			WidgetsManager.LoadWidgetContents(this, this, node);
			m_blockIconWidget = Children.Find<BlockIconWidget>("CraftingRecipeSlotWidget.Icon", true);
			m_labelWidget = Children.Find<LabelWidget>("CraftingRecipeSlotWidget.Count", true);
		}

		public void SetIngredient(string ingredient)
		{
			m_ingredient = ingredient;
			m_resultValue = 0;
			m_resultCount = 0;
		}

		public void SetResult(int value, int count)
		{
			m_resultValue = value;
			m_resultCount = count;
			m_ingredient = null;
		}

		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			m_blockIconWidget.IsVisible = false;
			m_labelWidget.IsVisible = false;
			if (!string.IsNullOrEmpty(m_ingredient))
			{
				CraftingRecipesManager.DecodeIngredient(m_ingredient, out string craftingId, out int? data);
				if (ItemBlock.IdTable.TryGetValue(craftingId, out int value))
				{
					m_blockIconWidget.Value = value;
					m_blockIconWidget.Light = 15;
					m_blockIconWidget.IsVisible = true;
				}
				else
				{
					var c__DisplayClass = new BlocksManager.c__DisplayClass6
					{
						craftingId = craftingId
					};
					var array = BlocksManager.Blocks.Where(c__DisplayClass.FindBlocksByCraftingId_b__5).ToArray();
					if (array.Length != 0)
					{
						Block block = array[(int)(1.0 * Time.RealTime) % array.Length];
						if (block != null)
						{
							m_blockIconWidget.Value = Terrain.MakeBlockValue(block.BlockIndex, 0, data.HasValue ? data.Value : 4);
							m_blockIconWidget.Light = 15;
							m_blockIconWidget.IsVisible = true;
						}
					}
				}
			}
			else if (m_resultValue != 0)
			{
				m_blockIconWidget.Value = m_resultValue;
				m_blockIconWidget.Light = 15;
				m_labelWidget.Text = m_resultCount.ToString();
				m_blockIconWidget.IsVisible = true;
				m_labelWidget.IsVisible = true;
			}
			base.MeasureOverride(parentAvailableSize);
		}
	}
	public class NewCraftingRecipeWidget : CanvasWidget
	{
		public LabelWidget m_nameWidget;

		public LabelWidget m_descriptionWidget;

		public GridPanelWidget m_gridWidget;

		public CraftingRecipeSlotWidget m_resultWidget;

		public CraftingRecipe m_recipe;

		public string m_nameSuffix;

		public bool m_dirty = true;

		public string NameSuffix
		{
			get
			{
				return m_nameSuffix;
			}
			set
			{
				if (value != m_nameSuffix)
				{
					m_nameSuffix = value;
					m_dirty = true;
				}
			}
		}

		public CraftingRecipe Recipe
		{
			get
			{
				return m_recipe;
			}
			set
			{
				if (value != m_recipe)
				{
					m_recipe = value;
					m_dirty = true;
				}
			}
		}

		public NewCraftingRecipeWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/CraftingRecipe");
			WidgetsManager.LoadWidgetContents(this, this, node);
			m_nameWidget = Children.Find<LabelWidget>("CraftingRecipeWidget.Name", true);
			m_descriptionWidget = Children.Find<LabelWidget>("CraftingRecipeWidget.Description", true);
			m_gridWidget = Children.Find<GridPanelWidget>("CraftingRecipeWidget.Ingredients", true);
			m_resultWidget = Children.Find<CraftingRecipeSlotWidget>("CraftingRecipeWidget.Result", true);
			for (int i = 0; i < m_gridWidget.RowsCount; i++)
			{
				for (int j = 0; j < m_gridWidget.ColumnsCount; j++)
				{
					CraftingRecipeSlotWidget widget = new CraftingRecipeSlotWidget();
					m_gridWidget.Children.Add(widget);
					m_gridWidget.SetWidgetCell(widget, new Point2(j, i));
				}
			}
		}

		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (m_dirty)
			{
				UpdateWidgets();
			}
			base.MeasureOverride(parentAvailableSize);
		}

		public void UpdateWidgets()
		{
			m_dirty = false;
			WidgetsList.Enumerator enumerator;
			if (m_recipe != null)
			{
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(m_recipe.ResultValue)];
				m_nameWidget.Text = block.GetDisplayName(null, m_recipe.ResultValue) + ((!string.IsNullOrEmpty(NameSuffix)) ? NameSuffix : string.Empty);
				m_descriptionWidget.Text = m_recipe.Description;
				m_nameWidget.IsVisible = true;
				m_descriptionWidget.IsVisible = true;
				enumerator = m_gridWidget.Children.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						CraftingRecipeSlotWidget craftingRecipeSlotWidget = (CraftingRecipeSlotWidget)enumerator.Current;
						Point2 widgetCell = m_gridWidget.GetWidgetCell(craftingRecipeSlotWidget);
						craftingRecipeSlotWidget.SetIngredient(m_recipe.Ingredients[widgetCell.X + widgetCell.Y * 3]);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				m_resultWidget.SetResult(m_recipe.ResultValue, m_recipe.ResultCount);
			}
			else
			{
				m_nameWidget.IsVisible = false;
				m_descriptionWidget.IsVisible = false;
				enumerator = m_gridWidget.Children.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						((CraftingRecipeSlotWidget)enumerator.Current).SetIngredient(null);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				m_resultWidget.SetResult(0, 0);
			}
		}
	}
	public class NewSmeltingRecipeWidget : CanvasWidget
	{
		public LabelWidget m_nameWidget;

		public LabelWidget m_descriptionWidget;

		public GridPanelWidget m_gridWidget;

		public FireWidget m_fireWidget;

		public CraftingRecipeSlotWidget m_resultWidget;

		public CraftingRecipe m_recipe;

		public string m_nameSuffix;

		public bool m_dirty = true;

		public string NameSuffix
		{
			get
			{
				return m_nameSuffix;
			}
			set
			{
				if (value != m_nameSuffix)
				{
					m_nameSuffix = value;
					m_dirty = true;
				}
			}
		}

		public CraftingRecipe Recipe
		{
			get
			{
				return m_recipe;
			}
			set
			{
				if (value != m_recipe)
				{
					m_recipe = value;
					m_dirty = true;
				}
			}
		}

		public NewSmeltingRecipeWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/SmeltingRecipe");
			WidgetsManager.LoadWidgetContents(this, this, node);
			m_nameWidget = Children.Find<LabelWidget>("SmeltingRecipeWidget.Name", true);
			m_descriptionWidget = Children.Find<LabelWidget>("SmeltingRecipeWidget.Description", true);
			m_gridWidget = Children.Find<GridPanelWidget>("SmeltingRecipeWidget.Ingredients", true);
			m_fireWidget = Children.Find<FireWidget>("SmeltingRecipeWidget.Fire", true);
			m_resultWidget = Children.Find<CraftingRecipeSlotWidget>("SmeltingRecipeWidget.Result", true);
			for (int i = 0; i < m_gridWidget.RowsCount; i++)
			{
				for (int j = 0; j < m_gridWidget.ColumnsCount; j++)
				{
					CraftingRecipeSlotWidget widget = new CraftingRecipeSlotWidget();
					m_gridWidget.Children.Add(widget);
					m_gridWidget.SetWidgetCell(widget, new Point2(j, i));
				}
			}
		}

		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (m_dirty)
			{
				UpdateWidgets();
			}
			base.MeasureOverride(parentAvailableSize);
		}

		public void UpdateWidgets()
		{
			m_dirty = false;
			WidgetsList.Enumerator enumerator;
			if (m_recipe != null)
			{
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(m_recipe.ResultValue)];
				m_nameWidget.Text = block.GetDisplayName(null, m_recipe.ResultValue) + ((!string.IsNullOrEmpty(NameSuffix)) ? NameSuffix : string.Empty);
				m_descriptionWidget.Text = m_recipe.Description;
				m_nameWidget.IsVisible = true;
				m_descriptionWidget.IsVisible = true;
				enumerator = m_gridWidget.Children.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						CraftingRecipeSlotWidget craftingRecipeSlotWidget = (CraftingRecipeSlotWidget)enumerator.Current;
						Point2 widgetCell = m_gridWidget.GetWidgetCell(craftingRecipeSlotWidget);
						craftingRecipeSlotWidget.SetIngredient(m_recipe.Ingredients[widgetCell.X + widgetCell.Y * 3]);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				m_resultWidget.SetResult(m_recipe.ResultValue, m_recipe.ResultCount);
				m_fireWidget.ParticlesPerSecond = 40f;
			}
			else
			{
				m_nameWidget.IsVisible = false;
				m_descriptionWidget.IsVisible = false;
				enumerator = m_gridWidget.Children.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						((CraftingRecipeSlotWidget)enumerator.Current).SetIngredient(null);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				m_resultWidget.SetResult(0, 0);
				m_fireWidget.ParticlesPerSecond = 0f;
			}
		}
	}
}