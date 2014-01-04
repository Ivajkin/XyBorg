using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XyBorg.SpellEffects
{
    class Burn : SpellEffect<Player>
    {
        public Burn(float _damage, float time_to_burn, float _interval_to_damage, string _burn_particles, string _burn_particles_distortion, ref Player player)
            : base( ref player, time_to_burn)
        {
            damage = _damage;
            interval_to_damage = _interval_to_damage;

            tmp_time = 0;

            burn_particles = _burn_particles;
            burn_particles_distortion = _burn_particles_distortion;
        }
        private readonly string burn_particles;
        private readonly string burn_particles_distortion;
        /// <summary>
        /// Каждое повреждение.
        /// </summary>
        private readonly float damage;
        /// <summary>
        /// Раз в какой промежуток времени повреждать.
        /// </summary>
        private readonly float interval_to_damage;
        /// <summary>
        /// Буффер времени.
        /// </summary>
        float tmp_time;
        protected override void affect(float time_since_last_frame, ref Player object_to_affect)
        {
            tmp_time += time_since_last_frame;
            if (tmp_time >= interval_to_damage)
            {
                tmp_time -= interval_to_damage;
                // Повреждаем.
                object_to_affect.OnDamaged(null, damage);
                AnimatedEffect.AddEffect(new AnimatedEffect(burn_particles, object_to_affect.Position + new Vector2(RandomValue.get_float(-30, 30), RandomValue.get_float(-30, 30)), SpriteEffects.None));
                AnimatedEffect.AddEffect(new AnimatedEffect(burn_particles_distortion, object_to_affect.Position + new Vector2(RandomValue.get_float(-30, 30), RandomValue.get_float(-30, 30)), SpriteEffects.None));
            }
        }
        protected override void on_start(ref Player object_to_affect, SpellEffect<Player> this_fx)
        {
        }
        protected override void on_end(ref Player object_to_affect, SpellEffect<Player> this_fx)
        {
        }
    }
}
