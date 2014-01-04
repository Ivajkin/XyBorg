using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace XyBorg
{
    /// <summary>
    /// Класс для упрощения работы с окружающими объекты прямоугольниками.
    /// </summary>
    public static class RectangleExtensions
    {
        /// <summary>
        /// Расчитывает глубину проникновения между двумя прямоугольниками.
        /// </summary>
        /// <returns>
        /// Глубина пересечения, зависит от стороны проникновения и
        /// помогает найти премещение препятствующее столкновению.
        /// Если пресечение не имеет места быть, возвращается Vector2.Zero.
        /// </returns>
#if false
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
#else
        public static Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
#endif
        {
            // Полуразмеры.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Центры.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Рассчитывает текущие и минимальные ( до столкновения ) радиусы.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // Возвращаем (0, 0) если не столкнулись.
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Расчитываем и возвращаем глубины.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Выдаёт центр низа прямоугольника.
        /// </summary>
#if false
        public static Vector2 GetBottomCenter(this Rectangle rect)
#else
        public static Vector2 GetBottomCenter(Rectangle rect)
#endif
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }
    }
}
