/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Omegasis.SaveBackup.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Omegasis.SaveBackup
{
    /// <summary>The mod entry point.</summary>
    public class SaveBackup : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The folder path containing the game's app data.</summary>
        private static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley");

        /// <summary>The folder path containing the game's saves.</summary>
        private static readonly string SavesPath = Path.Combine(SaveBackup.AppDataPath, "Saves");

        /// <summary>The folder path containing backups of the save before the player starts playing.</summary>
        private static readonly string PrePlayBackupsPath = Path.Combine(SaveBackup.AppDataPath, "Backed_Up_Saves", "Pre_Play_Saves");

        /// <summary>The folder path containing nightly backups of the save.</summary>
        private static readonly string NightlyBackupsPath = Path.Combine(SaveBackup.AppDataPath, "Backed_Up_Saves", "Nightly_InGame_Saves");

        /// <summary>The mod configuration.</summary>
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            this.BackupSaves(SaveBackup.PrePlayBackupsPath);

            helper.Events.GameLoop.Saving += this.OnSaving;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            this.BackupSaves(SaveBackup.NightlyBackupsPath);
        }

        /// <summary>Back up saves to the specified folder.</summary>
        /// <param name="folderPath">The folder path in which to generate saves.</param>
        private void BackupSaves(string folderPath)
        {
            // back up saves
            Directory.CreateDirectory(folderPath);
            ZipFile.CreateFromDirectory(SaveBackup.SavesPath, Path.Combine(folderPath, $"backup-{DateTime.Now:yyyyMMdd'-'HHmmss}.zip"));

            // delete old backups
            new DirectoryInfo(folderPath)
                .EnumerateFiles()
                .OrderByDescending(f => f.CreationTime)
                .Skip(this.Config.SaveCount)
                .ToList()
                .ForEach(file => file.Delete());
        }
    }
}
