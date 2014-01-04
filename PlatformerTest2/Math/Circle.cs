using System;
using Microsoft.Xna.Framework;

namespace XyBorg
{
    /// <summary>
    /// Представляет окружность на плоскости.
    /// </summary>
    struct Circle
    {
        /// <summary>
        /// Положение центра окружности.
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// Радиус окружности.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Создание новой окружности.
        /// </summary>
        public Circle(Vector2 position, float radius)
        {
            Center = position;
            Radius = radius;
        }

        /// <summary>
        /// Определяет пересечение с прямоугольником.
        /// </summary>
        /// <returns>True если пересечение имеет место. Иначе False.</returns>
        public bool Intersects(Rectangle rectangle)
        {
            Vector2 v = new Vector2(MathHelper.Clamp(Center.X, rectangle.Left, rectangle.Right),
                                    MathHelper.Clamp(Center.Y, rectangle.Top, rectangle.Bottom));

            Vector2 direction = Center - v;
            float distanceSquared = direction.LengthSquared();

            return ((distanceSquared > 0) && (distanceSquared < Radius * Radius));
        }
    }
}
