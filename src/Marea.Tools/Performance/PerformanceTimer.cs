using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace Marea
{
    public class PerformanceTimer
    {
        public static readonly bool IsHighPerformance;

        //[DllImport("Kernel32.dll")]
        //private static extern bool QueryPerformanceCounter(
        //        out long lpPerformanceCount);

        //[DllImport("Kernel32.dll")]
        //private static extern bool QueryPerformanceFrequency(
        //        out long lpFrequency);


        private long m_startTime;
        private long m_stopTime;
        private static long m_freq;

        static PerformanceTimer()
        {
            try
            {
                //IsHighPerformance =QueryPerformanceFrequency(out m_freq);
                IsHighPerformance = Stopwatch.IsHighResolution;
                m_freq = Stopwatch.Frequency;

            }
            catch (Exception)
            {
                IsHighPerformance = false;
            }
        }

        public static long Clock_freq()
        {
            return m_freq;
        }

        public PerformanceTimer()
        {
            m_startTime = 0;
            m_stopTime = 0;
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            // let the waiting threads do their work
            Thread.Sleep(0);

            if (IsHighPerformance)
            {
                //QueryPerformanceCounter(out m_startTime);
                m_startTime = Stopwatch.GetTimestamp();
            }
            else
            {
                m_startTime = DateTime.Now.Ticks;
            }
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Stop()
        {
            if (IsHighPerformance)
            {
                //QueryPerformanceCounter(out m_stopTime);
                m_stopTime = Stopwatch.GetTimestamp();
            }
            else
            {
                m_stopTime = DateTime.Now.Ticks;
            }
        }

        /// <summary>
        /// Gets the current value of the counter
        /// </summary>
        public static long Ticks()
        {
            long value;
            //QueryPerformanceCounter(out value);
            value = Stopwatch.GetTimestamp();
            return value;
        }

        /// <summary>
        /// Calculates the difference in ticks with the given counter value.
        /// </summary>
        public static long TicksDifference(long start)
        {
            long stop;
            //QueryPerformanceCounter(out stop);
            stop = Stopwatch.GetTimestamp();
            return stop - start;
        }

        /// <summary>
        /// Calculates the difference in seconds with the given counter value.
        /// </summary>
        public static double SecondsDifference(long start)
        {
            long stop;
            //QueryPerformanceCounter(out stop);
            stop = Stopwatch.GetTimestamp();
            return (double)(stop - start) / (double)m_freq;
        }

        /// <summary>
        /// Returns the duration of the timer
        /// (in fraction of seconds)
        /// </summary>         
        public double DurationSeconds
        {
            get
            {
                if (IsHighPerformance)
                {
                    return (double)(m_stopTime - m_startTime) /
                        (double)m_freq;
                }
                else
                {
                    TimeSpan span =
                        (new DateTime(m_stopTime)) -
                        (new DateTime(m_startTime));
                    return span.TotalSeconds;
                }
            }
        }
    }
}
