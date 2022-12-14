using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace CSCE3513_LaserTag_Project.Utils
{
    public abstract class SharedViewModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler collectionChanged = CollectionChanged;
            if (collectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in collectionChanged.GetInvocationList())
                {
                    var dispObj = nh.Target as DispatcherObject;

                    var dispatcher = dispObj?.Dispatcher;
                    if (dispatcher != null && !dispatcher.CheckAccess())
                    {
                        dispatcher.BeginInvoke(
                            (Action)(() => nh.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                            DispatcherPriority.DataBind);
                        continue;
                    }

                    nh.Invoke(this, e);
                }
        }


        /// <summary>
        /// Assign a value to the given field and raise PropertyChanged for the caller.
        /// </summary>
        protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            OnPropertyChanged(propName);
        }

        /// <summary>
        /// Assign a value using the given setter and raise PropertyChanged for the caller.
        /// </summary>
        protected virtual void SetValue<T>(Action<T> setter, T value, [CallerMemberName] string propName = "")
        {
            if (setter == null)
                throw new ArgumentNullException(nameof(setter));

            setter.Invoke(value);
            OnPropertyChanged(propName);
        }

        /// <summary>
        /// Fires PropertyChanged for all properties.
        /// </summary>
        public void RefreshModel()
        {
            foreach (var property in GetType().GetProperties())
                OnPropertyChanged(property.Name);
        }
    }

    public sealed class Persistent<T> : IDisposable where T : new()
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

        ~Persistent()
        {
            Dispose();
        }

        public Persistent(string path, T data = default)
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

        public static Persistent<T> Load(string path, bool saveIfNew = true)
        {
            Persistent<T> config = null;

            if (File.Exists(path))
            {
                try
                {
                    var ser = new XmlSerializer(typeof(T));
                    using (var f = File.OpenText(path))
                    {
                        config = new Persistent<T>(path, (T)ser.Deserialize(f));
                    }
                }
                catch
                {
                    config = null;
                }
            }
            if (config == null)
                config = new Persistent<T>(path, new T());
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
