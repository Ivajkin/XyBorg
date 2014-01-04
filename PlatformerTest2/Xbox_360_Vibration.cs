using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XyBorg
{
    internal static class Xbox_360_Vibration
    {
        /// <summary>
        /// Набор параметров для затухания.
        /// </summary>
        class FadeParamSet {
            public FadeParamSet(float _motor_force, float _time_full)
            {
                motor_force = _motor_force;
                time_full = _time_full;
                time_elapsed = 0;
            }
            //Vector2 motor_force;
            float motor_force;
            float time_elapsed;
            float time_full;
            public Vector2 getCurrentForces(float time_elapsed_since_last_frame)
            {
                time_elapsed += time_elapsed_since_last_frame;
                if (time_full < time_elapsed)
                {
                    return Vector2.Zero;
                }
                else
                {
                    return strength_to_motor_force(motor_force * (1 - time_elapsed / time_full));
                }
            }
            private Vector2 strength_to_motor_force(float strength)
            {
                float motor1 = 0;
                float motor2 = 0;
                motor1 = Math.Min(strength * 2, 1);
                if (strength > 0.5f)
                {
                    motor2 = Math.Min((strength - 0.5f) * 2, 1);
                }
                return new Vector2(motor1, motor2);
            }
        }
        static List<FadeParamSet> FadeParams = new List<FadeParamSet>();
        /// <summary>
        /// Добавляем силу вибрации на время time.
        /// </summary>
        /// <param name="strength">Сила вибрации.</param>
        /// <param name="time">Время затухания вибрации.</param>
        internal static void AddForce(float strength, float time)
        {
            /*float motor1 = 0;
            float motor2 = 0;
            motor1 = Math.Min(strength * 2,1);
            if (strength > 0.5f)
            {
                motor2 = Math.Min((strength - 0.5f) * 2, 1);
            }*/

            FadeParams.Add(new FadeParamSet(strength, time));
        }
        /// <summary>
        /// Обновление, происходит на каждом кадре.
        /// </summary>
        /// <param name="time_elapsed"></param>
        internal static void Update(float time_elapsed)
        {
            Vector2 current_full_force = Vector2.Zero;
            List<FadeParamSet> RemoveList = new List<FadeParamSet>();
            foreach(FadeParamSet force in FadeParams) {
                Vector2 cf = force.getCurrentForces(time_elapsed);
                if (cf.LengthSquared() == 0)
                {
                    RemoveList.Add(force);
                }
                else
                {
                    current_full_force += cf;
                }
            }
            foreach (FadeParamSet force in RemoveList)
            {
                FadeParams.Remove(force);
            }
            GamePad.SetVibration(PlayerIndex.One, current_full_force.X, current_full_force.Y);
        }
        internal static void Reset()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
            GamePad.SetVibration(PlayerIndex.Two, 0, 0);
            GamePad.SetVibration(PlayerIndex.Three, 0, 0);
            GamePad.SetVibration(PlayerIndex.Four, 0, 0);
        }
    }
}
