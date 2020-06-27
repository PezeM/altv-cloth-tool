using Microsoft.Win32;
using System.IO;
using System.Linq;

namespace AltTool
{
    class ProjectController
    {
        static ProjectController singleton = null;
        public static ProjectController Instance()
        {
            if(singleton == null)
                singleton = new ProjectController();
            return singleton;
        }

        public void AddFiles(ClothData.Sex targetSex)
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Clothes geometry (*.ydd)|*.ydd",
                FilterIndex = 1,
                DefaultExt = "ydd",
                Multiselect = true,
                Title = "Adding " + (targetSex == ClothData.Sex.Male ? "male" : "female") + " clothes"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    string baseFileName = Path.GetFileName(filename);
                    ClothNameResolver cData = new ClothNameResolver(baseFileName);

                    if(!cData.isVariation)
                    {
                        ClothData nextCloth = new ClothData(filename, cData.clothType, cData.drawableType, cData.bindedNumber, cData.postfix, targetSex);
                        
                        if(cData.clothType == ClothNameResolver.Type.Component)
                        {
                            nextCloth.SearchForFPModel();
                            nextCloth.SearchForTextures();

                            var _clothes = MainWindow.clothes.ToList();
                            _clothes.Add(nextCloth);
                            _clothes = _clothes.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();
                            MainWindow.clothes.Clear();

                            foreach(var cloth in _clothes)
                            {
                                MainWindow.clothes.Add(cloth);
                            }

                            StatusController.SetStatus(nextCloth + " added (FP model found: " + (nextCloth.fpModelPath != "" ? "Yes" : "No") + ", Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                        }
                        else
                        {
                            nextCloth.SearchForTextures();

                            var _clothes = MainWindow.clothes.ToList();
                            _clothes.Add(nextCloth);
                            _clothes = _clothes.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();
                            MainWindow.clothes.Clear();

                            foreach (var cloth in _clothes)
                            {
                                MainWindow.clothes.Add(cloth);
                            }

                            StatusController.SetStatus(nextCloth + " added, Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                        }
                    }
                    else
                        StatusController.SetStatus("Item " + baseFileName + " can't be added. Looks like it's variant of another item");
                }
            }
        }

        public void AddTexture(ClothData cloth)
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Clothes texture (*.ytd)|*.ytd",
                FilterIndex = 1,
                DefaultExt = "ytd",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true) return;
            foreach (var filename in openFileDialog.FileNames)
            {
                if (!filename.EndsWith(".ytd")) continue;
                cloth.AddTexture(filename);
            }
        }

        public void SetFPModel(ClothData cloth)
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Clothes drawable (*.ydd)|*.ydd",
                FilterIndex = 1,
                DefaultExt = "ydd",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    if (filename.EndsWith(".ydd"))
                    {
                        cloth.SetFPModel(filename);
                    }
                }
            }
        }
    }
}
