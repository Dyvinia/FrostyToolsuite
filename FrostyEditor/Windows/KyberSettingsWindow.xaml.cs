using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Linq;
using Frosty.Core;
using Frosty.Controls;
using FrostySdk.Managers;
using FrostySdk.Ebx;

namespace FrostyEditor.Windows
{
    public static class KyberSettings
    {
        public static string CliDirectory { 
            get => Config.Get("Kyber_CliDirectory", "", ConfigScope.Global); 
            set => Config.Add("Kyber_CliDirectory", value, ConfigScope.Global); 
        }
        public static string GameMode { 
            get => Config.Get("Kyber_SelectedMode", "Mode1", ConfigScope.Global); 
            set => Config.Add("Kyber_SelectedMode", value, ConfigScope.Global); 
        }
        public static string Level { 
            get => Config.Get("Kyber_SelectedLevel", "S6_2/Geonosis_02/Levels/Geonosis_02/Geonosis_02", ConfigScope.Global); 
            set => Config.Add("Kyber_SelectedLevel", value, ConfigScope.Global); 
        }
        public static int TeamId { 
            get => Config.Get("Kyber_TeamId", 1, ConfigScope.Global); 
            set => Config.Add("Kyber_TeamId", value, ConfigScope.Global); 
        }
        public static string AutoplayerType { 
            get => Config.Get("Kyber_AutoplayerType", "Gamemode Tied", ConfigScope.Global); 
            set => Config.Add("Kyber_AutoplayerType", value, ConfigScope.Global); 
        }
        public static int Team1Bots { 
            get => Config.Get("Kyber_Team1Bots", 20, ConfigScope.Global); 
            set => Config.Add("Kyber_Team1Bots", value, ConfigScope.Global); 
        }
        public static int Team2Bots { 
            get => Config.Get("Kyber_Team2Bots", 20, ConfigScope.Global); 
            set => Config.Add("Kyber_Team2Bots", value, ConfigScope.Global); 
        }
        public static string SelectedLoadOrder { 
            get => Config.Get("Kyber_SelectedLoadOrder", "No Order", ConfigScope.Global); 
            set => Config.Add("Kyber_SelectedLoadOrder", value, ConfigScope.Global); 
        }
        public static bool Autostart { 
            get => Config.Get("Kyber_AutoStart", false, ConfigScope.Global); 
            set => Config.Add("Kyber_AutoStart", value, ConfigScope.Global); 
        }
        public static List<string> LaunchCommands {
            get => Config.Get<string>("Kyber_LaunchCommands", null, ConfigScope.Global)?.Split('$').ToList() ?? [];
            set => Config.Add("Kyber_LaunchCommands", string.Join("$", value.ToList()), ConfigScope.Global);
        }
    }

    public class KyberJsonSettings
    {
        //public List<KyberGamemodeJsonSettings> GamemodeOverrides { get; set; }
        public List<KyberLevelJsonSettings> LevelOverrides { get; set; }
        public List<KyberLoadOrderJsonSettings> LoadOrders { get; set; }
        public KyberJsonSettings()
        {

        }
    }
    public class KyberGamemodeJsonSettings
    {
        public string Name { get; set; }
        public string ModeId { get; set; }
        public int PlayerCount { get; set; }
        public KyberGamemodeJsonSettings()
        {

        }
    }
    public class KyberLevelJsonSettings
    {
        public string Name { get; set; }
        public string LevelId { get; set; }
        public List<string> ModeIds { get; set; }
        public KyberLevelJsonSettings()
        {

        }
    }
    public class KyberLoadOrderJsonSettings
    {
        public string Name { get; set; }
        public List<string> FbmodNames { get; set; }
        public KyberLoadOrderJsonSettings()
        {

        }
    }


    /// <summary>
    /// Interaction logic for KyberSettingsWindow.xaml
    /// </summary>
    public partial class KyberSettingsWindow : FrostyDockableWindow
    {
        private List<(string, string, List<(string, string)>, int)> gameModesData = [];
        private readonly KyberJsonSettings jsonSettings = new();

