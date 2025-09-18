using Satchel.BetterMenus;

namespace MarkothShield
{
    public static class ModMenu
    {
        private static Menu menu;
        private static MenuScreen menuScreen;

        /// <summary>
        /// Builds the Exaltation Expanded menu
        /// </summary>
        /// <param name="modListMenu"></param>
        /// <returns></returns>
        public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
        {
            // Declare the menu
            menu = new Menu("Markoth Shield Options", new Element[] { });

            menu.AddElement(new HorizontalOption("Difficulty Level",
                    "Requirement to unlock Markoth Shield",
                    DifficultyLevels(),
                    value => MarkothShield.Instance.globalSettings.difficultyLevel = value,
                    () => MarkothShield.Instance.globalSettings.difficultyLevel));

            // Insert the menu into the overall menu
            menuScreen = menu.GetMenuScreen(modListMenu);

            return menuScreen;
        }

        /// <summary>
        /// String values for int options
        /// </summary>
        /// <returns></returns>
        private static string[] DifficultyLevels()
        {
            return new string[] { "Dreamshield", "Markoth", "Attuned Markoth", "Ascended Markoth", "Radiant Markoth" };
        }
    }
}