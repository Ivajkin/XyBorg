using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text.RegularExpressions;

namespace XyBorg
{
    /// <summary>
    /// Анимированный эффект, спрайт, загружается из файла вида:
    /// file:
    ///     effect_name Sprites/Player/Run
    ///     frames_per_second   60
    ///     looping true
    ///     [normal_map true]
    /// end_of_file.
    /// </summary>
    class AnimatedEffect
    {
        /// <summary>
        /// Конструктор для создания анимированного эффекта из файла.
        /// </summary>
        /// <param name="file_name">Имя файла вида "Sprites/Effects/Brush-1.afx.txt"</param>
        /// <param name="_position">Расположение вновь соданного эффекта.</param>
        /// <param name="_facing">Поворот эффекта - [зеркальный горизонтальный, зеркальный вертикальный, без поворота].</param>
        public AnimatedEffect(string file_name, Vector2 _position, SpriteEffects _facing)
        {
            if (EffectFiles.ContainsKey(file_name))
            {
                this.animation = EffectFiles[file_name].animation;
                this.position = _position;
                this.facing = _facing;
                this.normal_map = EffectFiles[file_name].normal_map;
                return;
            }

            string path = null;
            for (; ; )
            {
                path = "Sprites/" + file_name;
                path = Path.Combine(
                        Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation,
                        "Content/" + path);
                if (File.Exists(path))
                    break;

                path = file_name;
                path = Path.Combine(
                        Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation,
                        "Content/" + path);
                if (File.Exists(path))
                    break;

                throw new Exception("Wrong effect file!");
            }

            StreamReader sr = new StreamReader(path);
            string line = sr.ReadLine();
            string[] lines = line.Split(" \t".ToCharArray());
            if(lines[0]!="effect_name") {
                sr.Close();
                throw new Exception("Wrong effect file!");
            }
            string fx_name = lines[1];

            line = sr.ReadLine();
            lines = line.Split(" \t".ToCharArray());
            if (lines[0] != "frames_per_second")
            {
                sr.Close();
                throw new Exception("Wrong effect file!");
            }
            float frames_per_second = float.Parse(lines[1]);

            /*line = sr.ReadLine();
            lines = line.Split(" \t".ToCharArray());
            if(lines[0]!="position") {
                sr.Close();
                throw new Exception("Wrong effect file!");
            }
            string[] pos = lines[1].Split(",".ToCharArray());
            Vector2 _position = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));*/

            line = sr.ReadLine();
            lines = line.Split(" \t".ToCharArray());
            if (lines[0] != "looping")
            {
                sr.Close();
                throw new Exception("Wrong effect file!");
            }
            bool looping = (lines[1]=="true");

            bool _normal_map = false;
            if (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                lines = line.Split(" \t".ToCharArray());
                if (lines[0] != "normal_map")
                {
                    sr.Close();
                    if (!lines[0].Contains("#"))
                        throw new Exception("Wrong effect file!");
                }
                else
                {
                    _normal_map = (lines[1] == "true");
                }
            }            

            sr.Close();

            Load(fx_name, 1.0f / frames_per_second, _position, _facing, looping, _normal_map);
            EffectFiles.Add(file_name, this);
        }
        public AnimatedEffect(string fx_name, float frame_time, Vector2 _position, SpriteEffects _facing, bool looping, bool _normal_map)
        {
            Load(fx_name, frame_time, _position, _facing, looping, _normal_map);
        }
        /// <summary>
        /// Загружает анимированный эффект как конструктор.
        /// </summary>
        /// <param name="fx_name"></param>
        /// <param name="frame_time"></param>
        /// <param name="_position"></param>
        /// <param name="_facing"></param>
        /// <param name="looping"></param>
        /// <param name="_normal_map"></param>
        void Load(string fx_name, float frame_time, Vector2 _position, SpriteEffects _facing, bool looping, bool _normal_map)
        {
            position = _position;
            facing = _facing;
            animation = new Animation(Content.Load<Texture2D>(fx_name), frame_time, looping);
            normal_map = _normal_map;
        }
        /// <summary>
        /// Загрузка файла формата:
        ///     'Effects/LaserBeam.afx.txt' {0,0} flip
        ///     'Effects/StreetLight.afx.txt' {0,0} none
        /// </summary>
        /// <param name="fx_file_name">Имя файла</param>
        static public void LoadSceneEffects(string fx_file_name)
        {
            // Проверяем существование файла
            if (!File.Exists(fx_file_name))
            {
                throw new Exception(String.Format("Ошибка при загрузке эффектов на уровне. Файла \"{0}\" не существует.",fx_file_name));
            }
            StreamReader sr = new StreamReader(fx_file_name);
            while(!sr.EndOfStream) {
                string line = sr.ReadLine();
                Regex rx = new Regex(@"'(.+)'\s\{(\d+),(\d+)\}\s(.+)");
                Match match = rx.Match(line);

                string fx_name = match.Groups[1].Value;

                Vector2 position = new Vector2(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));

                string flip = match.Groups[4].Value;
                SpriteEffects facing = SpriteEffects.None;
                if(flip!="none") {
                    if(flip!="flip") {
                        throw new Exception(String.Format("Ошибка при загрузке эффектов на уровне (файл \"{0}\"). Непонятный модификатор поворота - нужно использовать flip|none.",fx_file_name));
                    }
                    facing = SpriteEffects.FlipHorizontally;
                }

                AddEffect(new AnimatedEffect(fx_name, position, facing));
            }
            sr.Close();
        }
        Animation animation = null;
        public Vector2 position = new Vector2(0, 0);

