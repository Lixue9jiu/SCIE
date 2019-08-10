using Engine;
using System;

namespace Game
{
	public class NewCraftingRecipeWidget : CraftingRecipeWidget
	{
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (m_dirty)
			{
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
		}
	}
}