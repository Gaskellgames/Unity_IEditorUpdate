#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_IEditorUpdate
    /// </summary>
    
    [InitializeOnLoad]
    public static class EditorUpdateLoop
    {
        private static List<IEditorUpdate> editorUpdateList = new List<IEditorUpdate>();
        
        /// <summary>
        /// Constructor will be called during initialisation
        /// </summary>
        static EditorUpdateLoop()
        {
            EditorApplication.update += HandleIEditorUpdateCallbacks;
        }
        
        /// <summary>
        /// Handle subscribing to or from the editor update loop
        /// </summary>
        private static void HandleIEditorUpdateCallbacks()
        {
            // playmode check
            if (Application.isPlaying)
            {
                // unsubscribe all existing IEditorUpdate components from the editor update loop
                if (0 < editorUpdateList.Count) { UnsubscribeFromEditorUpdateLoop(); }
                return;
            }

            // if list of all IEditorUpdate has changed ...
            if (EditorUpdateListChanged())
            {
                // ... subscribe all IEditorUpdate components to editor update loop
                SubscribeToEditorUpdateLoop();
                VerboseLogs.Log($"Components using IEditorUpdate: {editorUpdateList.Count}");
            }
        }

        /// <summary>
        /// Checks to see if the list of all IEditorUpdate has changed
        /// </summary>
        /// <returns>True if changed, False if not</returns>
        private static bool EditorUpdateListChanged()
        {
            // get a list of all IEditorUpdate in the open scenes
            List<IEditorUpdate> newEditorUpdateList = new List<IEditorUpdate>();
            List<GameObject> rootObjects = SceneExtensions.GetAllRootGameObjects();
            foreach(GameObject root in rootObjects)
            {
                IEditorUpdate[] iEditorUpdateArray = root.GetComponentsInChildren<IEditorUpdate>(true);
                foreach (IEditorUpdate iEditorUpdate in iEditorUpdateArray)
                {
                    newEditorUpdateList.Add(iEditorUpdate);
                }
            }

            // check to see if the list has been updated
            foreach (IEditorUpdate iEditorUpdate in editorUpdateList)
            {
                if (newEditorUpdateList.Contains(iEditorUpdate)) { continue; }
                UnsubscribeFromEditorUpdateLoop();
                editorUpdateList = newEditorUpdateList;
                return true;
            }
            foreach (IEditorUpdate iEditorUpdate in newEditorUpdateList)
            {
                if (editorUpdateList.Contains(iEditorUpdate)) { continue; }
                UnsubscribeFromEditorUpdateLoop();
                editorUpdateList = newEditorUpdateList;
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Subscribe a list of components with IEditorUpdate to the editor update loop
        /// </summary>
        private static void SubscribeToEditorUpdateLoop()
        {
            for (var index = editorUpdateList.Count - 1; index >= 0; index--)
            {
                if (editorUpdateList[index] != null)
                {
                    EditorApplication.update += editorUpdateList[index].EditorUpdate;
                }
                else
                {
                    editorUpdateList.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Unsubscribe a list of components with IEditorUpdate from the editor update loop
        /// </summary>
        private static void UnsubscribeFromEditorUpdateLoop()
        {
            foreach (IEditorUpdate iEditorUpdate in editorUpdateList)
            {
                EditorApplication.update -= iEditorUpdate.EditorUpdate;
            }
            editorUpdateList.Clear();
        }

    } // class end
}
#endif
