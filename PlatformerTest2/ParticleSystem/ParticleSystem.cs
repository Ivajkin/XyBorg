using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace XyBorg.ParticleSystem
{
    class ParticleSystem
    {
        List<Particle> particles = new List<Particle>();
        private string particle_data_path;
        /// <summary>
        /// �����������.
        /// </summary>
        internal ParticleSystem(GenericEmitter _starter, string _particle_data_path)
        {
            starter = _starter;
            particle_data_path = _particle_data_path;
        }
        /// <summary>
        /// ������, ��������� ����� ������� ��� ���������� ( void update(time) ).
        /// </summary>
        public GenericEmitter starter;
        /// <summary>
        /// ���������� - ��������� �� ��� �.
        /// </summary>
        public Vector2 gravity = Vector2.Zero;
        /// <summary>
        /// ����� � �������� �������, ����� ��� ��������.
        /// ���� null, ������ �� �������.
        /// </summary>
        public float? time_to_live = null;
        /// <summary>
        /// ���������� - ���������� �� ������ ���������� ������� ������ (������ ��� �� ����).
        /// </summary>
        /// <param name="time_since_last_frame">����� � �������� ������.</param>
        internal void update(float time_since_last_frame)
        {
            if (time_to_live != null)
            {
                time_to_live -= time_since_last_frame;
                if (time_to_live < 0)
                {
                    if (particles.Count == 0)
                        isDead = true;
                }
                else
                {
                    if (particles.Count < GenericEmitter.max_particles_per_ps_num)
                        particles.InsertRange(0, starter.Create(time_since_last_frame, particle_data_path));
                }
            }
            else
            {
                if (particles.Count < GenericEmitter.max_particles_per_ps_num)
                    particles.InsertRange(0, starter.Create(time_since_last_frame, particle_data_path));
            }

            if (particles.Count > GenericEmitter.max_particles_per_ps_num)
                particles.RemoveRange(GenericEmitter.max_particles_per_ps_num - 1, particles.Count - GenericEmitter.max_particles_per_ps_num);

            List<Particle> RemoveList = new List<Particle>();
            foreach (Particle particle in particles)
            {
                if (particle.particle.IsDead)
                {
                    RemoveList.Add(particle);
                }
                else
                {
                    particle.velocity += gravity * time_since_last_frame;
                    particle.update(time_since_last_frame);
                }
            }
            starter.RemoveCheck(RemoveList.Count);
            foreach (Particle particle in RemoveList)
            {
                particles.Remove(particle);
            }
        }
        public bool isDead { get; private set; }
    }
}