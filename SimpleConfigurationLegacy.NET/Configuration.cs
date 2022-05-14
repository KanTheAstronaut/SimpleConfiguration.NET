using System;
using System.IO;
using System.Text;

namespace SimpleConfigurationLegacy.NET
{
    public class ConfigurationOptions
    {
        /// <summary>
        /// This controls the name of the folder that holds the settings file. This must adhere to folder naming restrictions!
        /// </summary>
        public string ProgramName { get; set; } = "SC";
        /// <summary>
        /// This controls the extension of the settings file (text after the dot) It has no real purpose but to improve configurability.
        /// </summary>
        public string SettingsExtension { get; set; } = "sc";
        /// <summary>
        /// This controls the name of the settings file (text before the extension/dot) and if left empty it will use reflection to get the name of the type specified
        /// </summary>
        public string SettingsName { get; set; }
        /// <summary>
        /// This controls the path that the settings folder will be created in
        /// </summary>
        public string SettingsPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }
    /// <summary>
    /// A simple configuration class that supports simple operations without third-party dependencies.
    /// </summary>
    /// <typeparam name="T">The type to use as the configuration data object and it must have a public parameterless constructor</typeparam>
    public class Configuration<T> where T : new()
    {
        private string LocalPath { get; set; }
        private string LocalFilePath { get; set; }
        /// <summary>
        /// This is the options object passed to the constructor
        /// </summary>
        public ConfigurationOptions Options { get; private set; } = new ConfigurationOptions();
        private T _data = new T();
        /// <summary>
        /// The property that contains your object that will be saved/loaded
        /// </summary>
        /// <remarks>By modifying properties from this the library will not invoke the <see cref="OnDataChanged"/> event as you will need to use <see cref="Set(Action{T})"/> instead</remarks>
        public T Data
        {
            get => _data;
            private set
            {
                if (OnDataChanged != null) OnDataChanged.Invoke(_data, value);
                _data = value;
            }
        }
        public delegate void DataChanged(T dataBefore, T dataAfter);
        /// <summary>
        /// This event is invoked whenever the <see cref="Data"/> property is modified
        /// </summary>
        public event DataChanged OnDataChanged;
        public Configuration(ConfigurationOptions options = null)
        {
            if (options != null)
                Options = options;
            if (string.IsNullOrWhiteSpace(Options.SettingsName))
                Options.SettingsName = typeof(T).Name;
            LocalPath = Path.Combine(Options.SettingsPath, Options.ProgramName);
            LocalFilePath = Path.Combine(LocalPath, $"{Options.SettingsName}.{Options.SettingsExtension}");
        }

        /// <summary>
        /// This checks if there is an existing settings file
        /// </summary>
        /// <returns><see langword="true"></see> if an existing settings file was found</returns>
        public bool SaveExists() => File.Exists(LocalFilePath);

        /// <summary>
        /// Loads the configuration from the settings file
        /// </summary>
        /// <param name="overwrite">The value to save to if it fails to find a settings file and if <paramref name="createIfNull"/> is <see langword="true"/></param>
        /// <param name="createIfNull"><see langword="true"/> will save <paramref name="overwrite"/> or a new instance of <typeparamref name="T"/> if <paramref name="overwrite"/> is null</param>
        /// <returns>The same object that the method was called from</returns>
        public Configuration<T> Load(T overwrite = default, bool createIfNull = false)
        {
            if (!File.Exists(LocalFilePath) && createIfNull)
            {
                var newData = overwrite != null ? overwrite : new T();
                File.WriteAllText(LocalFilePath, System.Text.Json.JsonSerializer.Serialize(newData));
                Data = newData;
            }
            else
                Data = System.Text.Json.JsonSerializer.Deserialize<T>(File.ReadAllBytes(LocalFilePath));
            return this;
        }

        /// <summary>
        /// Serializes and saves the <see cref="Data"/> property to the settings file and creates the settings directory if it does not exist
        /// </summary>
        /// <returns>The same object that the method was called from</returns>
        public Configuration<T> Save()
        {
            if (!Directory.Exists(LocalPath))
                Directory.CreateDirectory(LocalPath);
            File.WriteAllText(LocalFilePath, System.Text.Json.JsonSerializer.Serialize(Data));
            return this;
        }

        /// <summary>
        /// The same as <see cref="SaveExists"/> but is chainable
        /// </summary>
        /// <returns>The same object that the method was called from</returns>
        public Configuration<T> SaveExistsChainable(out bool exists)
        {
            exists = File.Exists(LocalFilePath);
            return this;
        }


        /// <summary>
        /// Allows you to set a field or property while properly invoking <see cref="OnDataChanged"/>
        /// </summary>
        /// <param name="action">You can change any field/property that you want in here</param>
        /// <param name="optimize">This field skips the cloning of the <see cref="Data"/> property to improve performance but will cause dataBefore in the <see cref="OnDataChanged"/> event to be inaccurate</param>
        /// <returns>The same object that the method was called from</returns>
        public Configuration<T> Set(Action<T> action, bool optimize = false)
        {
            T temporaryData;
            if (!optimize)
            {
                var temporarySerializedData = System.Text.Json.JsonSerializer.Serialize(Data);
                temporaryData = System.Text.Json.JsonSerializer.Deserialize<T>(Encoding.UTF8.GetBytes(temporarySerializedData));
            }
            else temporaryData = Data;
            action.Invoke(temporaryData);
            Data = temporaryData;
            return this;
        }

        /// <summary>
        /// Deletes the settings data from the filesystem
        /// </summary>
        /// <param name="onlySettings">Only deletes the settings file and leaves the settings folder</param>
        /// <returns>The same object that the method was called from</returns>
        public Configuration<T> Delete(bool onlySettings = false)
        {
            if (File.Exists(LocalFilePath))
                File.Delete(LocalFilePath);
            if (onlySettings)
                return this;
            if (Directory.Exists(LocalPath))
                Directory.Delete(LocalPath, true);
            return this;
        }

        /// <summary>
        /// This allows you to set the value of the <see cref="Data"/> property
        /// </summary>
        /// <remarks>The only reason this exists is to allow method chaining</remarks>
        /// <param name="value">The value to set <see cref="Data"/> to</param>
        /// <returns>The same object that the method was called from</returns>
        public Configuration<T> Set(T value)
        {
            Data = value;
            return this;
        }
        /// <summary>
        /// Serializes the <see cref="Data"/> property as JSON
        /// </summary>
        /// <returns>Serialized <see cref="Data"/> property</returns>
        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(Data);
        }
    }
}
