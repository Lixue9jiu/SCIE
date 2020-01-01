using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Game;
using System.Xml.Linq;
namespace Game
{
	public class RecipeAll : RecipaediaRecipesScreen
	{
		public AllRecipeWidget m_AllRecipeWidget;
		public CastRecipeWidget m_CastRecipeWidget;
		public BFRecipeWidget m_BFRecipeWidget;
		public CFRecipeWidget m_CFRecipeWidget;
		public SRecipeWidget m_MRecipeWidget;
		public RecipeAll() : base()
		{
			//XElement node = ContentManager.Get<XElement>("Screens/RecipaediaRecipesScreen2");
			//WidgetsManager.LoadWidgetContents(this, this, node);
			//m_craftingRecipeWidget = Children.Find<NewCraftingRecipeWidget>("CraftingRecipe");
			//m_smeltingRecipeWidget = Children.Find<SmeltingRecipeWidget>("SmeltingRecipe");
			m_AllRecipeWidget = Children.Find<AllRecipeWidget>("AllRecipe");
			m_CastRecipeWidget = Children.Find<CastRecipeWidget>("CastRecipe");
			m_CFRecipeWidget = Children.Find<CFRecipeWidget>("CFRecipe");
			m_BFRecipeWidget = Children.Find<BFRecipeWidget>("BFRecipe");
			m_MRecipeWidget = Children.Find<SRecipeWidget>("SRecipe");
			//m_prevRecipeButton = Children.Find<ButtonWidget>("PreviousRecipe");
			//m_nextRecipeButton = Children.Find<ButtonWidget>("NextRecipe");
		}

		public override void Enter(object[] parameters)
		{
			c__DisplayClass7_0 c__DisplayClass7_ = new c__DisplayClass7_0();
			c__DisplayClass7_.value = (int)parameters[0];
			m_craftingRecipes.Clear();
			m_craftingRecipes.AddRange(CraftingRecipesManager.Recipes.Where(c__DisplayClass7_.Enter_b__0));
			m_recipeIndex = 0;
		}
		public override void Update()
		{
			if (m_recipeIndex < m_craftingRecipes.Count)
			{
				CraftingRecipe craftingRecipe = m_craftingRecipes[m_recipeIndex];
				if (craftingRecipe.RequiredHeatLevel == 0f)
				{
					m_craftingRecipeWidget.Recipe = craftingRecipe;
					m_craftingRecipeWidget.NameSuffix = $" (recipe #{m_recipeIndex + 1})";
					m_craftingRecipeWidget.IsVisible = true;
					m_smeltingRecipeWidget.IsVisible = false;
					m_AllRecipeWidget.IsVisible = false;
					m_CastRecipeWidget.IsVisible = false;
					m_CFRecipeWidget.IsVisible = false;
					m_BFRecipeWidget.IsVisible = false;
					m_MRecipeWidget.IsVisible = false;
				}
				else if (craftingRecipe.RequiredHeatLevel == -1f)
				{
					m_AllRecipeWidget.Recipe = craftingRecipe;
					m_AllRecipeWidget.NameSuffix = $" (recipe #{m_recipeIndex + 1})";
					m_craftingRecipeWidget.IsVisible = false;
					m_smeltingRecipeWidget.IsVisible = false;
					m_AllRecipeWidget.IsVisible = true;
					m_CastRecipeWidget.IsVisible = false;
					m_CFRecipeWidget.IsVisible = false;
					m_BFRecipeWidget.IsVisible = false;
					m_MRecipeWidget.IsVisible = false;
				}
				else if (craftingRecipe.RequiredHeatLevel == -2f)
				{
					m_CastRecipeWidget.Recipe = craftingRecipe;
					m_AllRecipeWidget.NameSuffix = $" (recipe #{m_recipeIndex + 1})";
					m_craftingRecipeWidget.IsVisible = false;
					m_smeltingRecipeWidget.IsVisible = false;
					m_CastRecipeWidget.IsVisible = true;
					m_AllRecipeWidget.IsVisible = false;
					m_CFRecipeWidget.IsVisible = false;
					m_BFRecipeWidget.IsVisible = false;
					m_MRecipeWidget.IsVisible = false;
					//m_CastRecipeWidget.IsVisible = false;
				}
				else if (craftingRecipe.RequiredHeatLevel == -3f)
				{
					m_BFRecipeWidget.Recipe = craftingRecipe;
					m_AllRecipeWidget.NameSuffix = $" (recipe #{m_recipeIndex + 1})";
					m_craftingRecipeWidget.IsVisible = false;
					m_smeltingRecipeWidget.IsVisible = false;
					m_CastRecipeWidget.IsVisible = false;
					m_AllRecipeWidget.IsVisible = false;
					m_CFRecipeWidget.IsVisible = false;
					m_BFRecipeWidget.IsVisible = true;
					m_MRecipeWidget.IsVisible = false;
					//m_CastRecipeWidget.IsVisible = false;
				}
				else if (craftingRecipe.RequiredHeatLevel == -4f)
				{
					m_CFRecipeWidget.Recipe = craftingRecipe;
					m_AllRecipeWidget.NameSuffix = $" (recipe #{m_recipeIndex + 1})";
					m_craftingRecipeWidget.IsVisible = false;
					m_smeltingRecipeWidget.IsVisible = false;
					m_CastRecipeWidget.IsVisible = false;
					m_AllRecipeWidget.IsVisible = false;
					m_CFRecipeWidget.IsVisible = true;
					m_BFRecipeWidget.IsVisible = false;
					m_MRecipeWidget.IsVisible = false;
					//m_CastRecipeWidget.IsVisible = false;
				}
				else if (craftingRecipe.RequiredHeatLevel == -5f)
				{
					m_MRecipeWidget.Recipe = craftingRecipe;
					m_AllRecipeWidget.NameSuffix = $" (recipe #{m_recipeIndex + 1})";
					m_craftingRecipeWidget.IsVisible = false;
					m_smeltingRecipeWidget.IsVisible = false;
					m_CastRecipeWidget.IsVisible = false;
					m_AllRecipeWidget.IsVisible = false;
					m_CFRecipeWidget.IsVisible = false;
					m_BFRecipeWidget.IsVisible = false;
					m_MRecipeWidget.IsVisible = true;
					//m_CastRecipeWidget.IsVisible = false;
				}
				else
				{
					m_smeltingRecipeWidget.Recipe = craftingRecipe;
					m_smeltingRecipeWidget.NameSuffix = $" (recipe #{m_recipeIndex + 1})";
					m_smeltingRecipeWidget.IsVisible = true;
					m_craftingRecipeWidget.IsVisible = false;
					m_AllRecipeWidget.IsVisible = false;
					m_CastRecipeWidget.IsVisible = false;
					m_CFRecipeWidget.IsVisible = false;
					m_BFRecipeWidget.IsVisible = false;
					m_MRecipeWidget.IsVisible = false;
				}
			}
			m_prevRecipeButton.IsEnabled = (m_recipeIndex > 0);
			m_nextRecipeButton.IsEnabled = (m_recipeIndex < m_craftingRecipes.Count - 1);
			if (m_prevRecipeButton.IsClicked)
			{
				m_recipeIndex = MathUtils.Max(m_recipeIndex - 1, 0);
			}
			if (m_nextRecipeButton.IsClicked)
			{
				m_recipeIndex = MathUtils.Min(m_recipeIndex + 1, m_craftingRecipes.Count - 1);
			}
			if (base.Input.Back || base.Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back").IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
			}
		}

	}

