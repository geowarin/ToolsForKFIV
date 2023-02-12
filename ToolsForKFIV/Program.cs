using System;
using System.IO;
using System.Linq;
using FormatKFIV.Asset;
using FormatKFIV.Utility;
using ToolsForKFIV.Asset;
using ToolsForKFIV.Gltf;
using ToolsForKFIV.UI.Control;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ToolsForKFIV
{
    /// <summary>A STATIC class used to store program settings</summary>
    public class Settings
    {
        /// <summary>Defines the logging level.</summary>
        public int LoggingLevel = 3;  //0 = Exception Only, 1 = Ex and Error, 2 = Ex, Error and Warning, 3 = Everything

        public Colour mtBgCC = Colour.FromARGB(255, 40, 22, 46);
        public Colour mtXAxC = Colour.FromARGB(255, 244, 67, 54);
        public Colour mtYAxC = Colour.FromARGB(255, 3, 169, 244);
        public Colour mtZAxC = Colour.FromARGB(255, 76, 175, 80);
        public bool mtShowGridAxis = true;

        public static Settings LoadConfiguration()
        {
            Settings settings;

            string confYaml = "";

            if(!File.Exists("configuration.yaml"))
            {
                settings = new Settings();

                settings.SaveConfiguration();
                return settings;
            }

            using(StreamReader sr = new StreamReader(File.OpenRead("configuration.yaml")))
            {
                confYaml = sr.ReadToEnd();
            }

            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            settings = deserializer.Deserialize<Settings>(confYaml);

            return settings;
        }
        public void SaveConfiguration()
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var confYaml = serializer.Serialize(this);
            
            using (StreamWriter sw = new StreamWriter(File.OpenWrite("configuration.yaml")))
            {
                sw.Write(confYaml);
            }
        }
    }

    /// <summary>A STATIC class used to assist in logging</summary>
    public class Logger
    {
        public static void LogInfo(string msg)
        {
            if (ResourceManager.settings.LoggingLevel >= 3)
            {
                var dateTime = DateTime.Now;

                Console.Write("<[INFO][");
                Console.Write(dateTime.ToShortTimeString());
                Console.Write("]> ");
                Console.WriteLine(msg);
            }
        }
        public static void LogWarn(string msg)
        {
            if (ResourceManager.settings.LoggingLevel >= 2)
            {
                var dateTime = DateTime.Now;

                Console.Write("<[WARN][");
                Console.Write(dateTime.ToShortTimeString());
                Console.Write("]> ");
                Console.WriteLine(msg);
            }
        }

        public static void LogError(string msg)
        {
            if (ResourceManager.settings.LoggingLevel >= 1)
            {
                var dateTime = DateTime.Now;

                Console.Write("<[SHIT][");
                Console.Write(dateTime.ToShortTimeString());
                Console.Write("]> ");
                Console.WriteLine(msg);
            }
        }
    }

    static class Program
    {
        static void Main()
        {
            ResourceManager.Initialize(AppDomain.CurrentDomain.BaseDirectory);
            var resources = ResourceLoader.OpenKFivFile("/home/geo/Documents/King's Field - The Ancient City/SLUS_203.18");

            // foreach (var resource in resources)
            // {
            //     Console.Out.WriteLine("resource = {0}", resource.RelativePath);
            // }
            
            var resource = resources.ToArray().First(r => r.RelativePath == "DATA/KF4.DAT/005.map");
            // var resource = resources.ToArray().First(r => r.RelativePath == "DATA/KF4.DAT/chr/c0133.chr");
            var asset = ResourceLoader.OpenResource(resource);
            
            // View(asset);
            Export(asset);
        }

        private static void View(AssetType asset)
        {
            switch (asset)
            {
                case SceneAsset sceneAsset:
                {
                    // var fileName = Path.GetFileName(resource.RelativePath);
                    // new GltfExporter(sceneData).Export(fileName);
                    using var scene = new ToolFFScene(800, 600, "Scene", sceneAsset.Scene);
                    scene.Run();
                    break;
                }
                case ModelAsset modelAsset:
                {
                    using var model = new ToolFFModel(800, 600, "Model", modelAsset);
                    model.Run();
                    break;
                }
            }
        }
        private static void Export(AssetType asset)
        {
            switch (asset)
            {
                case SceneAsset sceneAsset:
                {
                    var fileName = Path.GetFileName(asset.RelativePath);
                    new GltfExporter(sceneAsset.Scene).Export(fileName);
                    break;
                }
                case ModelAsset modelAsset:
                {
                    break;
                }
            }
        }
    }
}
