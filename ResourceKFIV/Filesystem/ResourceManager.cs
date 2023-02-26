using FormatKFIV.Asset;
using FormatKFIV.FileFormat;

namespace ResourceKFIV.Filesystem
{
    /// <summary>Static storage location for all currently loaded/allocated data.</summary>
    public static class ResourceManager
    {
        private static List<FIFormat<Model>> formatsModel;
        private static List<FIFormat<Texture>> formatsTexture;
        private static List<FIFormat<Scene>> formatsScene;
        private static List<FIFormat<Param>> formatsParam;

        public static VirtualFileSystem vfs = new();

        public static void Initialize()
        {
            //Initialize Model Handlers
            formatsModel = new List<FIFormat<Model>>
            {
                new FFModelICO(),
                new FFModelOMD(),
                new FFModelOM2(),
                new FFModelMOD(),
                new FFModelCHR(),
                new FFModelOBJ()
            };

            //Initialize Texture Handlers
            formatsTexture = new List<FIFormat<Texture>>
            {
                new FFTextureTX2(),
                new FFTextureTM2(),
                new FFTextureTMX(),
                new FFTextureTGA(),
                new FFTexturePNG()
            };

            //Initialize Scene Handlers
            formatsScene = new List<FIFormat<Scene>>
            {
                new FFSceneMAP()
            };

            //Initialize Param Handlers
            formatsParam = new List<FIFormat<Param>>
            {
                new FFParamReverb(),
                new FFParamItemName(),
                new FFParamWeapon(),
                new FFParamMagic()
            };
        }

        /// <summary>Scan through each registered format to see if a particular one is supported</summary>
        /// <param name="fileExt">The extension of the file to scan</param>
        /// <param name="fileBuffer">Data buffer of the file to scan</param>
        /// <param name="type">(OUT) The returning type of the format, or FEType.None</param>
        /// <param name="handler">(OUT) The returning handler of the format, or null</param>
        /// <returns>True if the format is supported, false otherwise</returns>
        public static bool FormatIsSupported(string fileExt, byte[] fileBuffer, out FEType type, out object handler)
        {
            object formatHandler = null;
            FEType formatType = FEType.None;

            //Scan each model format
            foreach (FIFormat<Model> fmt in formatsModel)
            {
                //Check all extensions this particular format supports...
                string[] extensions = fmt.Parameters.Extensions;
                for (int i = 0; i < extensions.Length; ++i)
                {
                    //First Check - Extension (both with string lower for sanity reasons)
                    if (extensions[i].ToLower() != fileExt.ToLower())
                    {
                        continue;
                    }

                    //Second Check - Validator Delegate call
                    if (!fmt.Parameters.Validator(fileBuffer))
                    {
                        continue;
                    }

                    Logger.LogInfo("Found handler for: " + fileExt);
                    Logger.LogInfo("Handler Class Name: " + fmt.GetType().Name);

                    formatHandler = fmt;
                    formatType = FEType.Model;

                    goto FoundFormatHandler;
                }
            }

            //Scan each texture format
            foreach (FIFormat<Texture> fmt in formatsTexture)
            {
                //Check all extensions this particular format supports...
                string[] extensions = fmt.Parameters.Extensions;
                for (int i = 0; i < extensions.Length; ++i)
                {
                    //First Check - Extension (both with string lower for sanity reasons)
                    if (extensions[i].ToLower() != fileExt.ToLower())
                    {
                        continue;
                    }

                    //Second Check - Validator Delegate call
                    if (!fmt.Parameters.Validator(fileBuffer))
                    {
                        continue;
                    }

                    Logger.LogInfo("Found handler for: " + fileExt);
                    Logger.LogInfo("Handler Class Name: " + fmt.GetType().Name);

                    formatHandler = fmt;
                    formatType = FEType.Texture;

                    goto FoundFormatHandler;
                }
            }

            //Scan each scene format
            foreach (FIFormat<Scene> fmt in formatsScene)
            {
                //Check all extensions this particular format supports...
                string[] extensions = fmt.Parameters.Extensions;
                for (int i = 0; i < extensions.Length; ++i)
                {
                    //First Check - Extension (both with string lower for sanity reasons)
                    if (extensions[i].ToLower() != fileExt.ToLower())
                    {
                        continue;
                    }

                    //Second Check - Validator Delegate call
                    if (!fmt.Parameters.Validator(fileBuffer))
                    {
                        continue;
                    }

                    Logger.LogInfo("Found handler for: " + fileExt);
                    Logger.LogInfo("Handler Class Name: " + fmt.GetType().Name);

                    formatHandler = fmt;
                    formatType = FEType.Scene;

                    goto FoundFormatHandler;
                }
            }

            //Scan each param format
            foreach (FIFormat<Param> fmt in formatsParam)
            {
                //Check all extensions this particular format supports...
                string[] extensions = fmt.Parameters.Extensions;
                for (int i = 0; i < extensions.Length; ++i)
                {
                    //First Check - Extension (both with string lower for sanity reasons)
                    if (extensions[i].ToLower() != fileExt.ToLower())
                    {
                        continue;
                    }

                    //Second Check - Validator Delegate call
                    if (!fmt.Parameters.Validator(fileBuffer))
                    {
                        continue;
                    }

                    Logger.LogInfo("Found handler for: " + fileExt);
                    Logger.LogInfo("Handler Class Name: " + fmt.GetType().Name);

                    formatHandler = fmt;
                    formatType = FEType.Param;

                    goto FoundFormatHandler;
                }
            }

            goto NoFoundFormatHandler; //Exclusively here to piss you off.
            NoFoundFormatHandler: //^^
            Logger.LogWarn("Couldn't find a format handler for: " + fileExt);

            type = formatType;
            handler = formatHandler;
            return false;

            FoundFormatHandler: //Don't get lost in the gotos. You get send here from the loops to avoid redudent code.
            type = formatType;
            handler = formatHandler;
            return true;
        }

        public static FIFormat<Texture>[] GetExportableTextureFormats()
        {
            List<FIFormat<Texture>> formats = new List<FIFormat<Texture>>();

            //Scan for exportable texture formats
            foreach (FIFormat<Texture> fmt in formatsTexture)
            {
                if (!fmt.Parameters.AllowExport)
                    continue;
                formats.Add(fmt);
            }

            //Conversion to array and cleanup
            FIFormat<Texture>[] formatsArray = formats.ToArray();
            formats.Clear();

            return formatsArray;
        }

        public static FIFormat<Model>[] GetExportableModelFormats()
        {
            List<FIFormat<Model>> formats = new List<FIFormat<Model>>();

            //Scan for exportable texture formats
            foreach (FIFormat<Model> fmt in formatsModel)
            {
                if (!fmt.Parameters.AllowExport)
                    continue;
                formats.Add(fmt);
            }

            //Conversion to array and cleanup
            FIFormat<Model>[] formatsArray = formats.ToArray();
            formats.Clear();

            return formatsArray;
        }
    }
}