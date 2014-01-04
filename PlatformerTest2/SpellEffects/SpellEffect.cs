using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XyBorg
{
    /// <summary>
    /// Эффект, оказывающий влияние на определённый объект в течении заданного времени.
    /// В начале и по окончании действия эффекта происходят события on_start и on_end.
    /// </summary>
    /// <typeparam name="affectable_t">Тип объекта, подверженного влиянию эффекта.</typeparam>
    abstract class SpellEffect<affectable_t>
    {
        abstract protected void affect(float time_since_last_frame, ref affectable_t object_to_affect);
        abstract protected void on_start(ref affectable_t object_to_affect, SpellEffect<affectable_t> this_fx);
        abstract protected void on_end(ref affectable_t object_to_affect, SpellEffect<affectable_t> this_fx);

        affectable_t object_to_affect;
        protected float fx_durance_time = 0;
        static public float max_fx_durance_time {get; private set;}
        public SpellEffect(ref affectable_t _object_to_affect, float _fx_durance_time)
        {
            object_to_affect = _object_to_affect;

            fx_durance_time = _fx_durance_time;
            max_fx_durance_time = fx_durance_time;
            SpellEffects.Add(this);

            on_start(ref object_to_affect, this);
        }
        void update(float elapsed_time)
        {
            fx_durance_time -= elapsed_time;
            if (fx_durance_time <= 0)
            {
                on_end(ref object_to_affect, this);
                dead = true;
                return;
            }
            affect(elapsed_time, ref object_to_affect);
        }
        private bool dead = false;

        static private List<SpellEffect<affectable_t>> SpellEffects = new List<SpellEffect<affectable_t>>();
        static internal void Update(GameTime time)
        {
            float elapsed_time = (float)time.ElapsedGameTime.TotalSeconds;
            List<SpellEffect<affectable_t>> OnRemove = new List<SpellEffect<affectable_t>>();
            foreach (SpellEffect<affectable_t> se in SpellEffects)
            {
                se.update(elapsed_time);
                if(se.dead)
                    OnRemove.Add(se);
            }
            foreach (SpellEffect<affectable_t> se in OnRemove)
            {
                SpellEffects.Remove(se);
            }
        }
    }

}
