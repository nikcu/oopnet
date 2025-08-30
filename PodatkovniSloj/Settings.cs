using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using Utils;

namespace DataLayer
{
    public class Settings : Singleton<Settings>
    {

        public static string[] Championship = ["m", "f"];

        private const string SelectedChampionshipDefault = "m";

        public string SelectedChampionship { get; set; } = SelectedChampionshipDefault;

        private bool IsLoadedFromFile = false;

        private Settings() {
            LoadSettings();
        }

        private void LoadSettings()
        {
            if(!File.Exists(Constant.pathSettings))
            {
                return;
            }

            using StreamReader reader = new(Constant.pathSettings);
            string json = reader.ReadToEnd();

            JsonNode settingsNode = JsonNode.Parse(json)!;

            SelectedChampionship = settingsNode[nameof(SelectedChampionship)]?.GetValue<string>() ?? SelectedChampionshipDefault;

            IsLoadedFromFile = true;
        }

        public bool GetIsLoadedFromFile()
        {
            return IsLoadedFromFile;
        }

        public async void SaveSettings()
        {
            await using FileStream createStream = File.Create(Constant.pathSettings);
            await JsonSerializer.SerializeAsync(createStream, this);
        }
    }
}
