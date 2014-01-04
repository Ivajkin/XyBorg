using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XyBorg
{
    class CoordConvert
    {
        public static Vector2 world_into_screen(Vector2 world)
        {
            return new Vector2(world.X - Level.current_level.CameraPosition.X,
                               world.Y + Level.current_level.CameraPosition.Y);
        }
        public static Vector2 screen_into_world(Vector2 screen)
        {
            return new Vector2(screen.X + Level.current_level.CameraPosition.X,
                               screen.Y - Level.current_level.CameraPosition.Y);
        }
        public static float scalar_into_polar(Vector2 vec)
        {
            vec.Normalize();
            float ret = (float)Math.Acos(vec.X);
            if (vec.Y > 0)
                ret = -ret;
            return ret;
        }
        private const float PI = (float)Math.PI;
    }
}
