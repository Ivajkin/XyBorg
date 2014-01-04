using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace XyBorg.Utility
{
    /// <summary>
    /// Класс предоставляет возможность замера времени исполнения различных задач.
    /// </summary>
    class Profiler
    {
        private static Dictionary<string, Profiler> profilers = new Dictionary<string, Profiler>();
        public Profiler(string name)
        {
            profilers[name] = this;
            profiler_name = name;
        }
        /// <summary>
        /// Сбрасывает иформацию о профилировании в файл.
        /// </summary>
        public static void FlushTimeData()
        {

#if DEBUG
            StreamWriter sw = new StreamWriter(string.Format("profile_data_{0}.{1}.{2}.log.txt", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year));
            foreach (string key in profilers.Keys)
            {
                sw.WriteLine("{0}:\t{1}", key, profilers[key].task_time);
            }
            sw.Close();
#else
#endif
        }
        string profiler_name = "";

        private float task_time = 0;
        private float start_time;
        /// <summary>
        /// Хранит ссылки на запущеные сеансы профилирования, для автоматической смены имени профилера на дочернюю последнему запущенному форму.
        /// Пример:
        ///     Запущен профайлер Draw, запускается Animation,
        ///     имя последнего автоматически изменяется на Draw.Animation.
        /// </summary>
        private static Stack<Profiler> profiles_started = new Stack<Profiler>();
        /// <summary>
        /// Хранит уровень вложенности запуска профайлера.
        /// </summary>
        int embedence_level = 0;
        public void start_task()
        {
            if (started)
                throw new Exception(string.Format("Профайлер \"{0}\" уже запущен.", profiler_name));

            start_time = Time.getTime();

            if (profiles_started.Count != embedence_level)
            {
                if (embedence_level != 0)
                    throw new Exception(string.Format("Уровень вложенности запуска профайлера \"{0}\" изменился!", profiler_name));
                embedence_level = profiles_started.Count;

                Profiler parent = profiles_started.Peek();
                profilers.Remove(profiler_name);
                profiler_name = parent.profiler_name + "." + profiler_name;
                profilers[profiler_name] = this;
            }
            profiles_started.Push(this);

            started = true;
        }
        bool started = false;
        public void end_task()
        {
            if (!started)
                throw new Exception(string.Format("Профайлер \"{0}\" ещё не запущен.", profiler_name));

            profiles_started.Pop();

            task_time += Time.getTime() - start_time;

            if (task_time<0)
#if DEBUG
                throw new Exception(string.Format("Профайлер \"{0}\" создал временной срез, значение времени оказалось отрицательным.", profiler_name));
#else
                task_time = 0;
#endif

            started = false;
        }
    }
}