        /// <summary>
        /// Объект не используется и может быть уничтожен.
        /// </summary>
        public bool IsDead { get { return dead; } private set { } }
        private bool dead = false;

        /// <summary>
        /// Поворот эффекта.
        /// </summary>
        SpriteEffects facing = SpriteEffects.None;

        /// <summary>
        /// Указывает, является ли карта нормальной (взрывы, дождь).
        /// </summary>
        bool normal_map = false;

        /// <summary>
        /// Объект для проигрывания анимации эффекта с течением времени.
        /// </summary>
        AnimationPlayer sprite = new AnimationPlayer();

        /// <summary>
        /// Масштаб эффекта.
        /// </summary>
        public float scale = 1.0f;
        /// <summary>
        /// Отрисовка эффекта.
        /// </summary>
        /// <param name="gameTime">Игровое время.</param>
        /// <param name="spriteBatch"></param>
        /// <param name="normal_map">Является ли эффект искажением.</param>
        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch, bool normal_map)
        {
            List<AnimatedEffect> RemoveList = new List<AnimatedEffect>();
            foreach (AnimatedEffect ae in Effects)
            {
                if (normal_map != ae.normal_map)
                    continue;
                ae.sprite.PlayAnimation(ae.animation);
                if (!ae.animation.IsLooping && ae.sprite.FrameIndex >= ae.animation.FrameCount-1)
                {
                    RemoveList.Add(ae);
                    ae.dead = true;
                }
            }
            foreach (AnimatedEffect ae in RemoveList)
            {
                if (normal_map != ae.normal_map)
                    continue;
                Effects.Remove(ae);
            }

            spriteBatch.Begin();
            foreach (AnimatedEffect ae in Effects)
            {
                if (normal_map != ae.normal_map)
                    continue;
                //TODO - Culling
                Vector2 pos = new Vector2(ae.position.X-Level.current_level.CameraPosition.X,ae.position.Y+Level.current_level.CameraPosition.Y);
                ae.sprite.Draw(gameTime, spriteBatch, pos, ae.facing, ae.scale);
            }
            spriteBatch.End();
        }
        /// <summary>
        /// Добавляем эффект (регестрируем для рендеринга).
        /// </summary>
        /// <param name="fx">Эффект для добавления.</param>
        public static void AddEffect(AnimatedEffect fx) {
            Effects.Add(fx);
        }
        /// <summary>
        /// Удаляем эффект (дерегестрируем).
        /// </summary>
        /// <param name="fx">Эффект для уничтожения.</param>
        public static void RemoveEffect(AnimatedEffect fx)
        {
            Effects.Remove(fx);
        }
        /// <summary>
        /// Уничтожаем все эффекты.
        /// </summary>
        public static void CleanEffects()
        {
            Effects.Clear();
            EffectFiles.Clear();
        }
        /// <summary>
        /// Коллекция всех эффектов.
        /// </summary>
        static List<AnimatedEffect> Effects = new List<AnimatedEffect>();
        /// <summary>
        /// Словарь, в котором каждому пути соответствует загруженый эффект (для быстрой загрузки).
        /// </summary>
        static Dictionary<string, AnimatedEffect> EffectFiles = new Dictionary<string, AnimatedEffect>();
    }
}
