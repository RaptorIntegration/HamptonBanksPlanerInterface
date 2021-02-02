using Mighty;
using System.Collections.Generic;
using System.Linq;

namespace WebSort.Model
{
    public class Recipe
    {
        public int RecipeID { get; set; }
        public string RecipeLabel { get; set; }
        public float TargetVolumePerHour { get; set; }
        public int TargetPiecesPerHour { get; set; }
        public int TargetLugFill { get; set; }
        public int TargetUptime { get; set; }
        public int Online { get; set; }
        public int Editing { get; set; }

        public static Recipe GetEditingRecipe()
        {
            MightyOrm<Recipe> db = new MightyOrm<Recipe>(Global.MightyConString, "Recipes", "RecipeID");
            return db.SingleWithParams("WHERE Editing = 1");
        }

        public static Recipe GetOnlineRecipe()
        {
            MightyOrm<Recipe> db = new MightyOrm<Recipe>(Global.MightyConString, "Recipes", "RecipeID");
            return db.SingleWithParams("WHERE Online = 1");
        }

        public static IEnumerable<Recipe> GetAllData()
        {
            MightyOrm<Recipe> db = new MightyOrm<Recipe>(Global.MightyConString, "Recipes", "RecipeID");
            return db.All().OrderBy(r => r.RecipeLabel);
        }

        public static IEnumerable<Recipe> ChangeEditing(Recipe recipe)
        {
            MightyOrm<Recipe> db = new MightyOrm<Recipe>(Global.MightyConString, "Recipes", "RecipeID");

            foreach (Recipe r in GetAllData())
            {
                r.Editing = 0;
                db.Update(r);
            }

            recipe.Editing = 1;
            db.Update(recipe);

            return GetAllData();
        }

        public static void ChangeActive(Recipe recipe)
        {
            MightyOrm<Recipe> db = new MightyOrm<Recipe>(Global.MightyConString, "Recipes", "RecipeID");

            foreach (Recipe r in GetAllData())
            {
                r.Online = 0;
                db.Update(r);
            }

            recipe.Online = 1;
            db.Update(recipe);
        }
    }
}