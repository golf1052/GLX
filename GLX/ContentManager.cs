using System;
using System.Collections.Generic;

namespace GLX
{
    public class ContentManager<T>
    {
        private Microsoft.Xna.Framework.Content.ContentManager Content;
        private Dictionary<string, T> loadedAssets;

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
                    throw new Exception("That asset hasn't been loaded.");
                }
            }
        }

        public ContentManager(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            this.Content = Content;
            loadedAssets = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        }

        public void Load(string assetName)
        {
            loadedAssets.Add(assetName, Content.Load<T>(assetName));
        }
    }
}
