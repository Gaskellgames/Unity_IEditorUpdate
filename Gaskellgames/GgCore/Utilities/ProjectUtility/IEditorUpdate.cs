namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_IEditorUpdate
    /// </summary>
    
    public interface IEditorUpdate
    {
        /// <summary>
        /// EditorUpdate is called infrequently, but will try to run multiple times per second
        /// </summary>
        public void EditorUpdate();

    } // class end
}
