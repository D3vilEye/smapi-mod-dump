/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Serialization
{
    /// <summary>
    /// Encapsulates SMAPI's JSON file parsing.
    /// </summary>
    /// <remarks>Copied from https://github.com/Pathoschild/SMAPI/blob/develop/src/SMAPI.Toolkit/Serialisation/JsonHelper.cs </remarks>
    internal class JsonHelper
    {
        /// <summary>The JSON settings to use when serialising and deserialising files.</summary>
        public JsonSerializerSettings JsonSettings { get; } = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Replace, // avoid issue where default ICollection<T> values are duplicated each time the config is loaded
        };

        /// <summary>Read a JSON file.</summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <param name="fullPath">The absolete file path.</param>
        /// <param name="result">The parsed content model.</param>
        /// <returns>Returns false if the file doesn't exist, else true.</returns>
        /// <exception cref="ArgumentException">The given <paramref name="fullPath"/> is empty or invalid.</exception>
        /// <exception cref="JsonReaderException">The file contains invalid JSON.</exception>
        public bool ReadJsonFileIfExists<TModel>(string fullPath, out TModel result)
        {
            // validate
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The file path is empty or invalid.", nameof(fullPath));

            // read file
            string json;
            try
            {
                json = File.ReadAllText(fullPath);
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is FileNotFoundException)
            {
                result = default(TModel);
                return false;
            }

            // deserialise model
            try
            {
                result = this.Deserialise<TModel>(json);
                return true;
            }
            catch (Exception ex)
            {
                string error = $"Can't parse JSON file at {fullPath}.";

                if (ex is JsonReaderException)
                {
                    error += " This doesn't seem to be valid JSON.";
                    if (json.Contains("“") || json.Contains("”"))
                        error += " Found curly quotes in the text; note that only straight quotes are allowed in JSON.";
                }
                error += $"\nTechnical details: {ex.Message}";
                throw new JsonReaderException(error);
            }
        }

        /// <summary>Save to a JSON file.</summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <param name="fullPath">The absolete file path.</param>
        /// <param name="model">The model to save.</param>
        /// <exception cref="InvalidOperationException">The given path is empty or invalid.</exception>
        public void WriteJsonFile<TModel>(string fullPath, TModel model)
            where TModel : class
        {
            // validate
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The file path is empty or invalid.", nameof(fullPath));

            // create directory if needed
            string dir = Path.GetDirectoryName(fullPath);
            if (dir == null)
                throw new ArgumentException("The file path is invalid.", nameof(fullPath));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // write file
            string json = this.Serialise(model);
            File.WriteAllText(fullPath, json);
        }

        /// <summary>Deserialize JSON text if possible.</summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <param name="json">The raw JSON text.</param>
        public TModel Deserialise<TModel>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<TModel>(json, this.JsonSettings);
            }
            catch (JsonReaderException)
            {
                // try replacing curly quotes
                if (json.Contains("“") || json.Contains("”"))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<TModel>(json.Replace('“', '"').Replace('”', '"'), this.JsonSettings);
                    }
                    catch { /* rethrow original error */ }
                }

                throw;
            }
        }

        /// <summary>Serialize a model to JSON text.</summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <param name="model">The model to serialise.</param>
        /// <param name="formatting">The formatting to apply.</param>
        public string Serialise<TModel>(TModel model, Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(model, formatting, this.JsonSettings);
        }
    }
}
