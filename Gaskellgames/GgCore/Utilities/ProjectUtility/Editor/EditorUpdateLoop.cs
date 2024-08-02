#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            // for all existing components with IEditorUpdate unsubscribe them from the editor update loop
            foreach (var editorUpdateMethod in editorUpdateList)
            {
                EditorApplication.update -= editorUpdateMethod.EditorUpdate;
            }
            editorUpdateList.Clear();
            
            // playmode check
            if (Application.isPlaying) { return; }
            
            // find all components with IEditorUpdate (including all inactive or disabled)
            GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach(GameObject root in rootObjs)
            {
                editorUpdateList.AddRange(root.GetComponentsInChildren<IEditorUpdate>(true));
            }

            // for all components with IEditorUpdate subscribe them to the editor update loop
            foreach (var editorUpdateMethod in editorUpdateList)
            {
                editorUpdateMethod.EditorUpdate();
            }
        }

    } // class end
}
#endif