using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XyBorg
{

    /// <summary>
    /// Класс описывающий эффект влияющий на объекты, врагов, игрока.
    /// После применения эффект уничтожается.
    /// </summary>
    public class AreaEffect
    {
        /// <summary>
        /// Перечисление типов эффектов.
        /// </summary>
        public enum ae_type
        {
            /// <summary>
            /// Эффект убивающий.
            /// </summary>
            kill,
            /// <summary>
            /// Эффект, который наносит повреждения.
            /// </summary>
            damage,
            /// <summary>
            /// Пустой эффект, не делает ни чего.
            /// </summary>
            nothing
        };
        /// <summary>
        /// Тип текущего эффекта.
        /// </summary>
        public ae_type type = ae_type.nothing;
        /// <summary>
        /// Область внутри которой действует эффект.
        /// </summary>
        public Rectangle rect;
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_rect"></param>
        public AreaEffect(ae_type _type, Rectangle _rect)
        {
            type = _type;
            rect = _rect;
            Level.current_level.AreaEffects.Add(this);
        }
        /// <summary>
        /// Применяем эффект.
        /// </summary>
        /// <param name="enemy"></param>
        internal virtual void Use(Enemy enemy)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Можно ли уже уничтожить эффект.
        /// </summary>
        public bool on_destroy = false;
        /// <summary>
        /// Глобальное время с начала игры в секундах.
        /// </summary>
        public static float current_time = 0;
    }
    /// <summary>
    /// Эффект, повреждающий.
    /// </summary>
    public class DamageAreaEffect : AreaEffect
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="_damage">Величина повреждения.</param>
        /// <param name="_rect">Область повреждения.</param>
        public DamageAreaEffect(float _damage, Rectangle _rect)
            : base(ae_type.damage, _rect)
        {
            damage = _damage;
        }
        /// <summary>
        /// Величина повреждения.
        /// </summary>
        public float damage = 0.0f;
        /// <summary>
        /// Применяем эффект.
        /// </summary>
        /// <param name="enemy"></param>
        internal override void Use(Enemy enemy)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Эффект, убивающий.
    /// </summary>
    public class KillAreaEffect : AreaEffect
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="_rect">Облать уничтожения.</param>
        /// <param name="wait_time">Время спустя которое эффект признаётся недействительным.</param>
        public KillAreaEffect(Rectangle _rect, float wait_time)
            : base(ae_type.kill, _rect)
        {
            wait_till_time = current_time + wait_time;
        }
        float wait_till_time = 0;
        /// <summary>
        /// Применяем эффект.
        /// </summary>
        /// <param name="enemy"></param>
        internal override void Use(Enemy enemy)
        {
            if (wait_till_time > current_time)
            {
                if (enemy.IsAlive && enemy.BoundingRectangle.Intersects(rect))
                {
                    enemy.OnKilled(null);
                    on_destroy = true;
                }
            }
            else
                on_destroy = true;
        }
    }
}
