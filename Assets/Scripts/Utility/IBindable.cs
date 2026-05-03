namespace Utility
{
    public interface IBindable
    {
        /// <summary>
        /// Binds an object
        /// </summary>
        public void BindObject();
        
        /// <summary>
        /// Unbinds an object automatically
        /// </summary>
        public void UnBindObject();
    }
}