	public class RecipaediaScreen2 : RecipaediaScreen
	{

		public override void Enter(object[] parameters)
		{
			if (!(ScreensManager.PreviousScreen is RecipeAll) && ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("RecipaediaDescription"))
			{
				m_previousScreen = ScreensManager.PreviousScreen;
			}
		}
		public override void Update()
		{
			c__DisplayClass12_0 c__DisplayClass12_ = new c__DisplayClass12_0();
			if (m_listCategoryIndex != m_categoryIndex)
			{
				PopulateBlocksList();
			}
			string text = m_categories[m_categoryIndex] ?? "All Blocks";
			m_categoryLabel.Text = string.Format("{0} ({1})", new object[2]
			{
			text,
			m_blocksList.Items.Count
			});
			m_prevCategoryButton.IsEnabled = (m_categoryIndex > 0);
			m_nextCategoryButton.IsEnabled = (m_categoryIndex < m_categories.Count - 1);
			c__DisplayClass12_.value = null;
			int num = 0;
			if (m_blocksList.SelectedItem is int)
			{
				c__DisplayClass12_.value = (int)m_blocksList.SelectedItem;
				num = CraftingRecipesManager.Recipes.Count(c__DisplayClass12_.Update_b__0);
			}
			if (num > 0)
			{
				m_recipesButton.Text = string.Format("{0} {1}", new object[2]
				{
				num,
				(num == 1) ? "recipe" : "recipes"
				});
				m_recipesButton.IsEnabled = true;
			}
			else
			{
				m_recipesButton.Text = "No recipes";
				m_recipesButton.IsEnabled = false;
			}
			m_detailsButton.IsEnabled = c__DisplayClass12_.value.HasValue;
			if (m_prevCategoryButton.IsClicked || base.Input.Left)
			{
				m_categoryIndex = MathUtils.Max(m_categoryIndex - 1, 0);
			}
			if (m_nextCategoryButton.IsClicked || base.Input.Right)
			{
				m_categoryIndex = MathUtils.Min(m_categoryIndex + 1, m_categories.Count - 1);
			}
			if (c__DisplayClass12_.value.HasValue && m_detailsButton.IsClicked)
			{
				ScreensManager.SwitchScreen("RecipaediaDescription", c__DisplayClass12_.value.Value, m_blocksList.Items.Cast<int>().ToList());
			}
			if (c__DisplayClass12_.value.HasValue && m_recipesButton.IsClicked)
			{
				ScreensManager.SwitchScreen(new RecipeAll(), c__DisplayClass12_.value.Value);
			}//new RecipeAll()
			if (base.Input.Back || base.Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back").IsClicked)
			{
				ScreensManager.SwitchScreen(m_previousScreen);
			}
		}
	}
}
