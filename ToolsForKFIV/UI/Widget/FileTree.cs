// using System.IO;
// using FormatKFIV.Utility;
//
// namespace ToolsForKFIV.UI.Control;
//
// public class FileTree
// {
//     /// <summary>Emumurate a virtual file system, to display its contents </summary>
//     public void EnumurateVFS(VirtualFileSystem vfs)
//     {
//         Resource[] resources = vfs.GetResources();
//         //Stage I - Directories
//         foreach (Resource resource in resources)
//         {
//             Logger.LogInfo(resource.ToString());
//             if(resource.RelativePath.Contains(Path.DirectorySeparatorChar))
//             {
//                 string[] splitPath = resource.RelativePath.Split(Path.DirectorySeparatorChar);
//
//                 TreeNode currentNode;
//                 if (!ftTreeView.Nodes.ContainsKey(splitPath[0]))
//                 {
//                     currentNode = ftTreeView.Nodes.Add(splitPath[0]);
//                     currentNode.Name = splitPath[0];
//                     currentNode.ImageIndex = 0;
//                     currentNode.SelectedImageIndex = currentNode.ImageIndex;
//
//                     currentNode.Tag = -1;
//                 }
//                 else 
//                 {
//                     currentNode = ftTreeView.Nodes[ftTreeView.Nodes.IndexOfKey(splitPath[0])];
//                 }
//
//                 for (int i = 1; i < (splitPath.Length - 1); ++i)
//                 {
//                     if(!currentNode.Nodes.ContainsKey(splitPath[i]))
//                     {
//                         currentNode = currentNode.Nodes.Add(splitPath[i]);
//                         currentNode.Name = splitPath[i];
//                         currentNode.ImageIndex = 0;
//                         currentNode.SelectedImageIndex = currentNode.ImageIndex;
//
//                         currentNode.Tag = -1;
//                     }
//                     else
//                     {
//                         currentNode = currentNode.Nodes[currentNode.Nodes.IndexOfKey(splitPath[i])];
//                     }
//                 }
//             }
//         }
//
//         //Stage II - Files
//         int resourceIndex = 0;
//         foreach (Resource resource in resources)
//         {
//             TreeNode currentNode;
//
//             if (resource.RelativePath.Contains(Path.DirectorySeparatorChar))
//             {
//                 string[] splitPath = resource.RelativePath.Split(Path.DirectorySeparatorChar);
//                 string file = splitPath[splitPath.Length - 1];
//                 currentNode = ftTreeView.Nodes[ftTreeView.Nodes.IndexOfKey(splitPath[0])];
//
//                 //Traverse Tree
//                 for(int i = 1; i < splitPath.Length - 1; ++i)
//                 {
//                     currentNode = currentNode.Nodes[currentNode.Nodes.IndexOfKey(splitPath[i])];
//                 }
//
//                 currentNode = currentNode.Nodes.Add(file);
//                 currentNode.Name = file;
//                 currentNode.ImageIndex = 1 + (int)FileTypeIdentifier.IdentifyByExtension(file);
//                 currentNode.SelectedImageIndex = currentNode.ImageIndex;
//             }
//             else
//             {
//                 currentNode = ftTreeView.Nodes.Add(resource.RelativePath);
//                 currentNode.Name = resource.RelativePath;
//                 currentNode.ImageIndex = 1 + (int)FileTypeIdentifier.IdentifyByExtension(resource.RelativePath);
//                 currentNode.SelectedImageIndex = currentNode.ImageIndex;
//             }
//
//             currentNode.Tag = resourceIndex;
//
//             resourceIndex++;
//         }
//     }
// }