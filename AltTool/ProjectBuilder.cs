using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AltTool
{

    class ProjectBuilder
    {
        public static void BuildProject(string outputFile)
        {
            var data = JsonConvert.SerializeObject(MainWindow.clothes, Formatting.Indented);

            File.WriteAllText(outputFile, data);
        }

        public static void LoadProject(string inputFile)
        {
            var data = JsonConvert.DeserializeObject<List<ClothData>>(File.ReadAllText(inputFile));

            MainWindow.clothes.Clear();

            var clothes = data.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();

            foreach (var cd in clothes)
            {
                MainWindow.clothes.Add(cd);
            }
        }
    }

}