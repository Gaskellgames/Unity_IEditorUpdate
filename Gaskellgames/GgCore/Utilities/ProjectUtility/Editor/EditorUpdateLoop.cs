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
            EditorApplication.hierarchyChanged += HandleIEditorUpdateCallbacks;
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
            
            if (ComponentListUpdated(out List<IEditorUpdate> addedComponents, out List<IEditorUpdate> removedComponents))
            {
                foreach (IEditorUpdate removedComponent in removedComponents)
                {
                    EditorApplication.update -= removedComponent.HandleEditorUpdate;
                    Debug.Log($"Removed: {removedComponent.GetType()}");
                }
                foreach (IEditorUpdate addedComponent in addedComponents)
                {
                    EditorApplication.update += addedComponent.HandleEditorUpdate;
                    Debug.Log($"Added: {addedComponent.GetType()}");
                }
            
                Debug.Log($"IEditorUpdate: {editorUpdateList.Count}");
            }
        }

        /// <summary>
        /// Checks to see if the list of all IEditorUpdate has changed
        /// </summary>
        /// <returns>True if changed, False if not</returns>
        private static bool ComponentListUpdated(out List<IEditorUpdate> addedComponents, out List<IEditorUpdate> removedComponents)
        {
            // get all 'component type' in open scenes
            List<IEditorUpdate> newEditorUpdateList = new List<IEditorUpdate>();
            List<GameObject> rootObjects = SceneExtensions.GetAllRootGameObjects();
            foreach(GameObject root in rootObjects)
            {
                IEditorUpdate[] componentsArray = root.GetComponentsInChildren<IEditorUpdate>(true);
                foreach (IEditorUpdate debugEvent in componentsArray)
                {
                    newEditorUpdateList.Add(debugEvent);
                }
            }
        
            // check to see if the list has been updated
            bool updated = false;
            addedComponents = new List<IEditorUpdate>();
            removedComponents = new List<IEditorUpdate>();
            foreach (IEditorUpdate editorUpdate in editorUpdateList)
            {
                if (newEditorUpdateList.Contains(editorUpdate)) { continue; }
                removedComponents.Add(editorUpdate);
                updated = true;
            }
            foreach (IEditorUpdate editorUpdate in newEditorUpdateList)
            {
                if (editorUpdateList.Contains(editorUpdate)) { continue; }
                addedComponents.Add(editorUpdate);
                updated = true;
            }

            if (updated) { editorUpdateList = newEditorUpdateList; }
            return updated;
        }

        /// <summary>
        /// Unsubscribe a list of components with IEditorUpdate from the editor update loop
        /// </summary>
        private static void UnsubscribeFromEditorUpdateLoop()
        {
            foreach (IEditorUpdate editorUpdate in editorUpdateList)
            {
                if (!(Object)editorUpdate) { continue; }
                EditorApplication.update -= editorUpdate.HandleEditorUpdate;
            }
            editorUpdateList.Clear();
            
            Debug.Log($"IEditorUpdate: {editorUpdateList.Count}");
        }

    } // class end
}
#endif
