using RaspProg.Models;
using System.Collections.Generic;

namespace RaspProg.Services
{
    public class TeacherService
    {
        public List<Lesson> Filter(List<Lesson> lessons, string teacherName)
        {
            if (string.IsNullOrWhiteSpace(teacherName))
                return new List<Lesson>();

            // Очистка: пробелы и регистр
            teacherName = teacherName.Trim().ToLower();

            var filtered = new List<Lesson>();

            foreach (var l in lessons)
            {
                if (string.IsNullOrWhiteSpace(l.Teacher))
                    continue;

                string lessonTeacher = l.Teacher.Trim().ToLower();

                // Поиск по точному совпадению или подстроке
                if (lessonTeacher.Contains(teacherName))
                    filtered.Add(l);
            }

            return filtered;
        }

        public int CalculateHours(List<Lesson> lessons)
        {
            int sum = 0;
            foreach (var l in lessons)
            {
                sum += l.Duration;
            }
            return sum;
        }
    }
}