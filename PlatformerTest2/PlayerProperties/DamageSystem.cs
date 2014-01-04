using System;
using System.Collections.Generic;
using System.Text;

namespace XyBorg.PlayerProperties
{
    class DamageSystem
    {
        public DamageSystem()
        {
            current_maxhp = maxhp_player;
            Reset();
        }
        // Количество жизненной энергии игрока.
        public const float maxhp_player = 100.0f;
        // Одноглазый на трёх ногах.
        public const float maxhp_enemyEye = 100.0f;
        // Типы брони.
        public const float armor_none = 1.0f;
        public const float armor_light = 0.7f;
        public const float armor_middle = 0.4f;
        public const float armor_heavy = 0.25f;

        // Броня.
        public float armor = armor_none;
        public float health_points { get; private set; }
        public float current_maxhp { get; private set; }
        public void Damage(float damage)
        {
            health_points -= damage * armor;
            if (health_points<0)
            {
                dead = true;
                health_points = 0;
            }
        }
        public void AddHP(float hp)
        {
            health_points += hp;
            if (health_points > current_maxhp)
                Reset();
        }
        /// <summary>
        /// Оживляем сущность.
        /// </summary>
        public void Reset()
        {
            health_points = current_maxhp;
            armor = armor_none;
            dead = false;
        }
        public bool dead = false;
    }
}
