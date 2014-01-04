using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace XyBorg
{
    static class Content
    {
        class ContentManagerNotInitialisedExeption : Exception
        {
            public ContentManagerNotInitialisedExeption()
                : base("Менеджер содержимого не инициализирован.") { }
        }
        static public ContentManager get()
        {
            if (content == null)
                throw new ContentManagerNotInitialisedExeption();
            else
                return content;
        }
        static private ContentManager content = null;
        static public void set(ContentManager value) {
            if (content != null)
                throw new Exception("Менеджер содержимого уже инициализирован.");
            else
                content = value;
        }
        public static T Load<T>(string assetName)
        {
            if (content == null)
                throw new ContentManagerNotInitialisedExeption();
            return content.Load<T>(assetName);
        }
    }
}
