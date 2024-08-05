using UnityEngine;
using UnityEditor;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_IEditorUpdate
    /// </summary>
    
    public interface IEditorUpdate
    {
        /// <summary>
        /// Calling method for 'EditorUpdate'
        /// </summary>
        public void HandleEditorUpdate()
        {
            // handle auto unsubscription if null
            if ((Object)this) { EditorUpdate(); }
            else { EditorApplication.update -= HandleEditorUpdate; }
        }
        
        /// <summary>
        /// EditorUpdate is called infrequently, but will try to run multiple times per second
        /// </summary>
        public void EditorUpdate();

    } // class end
}
