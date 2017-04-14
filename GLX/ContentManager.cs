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
        private readonly bool fullPath;

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
        /// <param name="fullPath">If true, assets are keyed by their full path name.
        /// If false, assets are keyed by only their file name. <see cref="Load(string)"/> will throw an exception if there is a name collision.
        /// Defaults to true.</param>
        public ContentManager(Microsoft.Xna.Framework.Content.ContentManager Content, bool fullPath = true)
        {
            this.Content = Content;
            loadedAssets = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            this.fullPath = fullPath;
        }

        /// <summary>
        /// Loads an asset with the given string name.
        /// </summary>
        /// <param name="assetName">The asset name</param>
        public void Load(string assetName)
        {
            T asset = Content.Load<T>(assetName);

            if (fullPath)
            {
                loadedAssets.Add(assetName, asset);
            }
            else
            {
                string normalizedPath = assetName.Replace('\\', '/');
                int lastSlashIndex = normalizedPath.LastIndexOf('/');
                string key;
                if (lastSlashIndex != -1)
                {
                    key = normalizedPath.Substring(lastSlashIndex + 1);
                }
                else
                {
                    key = normalizedPath;
                }
                loadedAssets.Add(key, asset);
            }
        }
    }
}
