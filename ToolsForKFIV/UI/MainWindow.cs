using System;
using System.IO;
using FormatKFIV.Asset;
using FormatKFIV.FileFormat;
using ToolsForKFIV.UI.Control;

namespace ToolsForKFIV
{
    public class MainWindow
    {
        private ToolFFParam controltool_Param = new();
        private ToolFFImage controltool_Texture = new();
        private ToolFFModel controltool_Model = new();
        private ToolFFScene controltool_Scene = new();

        #region mwMenuStrip Events

        public void openKFIVTool(string path)
        {
            string foundFile = path;
            string foundPath = Path.GetDirectoryName(foundFile);

            ResourceManager.vfs.Reset();
            ResourceManager.vfs.SetRoot(foundPath + Path.DirectorySeparatorChar);

            //Scan OS File System
            foreach (string dir in Directory.GetDirectories(foundPath, "*", SearchOption.AllDirectories))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    string vfsPath = file.Replace(foundPath + Path.DirectorySeparatorChar, "");

                    if (!vfsPath.Contains("KF4.DAT"))
                    {
                        ResourceManager.vfs.PutResource(new SystemResource(vfsPath, file));
                    }
                }
            }

            foreach (string file in Directory.GetFiles(foundPath))
            {
                string vfsPath = file.Replace(foundPath + Path.DirectorySeparatorChar, "");
                ResourceManager.vfs.PutResource(new SystemResource(vfsPath, file));
            }

            string[] kf4DatFiles;
            string kf4Region = "None";

            switch (Path.GetFileName(foundFile))
            {
                case "SLUS_203.18":
                case "SLUS_203.53":
                    kf4Region = "NTSC";
                    kf4DatFiles = new string[]
                    {
                        $"DATA{Path.DirectorySeparatorChar}KF4.DAT",
                    };
                    break;

                case "SLPS_250.57":
                    kf4Region = "NTSC-J";
                    kf4DatFiles = new string[]
                    {
                        $"DATA{Path.DirectorySeparatorChar}KF4.DAT",
                    };
                    break;

                case "SLES_509.20":
                    kf4Region = "PAL";
                    kf4DatFiles = new string[]
                    {
                        $"DATA{Path.DirectorySeparatorChar}KF4_ENG.DAT",
                    };
                    break;

                default:
                    kf4DatFiles = new string[] { "None" };
                    break;
            }

            if (kf4Region == "None")
            {
                Logger.LogError("Invalid KF4 Data! Did you try to trick the system by renaming something?");
                Logger.LogError($"{foundFile}");
                return;
            }

            Logger.LogInfo($"Loading KFIV Data (region: {kf4Region}, exe: {foundFile})");

            //Scan KF File System
            FFResourceDAT dataDat;
            foreach (string datFile in kf4DatFiles)
            {
                dataDat = FFResourceDAT.LoadFromFile(foundPath + Path.DirectorySeparatorChar + datFile);
                for (int i = 0; i < dataDat.FileCount; ++i)
                {
                    string vfsPath = datFile + Path.DirectorySeparatorChar +
                                     dataDat[i].name.Replace('/', Path.DirectorySeparatorChar);
                    ResourceManager.vfs.PutResource(new VirtualResource(vfsPath, dataDat[i].buffer));
                }
            }

