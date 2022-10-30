using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSCE3513_LaserTag_Project.Utils
{
    public sealed class SharedPersistent<T> : IDisposable where T : new()
    {
        //Logger Log = LogManager.GetCurrentClassLogger();

        public string Path { get; set; }
        private T _data;
        public T Data
        {
            get => _data;
            private set
            {
                if (_data is INotifyPropertyChanged npc1)
                    npc1.PropertyChanged -= OnPropertyChanged;
                _data = value;
                if (_data is INotifyPropertyChanged npc2)
                    npc2.PropertyChanged += OnPropertyChanged;
            }
        }

        ~SharedPersistent()
        {
            Dispose();
        }

        public SharedPersistent(string path, T data = default)
        {
            Path = path;
            Data = data;
        }

        private Timer _saveConfigTimer;

        private void SaveAsync()
        {
            if (_saveConfigTimer == null)
            {
                _saveConfigTimer = new Timer((x) => Save());
            }

            _saveConfigTimer.Change(1000, -1);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveAsync();
        }

        public void Save(string path = null)
        {
            if (path == null)
                path = Path;

            try
            {

                var ser = new XmlSerializer(typeof(T));
                using (var f = File.CreateText(path))
                {
                    ser.Serialize(f, Data);
                }
            }
            catch (IOException e)
            {
                //Log.Warn("Couldnt open stream for config file @: " + path, e);
            }
        }

        public static SharedPersistent<T> Load(string path, bool saveIfNew = true)
        {
            SharedPersistent<T> config = null;

            if (File.Exists(path))
            {
                try
                {
                    var ser = new XmlSerializer(typeof(T));
                    using (var f = File.OpenText(path))
                    {
                        config = new SharedPersistent<T>(path, (T)ser.Deserialize(f));
                    }
                }
                catch
                {
                    config = null;
                }
            }
            if (config == null)
                config = new SharedPersistent<T>(path, new T());
            if (!File.Exists(path) && saveIfNew)
                config.Save();

            return config;
        }

        public void Dispose()
        {
            try
            {
                if (Data is INotifyPropertyChanged npc)
                    npc.PropertyChanged -= OnPropertyChanged;
                _saveConfigTimer?.Dispose();
                Save();
            }
            catch
            {
                // ignored
            }
        }
    }
}