        public KyberSettingsWindow(KyberJsonSettings jsonSettings)
        {
            this.jsonSettings = jsonSettings;
            InitializeComponent();

            Loaded += ModSettingsWindow_Loaded;

            autoplayerTypeComboBox.SelectionChanged += (_, _) => UpdateAutoplayerCounts();

            copyArgsButton.Click += (_, _) => { 
                try
                {
                    Clipboard.SetText(GetArgs());
                }
                catch
                {

                }
            };
        }

        private string GetArgs()
        {
            string basePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", @"/")}/Mods/Kyber";
            try
            {
                return $"\"{kyberCliTextBox.Text}\" start_server --verbose --server-password \"amogus\" --debug --no-dedicated --server-name \"Test\" --map \"{gameModesData[gamemodeComboBox.SelectedIndex].Item3[levelComboBox.SelectedIndex].Item1}\" --mode \"{gameModesData[gamemodeComboBox.SelectedIndex].Item1}\" --raw-mods \"{$@"{basePath}/Kyber-Launch.json"}\" --startup-commands \"{$@"{basePath}/Kyber-Commands.txt"}\"";
            }
            catch
            {
                return $"\"{kyberCliTextBox.Text}\" start_server --verbose --server-password \"amogus\" --debug --no-dedicated --server-name \"Test\" --map \"{KyberSettings.Level}\" --mode \"{KyberSettings.GameMode}\" --raw-mods \"{$@"{basePath}/Kyber-Launch.json"}\" --startup-commands \"{$@"{basePath}/Kyber-Commands.txt"}\"";
            }
        }

        private void UpdateAutoplayerCounts()
        {
            if (team1AutoplayerCountComboBox.SelectedIndex != -1)
                KyberSettings.Team1Bots = team1AutoplayerCountComboBox.SelectedIndex;
            if (team2AutoplayerCountComboBox.SelectedIndex != -1)
                KyberSettings.Team2Bots = team2AutoplayerCountComboBox.SelectedIndex;

            if (autoplayerTypeComboBox.SelectedIndex == 1)
            {
                Team1APLabel.Content = "Count";
                Team1APLabel.Width = 42;
                Team2APColumn.Width = new(0);

                team1AutoplayerCountComboBox.Items.Clear();
                for (int i = 0; i <= 64; i++)
                {
                    team1AutoplayerCountComboBox.Items.Add(i);
                }
                team1AutoplayerCountComboBox.SelectedIndex = KyberSettings.Team1Bots + KyberSettings.Team2Bots;
            }
            else
            {
                Team1APLabel.Content = "Team 1";
                Team1APLabel.Width = double.NaN;
                Team2APColumn.Width = new(1, GridUnitType.Star);

                team1AutoplayerCountComboBox.Items.Clear();
                for (int i = 0; i <= 32; i++)
                {
                    team1AutoplayerCountComboBox.Items.Add(i);
                }
                team1AutoplayerCountComboBox.SelectedIndex = KyberSettings.Team1Bots / 2 + KyberSettings.Team1Bots % 2;
                team2AutoplayerCountComboBox.SelectedIndex = KyberSettings.Team1Bots / 2;
            }

            if (team1AutoplayerCountComboBox.SelectedIndex != -1)
                KyberSettings.Team1Bots = team1AutoplayerCountComboBox.SelectedIndex;
            if (team2AutoplayerCountComboBox.SelectedIndex != -1)
                KyberSettings.Team2Bots = team2AutoplayerCountComboBox.SelectedIndex;
        }

        private string ConvertToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Convert the string to lowercase
            input = input.ToLower();

