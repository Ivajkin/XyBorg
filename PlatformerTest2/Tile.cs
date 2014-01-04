using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XyBorg
{
    /// <summary>
    /// Контролирует выявление столкновений и ответное поведение тайла.
    /// </summary>
    enum TileCollision
    {
        /// <summary>
        /// Тайл, который не влияет на передвижение персонажей.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// Полностью твёрдый тайл, который не пропускает персонажей.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// Платформа по которой можно ходить, но с любой стороны,
        /// кроме как сверху, сквозь неё можно пройти.
        /// </summary>
        Platform = 2,

        /// <summary>
        /// Тайл, который убивает игрока (постепенно).
        /// </summary>
        DeadEnd = 3,

        /// <summary>
        /// Лестница, по которой можно карабкаться.
        /// </summary>
        Stair = 4
    }

    /// <summary>
    /// Хранит внешний вид тайла и поведение при столкновении.
    /// </summary>
    struct Tile
    {
        public Texture2D Texture;
        public TileCollision Collision;

#if ZUNE
        public const int Width = 30;
        public const int Height = 20;
#else
        public const int Width = 64;
        public const int Height = 48;
#endif
        public static readonly Vector2 Size = new Vector2(Width, Height);

        /// <summary>
        /// Создание нового тайла.
        /// </summary>
        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}