            mwFileTree.EnumurateVFS(ResourceManager.vfs);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WinSettings winSettings = new WinSettings();
            winSettings.ShowDialog();
        }

        #endregion

        public void OpenTool(TreeNode fileNode)
        {
            //Clear current tool
            mwSplit.Panel2.Controls.Clear();

            int resourceIndex = (int)fileNode.Tag;
            if (resourceIndex >= 0)
            {
                //Get File from VFS
                Type resourceType;
                Resource resource = ResourceManager.vfs.GetResource(resourceIndex, out resourceType);

                //Get File Buffer
                byte[] fileBuffer;
                string fileExtension = Path.GetExtension(resource.RelativePath);

                switch (resource)
                {
                    case VirtualResource vres:
                        Logger.LogInfo("Opened file is VIRTUAL type!");

                        fileBuffer = vres.Buffer;
                        break;

                    case SystemResource sres:
                        Logger.LogInfo("Opened file is SYSTEM type!");

                        if (!sres.GetBuffer(out fileBuffer))
                        {
                            Logger.LogError("Failed to aquire system file buffer!!!");
                            return;
                        }

                        break;

                    default:
                        Logger.LogError($"An invalid object is inside the VFS ({resourceType.Name}), WTF?");
                        return;
                }

                //Try to find a format handler, and the format type.
                object formatHandler;
                FEType formatType;

                if (!ResourceManager.FormatIsSupported(fileExtension, fileBuffer, out formatType, out formatHandler))
                {
                    Logger.LogWarn($"Unable to find format handler for file! (file: {resource.RelativePath})");
                    return;
                }

                //Open the required tool for the selected file.
                switch (formatType)
                {
                    default:
                    case FEType.None:
                        Logger.LogError("Invalid file type!");
                        break;

                    //FormatType = Parameters
                    case FEType.Param:
                        Logger.LogInfo("Attempting to cast generic format handler to a parameter format handler...");
                        FIFormat<Param> paramHandler = (FIFormat<Param>)formatHandler;

                        Logger.LogInfo("Attempting to import parameters...");
                        Param paramData = paramHandler.LoadFromMemory(fileBuffer, out _, out _, out _);

                        if (paramData != null)
                        {
                            mwSplit.Panel2.Controls.Add(controltool_Param);
                            controltool_Param.SetParamData(paramData);
                        }
                        else
                        {
                            Logger.LogError("Param Data is NULL!");
                            return;
                        }

                        break;

                    //FormatType == Scene
                    case FEType.Scene:
                        Logger.LogInfo("Attempting to cast generic format handler to a scene format handler...");
                        FIFormat<Scene> sceneHandler = (FIFormat<Scene>)formatHandler;

                        Logger.LogInfo("Attempting to import scene...");
                        Scene sceneData = sceneHandler.LoadFromMemory(fileBuffer, out _, out _, out _);

                        if (sceneData != null)
                        {
                            mwSplit.Panel2.Controls.Add(controltool_Scene);
                            controltool_Scene.SetSceneData(sceneData);
                        }
                        else
                        {
                            Logger.LogError("Scene Data is NULL!");
                            return;
                        }

                        break;

                    //FormatType == Texture
                    case FEType.Texture:
                        Logger.LogInfo("Attempting to cast generic format handler to a texture format handler...");
                        FIFormat<Texture> textureHandler = (FIFormat<Texture>)formatHandler;

                        Logger.LogInfo("Attempting to import texture...");
                        Texture textureData = textureHandler.LoadFromMemory(fileBuffer, out _, out _, out _);

                        if (textureData != null)
                        {
                            mwSplit.Panel2.Controls.Add(controltool_Texture);
                            controltool_Texture.SetTextureData(textureData);
                        }
                        else
                        {
                            Logger.LogError("Texture Data is NULL!");
                            return;
                        }

                        break;

                    //FormatType == Model
                    case FEType.Model:
                        Logger.LogInfo("Attempting to cast generic format handler to a model format handler...");
                        FIFormat<Model> modelHandler = (FIFormat<Model>)formatHandler;

                        Logger.LogInfo("Attempting to import model...");
                        object modelTextureData = null;

                        Model modelData = modelHandler.LoadFromMemory(fileBuffer, out modelTextureData, out _, out _);

                        if (modelData != null)
                        {
                            mwSplit.Panel2.Controls.Add(controltool_Model);
                            controltool_Model.SetModelFile(modelData, (Texture)modelTextureData);
                        }
                        else
                        {
                            Logger.LogError("Model Data is NULL!");
                            return;
                        }

                        break;
                }

                Logger.LogInfo("Import Complete!");
                return;
            }

            return; //Method end (incase you got lost in the woods lad)
        }
    }
}