using System;
using System.Collections.Generic;

namespace GLX
{
    /// <summary>
    /// A simple wrapper for XNAs ContentManager
    /// </summary>
    /// <typeparam name="T">The type of content this ContentManager is holder</typeparam>
    public class ContentManager<T>
    {
        private Microsoft.Xna.Framework.Content.ContentManager Content;
        private Dictionary<string, T> loadedAssets;

        /// <summary>
        /// Returns the given asset. Case-insensitive.
        /// If the asset hasn't been loaded this throws an exception.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T this[string key]
        {
            get
            {
                if (loadedAssets.ContainsKey(key))
                {
                    return loadedAssets[key];
                }
                else
                {
                    throw new GLXException("That asset hasn't been loaded.");
                }
            }
        }

        /// <summary>
        /// Creates a new ContentManager.
        /// </summary>
        /// <param name="Content">The XNA ContentManager.</param>
        public ContentManager(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            this.Content = Content;
            loadedAssets = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Loads an asset with the given string name.
        /// </summary>
        /// <param name="assetName"></param>
        public void Load(string assetName)
        {
            loadedAssets.Add(assetName, Content.Load<T>(assetName));
        }
    }
}
