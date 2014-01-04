using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace XyBorg
{
    class LevelEditor
    {
        const int selection_size_in_px = 128;
        const int effects_count = 20;
        /// <summary>
        /// На каком уровне находятся эффекты в панели - выше, ниже.
        /// </summary>
        int effect_position = 0;
        delegate void create_effect_func (int index, string effect_path);
        public LevelEditor()
        {
            isEnabled = true;

            string set_name = "Редактор уровней";

            Vector2 toolbar_origin = new Vector2(640 - selection_size_in_px / 2, -360 + selection_size_in_px / 2);

            create_effect_func create_effect = delegate(int index, string effect_path)
            {
                tool_paths[index] = effect_path;
                GUI.Add(set_name, new XyBorg.GUIElements.Button(toolbar_origin + new Vector2(0, index * selection_size_in_px), new Vector2(0, 0), "Overlays/Editor/blank_tool_place", null, null,
                   delegate()
                   {
                       if (index + effect_position < effects_count && index + effect_position >= 0)
                       {
                           selected_effect_path_index = index + effect_position;
                           selected_effect = new AnimatedEffect(tool_paths[selected_effect_path_index], MousePosition, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
                           AnimatedEffect.AddEffect(selected_effect);
                           //selected_effect_path = effect_path;
                       }
                   }));
                tools[index] = new AnimatedEffect(effect_path, new Vector2(0, 0), Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
                tools[index].scale = 0.5f;
                AnimatedEffect.AddEffect(tools[index]);
            };

            {
                create_effect(0, "Sprites/Effects/Palm1-1.afx.txt");
                create_effect(1, "Sprites/Effects/Palm1-2.afx.txt");
                create_effect(2, "Sprites/Effects/Palm1-3.afx.txt");

                create_effect(3, "Sprites/Effects/Palm2-1.afx.txt");
                create_effect(4, "Sprites/Effects/Palm2-2.afx.txt");

                create_effect(5, "Sprites/Effects/Brush-1.afx.txt");
                create_effect(6, "Sprites/Effects/Brush-2.afx.txt");
                create_effect(7, "Sprites/Effects/Brush-3.afx.txt");

                create_effect(8, "Sprites/Effects/Grass-1.afx.txt");
                create_effect(9, "Sprites/Effects/Grass-2.afx.txt");
                create_effect(10, "Sprites/Effects/Grass-3.afx.txt");
                
                create_effect(11, "Sprites/Effects/DistortionSmallNormalMap.afx.txt");
                create_effect(12, "Sprites/Effects/Fireflies.afx.txt");
                create_effect(13, "Sprites/Effects/FireflyFlower.afx.txt");
                create_effect(14, "Sprites/Effects/Flower1.afx.txt");
                create_effect(15, "Sprites/Effects/GlowingArtefact.afx.txt");
                create_effect(16, "Sprites/Effects/StreetLight.afx.txt");

                create_effect(17, "Sprites/Effects/Decorations/FlameThrowerDec.afx.txt");
                create_effect(18, "Sprites/Effects/Decorations/GatesOfGlass.afx.txt");
                create_effect(19, "Sprites/Effects/Decorations/GatesOfGlassNormalMap.afx.txt");
            }

            // В правом верхнем углу кнопка закрытия.
            GUI.Add(set_name, new XyBorg.GUIElements.Button(new Vector2(624, -344), new Vector2(0, 0), "Overlays/Editor/close_btn", null, null,
                delegate()
                {
                    isEnabled = false;
                    GUI.Destroy(set_name);
                    foreach (AnimatedEffect ae in tools)
                    {
                        AnimatedEffect.RemoveEffect(ae);
                    }
                }));
            
            // В правом верхнем углу кнопка закрытия.
            GUI.Add(set_name, new XyBorg.GUIElements.Button(new Vector2(480, 300), new Vector2(0, 0), "Overlays/Editor/down_arrow_normal", "Overlays/Editor/down_arrow_hover", null,
                delegate()
                {
                    effect_position++;
                }));
            GUI.Add(set_name, new XyBorg.GUIElements.Button(new Vector2(480, 241), new Vector2(0, 0), "Overlays/Editor/up_arrow_normal", "Overlays/Editor/up_arrow_hover", null,
                delegate()
                {
                    effect_position--;
                }));

        }
        public bool isEnabled { get; private set; }
        /// <summary>
        /// Анимированный эффект, который мы используем.
        /// </summary>
        private AnimatedEffect selected_effect = null;
        private int selected_effect_path_index = 0;
        private AnimatedEffect[] tools = new AnimatedEffect[effects_count];
        private string[] tool_paths = new string[effects_count];
        Vector2 MousePosition = Vector2.Zero;
        bool mouse_button_was_pressed = false;
        /// <summary>
        /// Обновляем: двигаем выбранный эффект и т.д.
        /// </summary>
        public void Update()
        {
            if (selected_effect != null)
            {
                MouseState mstate = Mouse.GetState();
                MousePosition = new Vector2(mstate.X, mstate.Y);
                selected_effect.position = MousePosition + new Vector2(Level.current_level.CameraPosition.X, -Level.current_level.CameraPosition.Y);

                // Проверяем нажатие левой, ставим объект и пишем в файл.
                if (CheckPressed(mstate))
                {
                    OnMousePressed();
                }
                // Если нажата правая кнопка мыши сбрасываем выборку.
                if (mstate.RightButton == ButtonState.Pressed)
                {
                    AnimatedEffect.RemoveEffect(selected_effect);
                    selected_effect = null;
                }
            }
            for (int i = 0; i < tools.Length; ++i)
            {
                int originY = - effect_position * selection_size_in_px;
                tools[i].position = new Vector2(Level.current_level.CameraPosition.X + 1220, -Level.current_level.CameraPosition.Y + 128 + i * selection_size_in_px + originY);
            }
        }
        bool CheckPressed(MouseState mstate)
        {
            if (mouse_button_was_pressed)
            {
                if (mstate.LeftButton == ButtonState.Released)
                {
                    mouse_button_was_pressed = false;
                }
            }
            else
            {
                if (mstate.LeftButton == ButtonState.Pressed)
                {
                    mouse_button_was_pressed = true;
                    // OnMousePressed
                    return true;
                }
            }
            return false;
        }
        void OnMousePressed()
        {
            // Создаём новый эффект.
            AnimatedEffect new_selected_effect = new AnimatedEffect(tool_paths[selected_effect_path_index], MousePosition, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
            AnimatedEffect.AddEffect(new_selected_effect);

            // Старый записываем.
            StreamWriter sw = new StreamWriter("EditorPositions.list.txt", true);
            // Строка вида:
            //  'Effects/DistortionSmallNormalMap.afx.txt' {31576,2300} none
            sw.Write('\'' + tool_paths[selected_effect_path_index] + "\' {" + selected_effect.position.X + "," + selected_effect.position.Y + "} none");
            sw.WriteLine();
            sw.Flush();
            sw.Close();

            // Меняем на новый.
            selected_effect = new_selected_effect;
        }
    }
}
