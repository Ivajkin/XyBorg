using System;
using System.Collections.Generic;
using System.Text;
using XyBorg.GUIElements;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace XyBorg
{
    static class GUI
    {
        static private List<BaseElement> elements = new List<BaseElement>();
        static public void HandleInput()
        {
            InvalidOperationException exception;
            MouseState mstate = Mouse.GetState();
            KeyboardState kstate = Keyboard.GetState();
            try
            {
                foreach (BaseElement element in elements)
                {
                    element.HandleInput(mstate, kstate);
                    if (elements.Count < 1)
                        break;
                }
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }
        }
        static public void Draw(SpriteBatch spriteBatch)
        {
            foreach (BaseElement element in elements)
            {
                element.Draw(spriteBatch);
            }
        }
        /*public enum element_t
        {
            button,
            canvas
        }*/
        /*public static void Create( element_t element, string image_path)
        {
            switch (element)
            {
                case element_t.canvas:
                    //elements.Add(new Canvas(new Vector2(0, 0), new Vector2(0, 0), "Overlays/overlay_test"));
                    elements.Add(new Canvas(new Vector2(0, 0), new Vector2(0, 0), image_path));
                    break;
                case element_t.button:
                    elements.Add(new Button(new Vector2(0, 0), new Vector2(0, 0), "", "", ""));
                    break;
            }
        }*/
        static private Dictionary<string,List<BaseElement>> ElementSets = new Dictionary<string,List<BaseElement>>();
        /*public static void DestroyLast()
        {
            elements.RemoveAt(elements.Count-1);
        }*/
        /// <summary>
        /// Добавляем элемент интерфейса в набор с именем "name".
        /// Например Add("Главное меню",new Panel(10,10,100,10)) добавит новый элемент типа Panel к набору "Главное меню".
        /// Набор не требует создания, автоматически создаётся при первом упоминании.
        /// </summary>
        /// <param name="name">Имя набора.</param>
        /// <param name="element">Элемент интерфейса для добавления.</param>
        public static void Add(string name, BaseElement element)
        {
            elements.Add(element);
            if (!ElementSets.ContainsKey(name))
                ElementSets[name] = new List<BaseElement>();
            ElementSets[name].Add(element);
        }
        /// <summary>
        /// Уничтожаем набор элементов интерфейса с именем "name".
        /// Например Destroy("Главное меню") уничтожит набор элементов, принадлежащих набору "Главное меню".
        /// </summary>
        /// <param name="name">Имя набора для уничтожения.</param>
        public static void Destroy(string name)
        {
            if (ElementSets.ContainsKey(name))
            {
                foreach (BaseElement le in ElementSets[name])
                {
                    le.Dispose();
                    elements.Remove(le);
                }
                ElementSets.Remove(name);
            }
        }
        /// <summary>
        /// Задаёт размер в пол окна приложения.
        /// Например запущено при разрзрешении 1280х720
        ///     - значит в переменной должны содержаться значения 640х360 перед использованием.
        /// </summary>
        public static Vector2 window_half_size;
    }
}