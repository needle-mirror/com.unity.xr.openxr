using System;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace UnityEditor.XR.OpenXR.Samples.MeshingFeature
{
    /// <summary>
    /// Custom editor for meshing behaviour that ensures the meshing feature plugin can be loaded.
    ///
    /// For the meshing feature sample to be loaded the MeshingFeature.dll and UnitySubsystemsManifest.json files
    /// must reside in a folder directly under the `Assets` folder, and must exist on Unity startup.  This script
    /// will copy those files to a new folder at `Assets/MeshingFilter` whenever it detects those files are not
    /// in that location.  Once copied Unity must be restarted for it to recognize the new subsystem.
    ///
    /// Note that when developing samples this copying functionality may not be desired.  To disable this
    /// functionality add the UNITY_SAMPLE_DEV define to your project.
    /// </summary>
    [InitializeOnLoad]
    public class MeshingBehaviourEditor : Editor
    {
#if !UNITY_SAMPLE_DEV
        private const string kMeshingFeaturePath = "MeshingFeature";

        private static string[] m_filesToMove = new[]
        {
            "Meshing Subsystem Feature/UnitySubsystemsManifest.json",
            "Meshing Subsystem Feature/windows/x64/MeshingFeature.dll",
            "Meshing Subsystem Feature/windows/x64/MeshingFeature.pdb"
        };

        private const string kErrorMessage = "The Meshing Subsystem Sample requires the MeshingFeature plugin and associated UnitySubsystemsManifest.json file be located in the Assets/MeshingFeature folder to run properly.  An attempt was made to automatically move these files to the correct location but failed, please move these files manaually before running the sample.";

        private const string dialogText = "The Meshing Feature Sample requires that the MeshingFeature subsystem be registered which will require Unity editor to be restarted.\n\nDo you want to *RESTART* the editor now?";

        static MeshingBehaviourEditor()
        {
            var success = true;

            // Create the meshing feature path
            if(!AssetDatabase.IsValidFolder(Path.Combine("Assets", kMeshingFeaturePath)))
                success = !string.IsNullOrEmpty(AssetDatabase.CreateFolder("Assets", kMeshingFeaturePath));

            // Attempt to move all of the files
            if (success)
            {
                var assetPath = "Assets/" + kMeshingFeaturePath;
                foreach (var fileToMove in m_filesToMove)
                    if (!MoveAsset(fileToMove, assetPath))
                    {
                        success = false;
                        break;
                    }
            }

            // If the move was successful suggest a restart so the subsystem will load
            if (success)
            {
                if (EditorUtility.DisplayDialog("Warning", dialogText, "Yes", "No"))
                {
                    // Restart editor.
                    var editorApplicationType = typeof(EditorApplication);
                    var requestCloseAndRelaunchWithCurrentArgumentsMethod =
                        editorApplicationType.GetMethod("RequestCloseAndRelaunchWithCurrentArguments",
                            BindingFlags.NonPublic | BindingFlags.Static);

                    if (requestCloseAndRelaunchWithCurrentArgumentsMethod == null)
                        throw new MissingMethodException(editorApplicationType.FullName, "RequestCloseAndRelaunchWithCurrentArguments");

                    requestCloseAndRelaunchWithCurrentArgumentsMethod.Invoke(null, null);
                }
            }
            // If the move failed just log a message indicating this work needs to be done manually
            else
            {
                Debug.LogError(kErrorMessage);
            }

            // Delete ourself so we never try this again!
            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath("889289a1c4a096340aede480e53f0e7e"));
        }

        /// <summary>
        /// Moves an asset referenced by GUID to a specific location within the asset folder
        /// </summary>
        /// <param name="find">File to find</param>
        /// <param name="target">Target folder to move the asset to.  Note this folder must exist</param>
        /// <returns>True if the file was moved</returns>
        private static bool MoveAsset(string find, string target)
        {
            var source = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(find))
                .Select(r => AssetDatabase.GUIDToAssetPath(r))
                .Where(r => r.Contains(find))
                .FirstOrDefault();

            if (string.IsNullOrEmpty(source))
            {
                Debug.LogError($"Failed to locate MeshingFeature file '{find}'");
                return false;
            }

            target = Path.Combine(target, Path.GetFileName(source));
            var moveResult = AssetDatabase.MoveAsset(source, target);
            if (!string.IsNullOrWhiteSpace(moveResult))
            {
                Debug.LogError(moveResult);
                return false;
            }

            Debug.Log($"Moved '{source}' to '{target}");
            return true;
        }
#endif
    }
}