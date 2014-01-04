using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;



namespace XyBorg.Objects
{
    /// <summary>
    /// Объект являющийся родительским для обновляющихся игровых объектов.
    /// </summary>
    abstract class IBaseObject
    {
        static List<IBaseObject> objects = new List<IBaseObject>();
        protected IBaseObject()
        {
            objects.Add(this);
        }
        internal abstract void Update(GameTime gameTime);
        static internal void UpdateObjects(GameTime gameTime)
        {
            foreach (IBaseObject o in IBaseObject.objects)
            {
                o.Update(gameTime);
            }
        }
    }
    /// <summary>
    /// Объект подкидывает игрока.
    /// </summary>
    class Jumper : IBaseObject
    {
        public Jumper(Vector2 position)
        {
            template_position_y = position.Y;
            windField = new Rectangle((int)(position.X - 50), (int)(position.Y - 300), 100, 300);
        }
        private Rectangle windField;
        private float template_position_y;
        internal override void Update(GameTime gameTime)
        {
            foreach (Player player in Level.current_level.players)
            {
                if (player.BoundingRectangle.Intersects(windField))
                {
                    const int max_vel = 100;
                    int range = (int)MathHelper.Max(template_position_y - player.Position.Y, 1);
                    float force = 10000.0f / range;
                    force = MathHelper.Min(max_vel, force);
                    player.AffectByForce(new Vector2(0, -force));
                }
            }
        }
    }
}
