using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace AltTool
{
    public class ClothData: INotifyPropertyChanged
    {
        public ClothNameResolver.Type clothType;
        public ClothNameResolver.DrawableType drawableType;

        public struct ComponentFlags
        {
            public bool unkFlag1;
            public bool unkFlag2;
            public bool unkFlag3;
            public bool unkFlag4;
            public bool isHighHeels;
        }

        public struct PedPropFlags
        {
            public bool unkFlag1;
            public bool unkFlag2;
            public bool unkFlag3;
            public bool unkFlag4;
            public bool unkFlag5;
        }

        public enum Sex
        {
            Male,
            Female
        }

        static int[] idsOffset = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static char offsetLetter = 'a';
        static string[] sexIcons = { "👨🏻", "👩🏻" };
        static string[] typeIcons = { "🧥", "👓" };

        public string mainPath = "";

        public ComponentFlags componentFlags;
        public PedPropFlags pedPropFlags;

        public string fpModelPath;
        public ObservableCollection<string> textures = new ObservableCollection<string>();

        public Sex targetSex;

        public string Icon => sexIcons[(int)targetSex];

        public string Type => typeIcons[(int)clothType];

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private int _currentComponentIndex;
        public int CurrentComponentIndex
        {
            get => _currentComponentIndex;
            set
            {
                _currentComponentIndex = value;
                OnPropertyChanged("CurrentComponentIndex");
            }
        }

        private string _componentNumerics;
        public string ComponentNumerics
        {
            get => _componentNumerics;
            set
            {
                _componentNumerics = value;
                OnPropertyChanged("ComponentNumerics");
            }
        }

        public string DisplayName => $"{_name} (ID: {_currentComponentIndex}) ({_componentNumerics})";

        private string _name = "";
        readonly string _origNumerics = "";
        readonly string _postfix = "";

        public ClothData()
        {

        }

        public ClothData(string path, ClothNameResolver.Type type, ClothNameResolver.DrawableType drawableType, string numeric, string postfix, Sex sex)
        {
            if (!File.Exists(path))
                throw new Exception("YDD file not found");

            clothType = type;
            this.drawableType = drawableType;
            _origNumerics = numeric;

            targetSex = sex;
            _postfix = postfix;

            _name = $"{this.drawableType}_{_origNumerics}";

            mainPath = path;
        }

        public void SearchForFPModel()
        {
            string rootPath = Path.GetDirectoryName(mainPath);
            string fileName = Path.GetFileNameWithoutExtension(mainPath);
            string relPath = rootPath + "\\" + fileName + "_1.ydd";
            fpModelPath = File.Exists(relPath) ? relPath : "";
        }

        public void SetFPModel(string path)
        {
            fpModelPath = path;
        }

        public void SearchForTextures()
        {
            textures.Clear();
            string rootPath = Path.GetDirectoryName(mainPath);

            if(IsComponent())
            {
                for (int i = 0; ; ++i)
                {
                    string relPath = rootPath + "\\" + ClothNameResolver.DrawableTypeToString(drawableType) + "_diff_" + _origNumerics + "_" + (char)(offsetLetter + i) + "_uni.ytd";
                    if (!File.Exists(relPath))
                        break;
                    textures.Add(relPath);
                }
                for (int i = 0; ; ++i)
                {
                    string relPath = rootPath + "\\" + ClothNameResolver.DrawableTypeToString(drawableType) + "_diff_" + _origNumerics + "_" + (char)(offsetLetter + i) + "_whi.ytd";
                    if (!File.Exists(relPath))
                        break;
                    textures.Add(relPath);
                }
            }
            else
            {
                for (int i = 0; ; ++i)
                {
                    string relPath = rootPath + "\\" + ClothNameResolver.DrawableTypeToString(drawableType) + "_diff_" + _origNumerics + "_" + (char)(offsetLetter + i) + ".ytd";
                    if (!File.Exists(relPath))
                        break;
                    textures.Add(relPath);
                }
            }
        }

        public void AddTexture(string path)
        {
            if(!textures.Contains(path))
                textures.Add(path);
        }

        public override string ToString()
        {
            return sexIcons[(int)targetSex] + " " + _name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsComponent() => drawableType <= ClothNameResolver.DrawableType.Top;

        public byte GetComponentTypeID() => IsComponent() ? (byte) drawableType : (byte) 255;

        public bool IsPedProp() => !IsComponent();

        public byte GetPedPropTypeID()
        {
            if (IsPedProp())
                return (byte)((int)drawableType - (int)ClothNameResolver.DrawableType.PropHead);
            return 255;
        }

        public string GetPrefix() => ClothNameResolver.DrawableTypeToString(drawableType);

        public void SetComponentNumerics(string componentNumerics, int currentComponentIndex)
        {
            ComponentNumerics = componentNumerics;
            CurrentComponentIndex = currentComponentIndex;
        }
    }
}
