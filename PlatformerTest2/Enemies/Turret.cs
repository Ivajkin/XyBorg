using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XyBorg.Enemies
{
    class Turret
    {
        public Turret(Vector2 _position)
        {
            position = _position;
            angle = 0;

            ps1 = new XyBorg.ParticleSystem.ParticleSystem(new XyBorg.ParticleSystem.FlamethrowerDistortionEmitter(), "Sprites/Effects/HotFireDistortionNormalMap.afx.txt");
            ps1.starter.position = new Rectangle((int)_position.X - 8, (int)_position.Y - 8, 16, 16);
            ParticleSystem.Global.Add(ps1);
            ps2 = new XyBorg.ParticleSystem.ParticleSystem(new XyBorg.ParticleSystem.FlamethrowerEmitter(), "Sprites/Effects/Fire01.afx.txt");
            ps2.starter.position = new Rectangle((int)_position.X - 4, (int)_position.Y - 4, 8, 8);
            ParticleSystem.Global.Add(ps2);
        }
        public Vector2 position;
        public float angle;
        private ParticleSystem.ParticleSystem ps1, ps2;
        class Counter
        {
            public delegate void lambda();
            private float time = 0;
            public void per(float interval, float ellapsed, lambda func_to_do)
            {
                time += ellapsed;
                if (time >= interval)
                {
                    func_to_do();
                    time -= interval;
                }
            }
        }
        Counter counter = new Counter();
        public void Update(GameTime gameTime)
        {
            //SearchPlayer
            //Shoot
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            angle = time;
            ps1.starter.start_velocity_angle = angle;
            ps2.starter.start_velocity_angle = angle;

            counter.per(0.5f, time, delegate()
            {
                foreach (Player player in Level.current_level.players)
                {
                    if ((player.Position - position).LengthSquared() < 320 * 320)
                    {
                        Player p = player;
                        XyBorg.SpellEffects.Burn burn = new XyBorg.SpellEffects.Burn(0.1f, 0.4f, 0.171171f, "Effects/Fire01.afx.txt", "Sprites/Effects/HotFireDistortionNormalMap.afx.txt", ref p);
                    }
                }
            });
        }
    }
    class TurretManager
    {
        readonly Vector2 radius_vector;
        public TurretManager()
        {
            body = Content.Load<Texture2D>("Sprites/Enemies/Turret/Tower0");
            shade = Content.Load<Texture2D>("Sprites/Enemies/Turret/Tower0Shade");
            radius_vector = new Vector2(body.Width / 2,body.Height / 2);
        }
        Texture2D body;
        Texture2D shade;
        //Texture2D dead_body;
        List<Turret> turrets = new List<Turret>();
        public void Add(Turret t)
        {
            turrets.Add(t);
        }
        public void Update(GameTime gameTime)
        {
            foreach (Turret t in turrets)
            {
                t.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Turret t in turrets)
            {
                spriteBatch.Draw(body, t.position, null, Color.White, t.angle, radius_vector, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(shade, t.position - radius_vector, Color.White);
            }
        }
    }
}
