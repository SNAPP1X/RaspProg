using RaspProg.Models;
using RaspProg.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaspProg.Services
{
    internal class ScheduleService
    {
        public Dictionary<string, string> Generate(List<Lesson> lessons)
        {
            var schedule = new Dictionary<string, string>();

            foreach (var l in lessons)
            {
                if (!RoomMap.Map.ContainsKey(l.Room))
                    continue;

                string key = $"{l.Date:dd.MM.yyyy}_{l.TimeSlot}_{l.Room}";

                if (!schedule.ContainsKey(key))
                {
                    schedule[key] = l.Teacher;
                }
                else
                {
                    schedule[key] += " / " + l.Teacher;
                }
            }

            return schedule;
        }
    }
}