            // Capitalize the first letter of each word
            string[] words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    char[] letters = words[i].ToCharArray();
                    letters[0] = char.ToUpper(letters[0]);
                    words[i] = new string(letters);
                }
            }

            return string.Join(" ", words);
        }

        private void RepopulateLevelComboBox(int oldPlayerCount)
        {
            string curLevel = levelComboBox.Text;
            levelComboBox.Items.Clear();
            foreach ((string, string) pair in gameModesData[gamemodeComboBox.SelectedIndex].Item3)
            {
                levelComboBox.Items.Add(pair.Item2);
                if (pair.Item2 == curLevel)
                    levelComboBox.SelectedIndex = levelComboBox.Items.Count - 1;
                else if (levelComboBox.Items.Count == 1)
                    levelComboBox.SelectedIndex = 0;
            }

            if ((Convert.ToInt32(Math.Floor((float)oldPlayerCount / 2) * 2) == team1AutoplayerCountComboBox.SelectedIndex + team2AutoplayerCountComboBox.SelectedIndex))
            {
                int newPlayerCount = gameModesData[gamemodeComboBox.SelectedIndex].Item4;
                int perTeamCount = Convert.ToInt32(Math.Floor((float)newPlayerCount / 2));
                team1AutoplayerCountComboBox.SelectedIndex = perTeamCount;
                team2AutoplayerCountComboBox.SelectedIndex = perTeamCount;
            }
            //else
            //{
            //    App.Logger.Log(oldPlayerCount.ToString());
            //    App.Logger.Log((team1AutoplayerCountComboBox.SelectedIndex + team2AutoplayerCountComboBox.SelectedIndex).ToString());
            //}
        }

        private void ModSettingsWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            teamIdComboBox.Items.Add("TeamNeutral");
            for (int i = 0; i < 16; i++)
                teamIdComboBox.Items.Add($"Team{i + 1}");
            for (int i = 0; i <= 32; i++)
            {
                team1AutoplayerCountComboBox.Items.Add(i);
                team2AutoplayerCountComboBox.Items.Add(i);
            }
            //loadOrderComboBox.Items.Add("No Order");
            //jsonSettings.LoadOrders.ForEach(loadOrder => loadOrderComboBox.Items.Add(loadOrder.Name));

            autoplayerTypeComboBox.Items.Add("No Bots");
            autoplayerTypeComboBox.Items.Add("Dummy Bots");
            autoplayerTypeComboBox.Items.Add("Gamemode Tied");

            autoStartComboBox.Items.Add("Disabled");
            autoStartComboBox.Items.Add("Enabled");

            //Fill Gamemodes/Levels
            EbxAssetEntry modesEntry = App.AssetManager.GetEbxEntry("UI/Data/GameModes/GameModes");
            dynamic modesRoot = App.AssetManager.GetEbx(modesEntry).RootObject;

            List<KyberGamemodeJsonSettings> baseModes =
                [
                    new() { Name = "Galactic Assault", ModeId = "PlanetaryBattles", PlayerCount = 40},
                    new() { Name = "Supremacy", ModeId = "Mode1", PlayerCount = 64},
                    new() { Name = "COOP Attack", ModeId = "Mode9", PlayerCount = 20},
                    new() { Name = "COOP Defend", ModeId = "ModeDefend", PlayerCount = 20},
                    new() { Name = "Ewok Hunt", ModeId = "Mode3", PlayerCount = 0},
                    new() { Name = "Extraction", ModeId = "Mode5", PlayerCount = 16},
                    new() { Name = "Hero Showdown", ModeId = "Mode6", PlayerCount = 4},
                    new() { Name = "Starfighter HvsV", ModeId = "Mode7", PlayerCount = 6},
                    new() { Name = "Jetpack Cargo", ModeId = "ModeC", PlayerCount = 16},
                    new() { Name = "Strike", ModeId = "PlanetaryMissions", PlayerCount = 16},
                    new() { Name = "Blast", ModeId = "Blast", PlayerCount = 16},
                    new() { Name = "Heroes Vs Villains", ModeId = "HeroesVersusVillains", PlayerCount = 6},
                    new() { Name = "Starfighter Assault", ModeId = "SpaceBattle", PlayerCount = 24}
                ];

            foreach (PointerRef modePr in modesRoot.GameModes)
            {
                EbxAssetEntry modeEntry = App.AssetManager.GetEbxEntry(modePr.External.FileGuid);
                if (modeEntry != null)
                {
                    dynamic modeRoot = App.AssetManager.GetEbx(modeEntry).RootObject;
                    string modeName = $"{ConvertToTitleCase(modeRoot.AurebeshGameModeName)} [{modeRoot.GameModeId}]";

                    List<(string, string)> levelPairs = [];
                    foreach (PointerRef levelPr in modeRoot.Levels)
                    {
                        EbxAssetEntry levEntry = App.AssetManager.GetEbxEntry(levelPr.External.FileGuid);
                        if (levEntry != null)
                        {
                            dynamic levRoot = App.AssetManager.GetEbx(levEntry).RootObject;
                            string aurabesh = ConvertToTitleCase(levRoot.LevelAurebesh.Internal.String);
                            if (aurabesh.StartsWith("Yavin") && aurabesh.EndsWith("4"))
                                aurabesh = "Yavin IV";
                            else if (aurabesh == "Death Star Ii")
                                aurabesh = "Death Star II";

                            string levName = $"{aurabesh} - {ConvertToTitleCase(LocalizedStringDatabase.Current.GetString((uint)levRoot.LevelName.Internal.StringHash))}";

                            jsonSettings.LevelOverrides.Where(levelOverride => levelOverride.LevelId == levRoot.LevelId).ToList().ForEach(levelOverride => levName = levelOverride.Name);
                            levelPairs.Add((levRoot.LevelId, levName));
                        }
                    }
                    foreach (KyberLevelJsonSettings levelJsonSettings in jsonSettings.LevelOverrides.Where(levelOverride => levelOverride.ModeIds.Contains(modeRoot.GameModeId)).ToList())
                    {
                        if (levelPairs.Select(pair => pair.Item1).Contains(levelJsonSettings.LevelId))
                            continue;
                        levelPairs.Add((levelJsonSettings.LevelId, levelJsonSettings.Name));
                    }

                    levelPairs = [.. levelPairs.OrderBy(item => item.Item2)];
                    List<string> duplicates = levelPairs.Select(item => item.Item2).GroupBy(x => x).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
                    for (int i = 0; i < levelPairs.Count; i++)
                    {
                        (string, string) pair = levelPairs[i];
                        if (duplicates.Contains(pair.Item2))
                            levelPairs[i] = (pair.Item1, $"{pair.Item2} [{pair.Item1}]");
                    }
                    int playerCount = modeRoot.NumberOfPlayers;
                    foreach (KyberGamemodeJsonSettings jsonGamemode in baseModes.Where(gamemodeInfo => gamemodeInfo.ModeId == modeRoot.GameModeId))
                    {
                        modeName = $"{jsonGamemode.Name} [{modeRoot.GameModeId}]";
                        playerCount = jsonGamemode.PlayerCount;
                    }
                    if (modeName.StartsWith("DO NOT USE"))
                        continue;
                    gameModesData.Add((modeRoot.GameModeId, modeName, levelPairs, playerCount));
                }
            }
            foreach (KyberGamemodeJsonSettings jsonGamemode in baseModes.Where(gamemodeInfo => !gameModesData.Select(modeData => modeData.Item1).Contains(gamemodeInfo.ModeId)))
            {
                List<(string, string)> levelPairs = [];
                foreach (KyberLevelJsonSettings levelJsonSettings in jsonSettings.LevelOverrides.Where(levelOverride => levelOverride.ModeIds.Contains(jsonGamemode.ModeId)).ToList())
                {
                    if (levelPairs.Select(pair => pair.Item1).Contains(levelJsonSettings.LevelId))
                        continue;
                    levelPairs.Add((levelJsonSettings.LevelId, levelJsonSettings.Name));
                }
                if (jsonGamemode.Name.StartsWith("DO NOT USE"))
                    continue;
                gameModesData.Add((jsonGamemode.ModeId, jsonGamemode.Name, levelPairs, jsonGamemode.PlayerCount));
            }


            gameModesData = [.. gameModesData.OrderBy(data => data.Item2)];
            //gameModesData.Insert(0, ("NOGAMEMODE", "Main Menu \t[FRONTEND]", new List<(string, string)>() { ("win32/Levels/Frontend/Frontend", "Frontend") }, 0));
            foreach (var item in gameModesData)
            {
                gamemodeComboBox.Items.Add(item.Item2);
            }
            //gamemodeComboBox.SelectedIndex = 0;

            //Set selected index
            kyberCliTextBox.Text = KyberSettings.CliDirectory;
            //loadOrderComboBox.SelectedIndex = loadOrderComboBox.Items.Contains(KyberSettings.SelectedLoadOrder) ? loadOrderComboBox.Items.IndexOf(KyberSettings.SelectedLoadOrder) : 0;
            gamemodeComboBox.SelectedIndex = gameModesData.Select(data => data.Item1).Contains(KyberSettings.GameMode) ? gameModesData.Select(data => data.Item1).ToList().IndexOf(KyberSettings.GameMode) : 0;
            RepopulateLevelComboBox(0);
            levelComboBox.SelectedIndex = gameModesData[gamemodeComboBox.SelectedIndex].Item3.Select(data => data.Item1).Contains(KyberSettings.Level) ? gameModesData[gamemodeComboBox.SelectedIndex].Item3.Select(data => data.Item1).ToList().IndexOf(KyberSettings.Level) : 0;
            autoStartComboBox.SelectedIndex = KyberSettings.Autostart ? 1 : 0;
            teamIdComboBox.SelectedIndex = KyberSettings.TeamId;
            team1AutoplayerCountComboBox.SelectedIndex = KyberSettings.Team1Bots;
            team2AutoplayerCountComboBox.SelectedIndex = KyberSettings.Team2Bots;
            autoplayerTypeComboBox.SelectedIndex = autoplayerTypeComboBox.Items.Contains(KyberSettings.AutoplayerType) ? autoplayerTypeComboBox.Items.IndexOf(KyberSettings.AutoplayerType) : 0;
            launchCommandTextBox.Text = string.Join(", ", KyberSettings.LaunchCommands);

            lastGamemodeIndex = gamemodeComboBox.SelectedIndex;
        }
        private int lastGamemodeIndex = -1;

        private void modCategoryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lastGamemodeIndex != -1)
            {
                RepopulateLevelComboBox(gameModesData[lastGamemodeIndex].Item4);
                lastGamemodeIndex = gamemodeComboBox.SelectedIndex;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            KyberSettings.CliDirectory = kyberCliTextBox.Text;
            //KyberSettings.SelectedLoadOrder = loadOrderComboBox.Text;
            KyberSettings.GameMode = gameModesData[gamemodeComboBox.SelectedIndex].Item1;
            KyberSettings.Level = gameModesData[gamemodeComboBox.SelectedIndex].Item3[levelComboBox.SelectedIndex].Item1;
            KyberSettings.Autostart = autoStartComboBox.Text == "Enabled";
            KyberSettings.TeamId = teamIdComboBox.SelectedIndex;
            KyberSettings.AutoplayerType = autoplayerTypeComboBox.Text;
            KyberSettings.Team1Bots = team1AutoplayerCountComboBox.SelectedIndex;
            KyberSettings.Team2Bots = team2AutoplayerCountComboBox.SelectedIndex;
            KyberSettings.LaunchCommands = launchCommandTextBox.Text.Split('\n', ',').Select(s => s.Trim()).ToList();
            Config.Save();

            DialogResult = true;
            Close();
        }
    }
}
