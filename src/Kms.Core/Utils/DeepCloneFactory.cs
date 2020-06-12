namespace Kms.Core.Utils
{
    /// <summary>
    /// DeepCloneFactory
    /// </summary>
    public static class DeepCloneFactory<T>
    {
        /// <summary>
        /// Deep clone
        /// </summary>
        /// <param name="orgObj">Original object</param>
        /// <returns>The new object</returns>
        public static T Clone(T orgObj)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, orgObj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
