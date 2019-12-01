using Engine;
using System;
using System.Xml.Linq;
namespace Game
{
	public class NewCraftingRecipeWidget : CraftingRecipeWidget
	{
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			
			if (m_dirty)
			{
				//if (!(ScreensManager.CurrentScreen is RecipeAll))
				//{
				//	ScreensManager.SwitchScreen(new RecipaediaScreen());
					//    return;
				//}
				m_dirty = false;
				WidgetsList.Enumerator enumerator;
				if (m_recipe != null)
				{
					m_nameWidget.Text = BlocksManager.Blocks[Terrain.ExtractContents(m_recipe.ResultValue)].GetDisplayName(null, m_recipe.ResultValue) + ((!string.IsNullOrEmpty(NameSuffix)) ? NameSuffix : string.Empty);
					m_descriptionWidget.Text = m_recipe.Description;
					m_nameWidget.IsVisible = true;
					m_descriptionWidget.IsVisible = true;
					enumerator = m_gridWidget.Children.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							var craftingRecipeSlotWidget = (CraftingRecipeSlotWidget)enumerator.Current;
							craftingRecipeSlotWidget.Size = new Vector2(55f);
							Point2 widgetCell = m_gridWidget.GetWidgetCell(craftingRecipeSlotWidget);
							craftingRecipeSlotWidget.SetIngredient(m_recipe.Ingredients[widgetCell.X + widgetCell.Y * 6]);
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
							((CraftingRecipeSlotWidget)enumerator.Current).SetIngredient(null);
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					m_resultWidget.SetResult(0, 0);
				}
			}
			var vector = Vector2.Zero;
			if (Size.X >= 0f)
				parentAvailableSize.X = MathUtils.Min(parentAvailableSize.X, Size.X);
			if (Size.Y >= 0f)
				parentAvailableSize.Y = MathUtils.Min(parentAvailableSize.Y, Size.Y);
			foreach (Widget child in Children)
			{
				if (child.IsVisible)
				{
					Vector2? widgetPosition = GetWidgetPosition(child);
					Vector2 vector2 = widgetPosition ?? Vector2.Zero;
					child.Measure(Vector2.Max(parentAvailableSize - vector2 - 2f * child.Margin, Vector2.Zero));
					var vector3 = default(Vector2);
					vector3.X = MathUtils.Max(vector.X, vector2.X + child.ParentDesiredSize.X + 2f * child.Margin.X);
					vector3.Y = MathUtils.Max(vector.Y, vector2.Y + child.ParentDesiredSize.Y + 2f * child.Margin.Y);
					vector = vector3;
				}
			}
			if (Size.X >= 0f)
				vector.X = Size.X;
			if (Size.Y >= 0f)
				vector.Y = Size.Y;
			DesiredSize = vector;
			
			//m_nameWidget.IsVisible = true;
			//m_descriptionWidget.IsVisible = true;

		}

		
	}

	public class AllRecipeWidget : CanvasWidget
	{
		public LabelWidget m_nameWidget;

		public LabelWidget m_descriptionWidget;

		public GridPanelWidget m_gridWidget;

		//public FireWidget m_fireWidget;

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

		public AllRecipeWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/4MRecipe");
			WidgetsManager.LoadWidgetContents(this, this, node);
			m_nameWidget = Children.Find<LabelWidget>("4MRecipeWidget.Name");
			m_descriptionWidget = Children.Find<LabelWidget>("4MRecipeWidget.Description");
			m_gridWidget = Children.Find<GridPanelWidget>("4MRecipeWidget.Ingredients");
			//m_fireWidget = Children.Find<FireWidget>("SmeltingRecipeWidget.Fire");
			m_resultWidget = Children.Find<CraftingRecipeSlotWidget>("4MRecipeWidget.Result");
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
			if (m_recipe != null)
			{
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(m_recipe.ResultValue)];
				m_nameWidget.Text = block.GetDisplayName(null, m_recipe.ResultValue) + ((!string.IsNullOrEmpty(NameSuffix)) ? NameSuffix : string.Empty);
				m_descriptionWidget.Text = m_recipe.Description;
				m_nameWidget.IsVisible = true;
				m_descriptionWidget.IsVisible = true;
				foreach (CraftingRecipeSlotWidget child in m_gridWidget.Children)
				{
					Point2 widgetCell = m_gridWidget.GetWidgetCell(child);
					child.SetIngredient(m_recipe.Ingredients[widgetCell.X + widgetCell.Y * 6]);
				}
				m_resultWidget.SetResult(m_recipe.ResultValue, m_recipe.ResultCount);
				//m_fireWidget.ParticlesPerSecond = 40f;
			}
			else
			{
				m_nameWidget.IsVisible = false;
				m_descriptionWidget.IsVisible = false;
				foreach (CraftingRecipeSlotWidget child2 in m_gridWidget.Children)
				{
					child2.SetIngredient(null);
				}
				m_resultWidget.SetResult(0, 0);
				//m_fireWidget.ParticlesPerSecond = 0f;
			}
		}
	}
	public class CastRecipeWidget : CanvasWidget
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

		public CastRecipeWidget()
		{
			XElement node = ContentManager.Get<XElement>("Widgets/4MRecipe");
			WidgetsManager.LoadWidgetContents(this, this, node);
			m_nameWidget = Children.Find<LabelWidget>("4MRecipeWidget.Name");
			m_descriptionWidget = Children.Find<LabelWidget>("4MRecipeWidget.Description");
			m_gridWidget = Children.Find<GridPanelWidget>("4MRecipeWidget.Ingredients");
			//m_fireWidget = Children.Find<FireWidget>("SmeltingRecipeWidget.Fire");
			m_resultWidget = Children.Find<CraftingRecipeSlotWidget>("4MRecipeWidget.Result");
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
			if (m_recipe != null)
			{
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(m_recipe.ResultValue)];
				m_nameWidget.Text = block.GetDisplayName(null, m_recipe.ResultValue) + ((!string.IsNullOrEmpty(NameSuffix)) ? NameSuffix : string.Empty);
				m_descriptionWidget.Text = m_recipe.Description;
				m_nameWidget.IsVisible = true;
				m_descriptionWidget.IsVisible = true;
				foreach (CraftingRecipeSlotWidget child in m_gridWidget.Children)
				{
					Point2 widgetCell = m_gridWidget.GetWidgetCell(child);
					child.SetIngredient(m_recipe.Ingredients[widgetCell.X + widgetCell.Y * 6]);
				}
				m_resultWidget.SetResult(m_recipe.ResultValue, m_recipe.ResultCount);
				m_fireWidget.ParticlesPerSecond = 40f;
			}
			else
			{
				m_nameWidget.IsVisible = false;
				m_descriptionWidget.IsVisible = false;
				foreach (CraftingRecipeSlotWidget child2 in m_gridWidget.Children)
				{
					child2.SetIngredient(null);
				}
				m_resultWidget.SetResult(0, 0);
				m_fireWidget.ParticlesPerSecond = 0f;
			}
		}
	}
}