using System;
using System.Collections.Generic;
using System.Text;

namespace RaspProg.Models
{
    public class Lesson
    {
        public int ID { get; set; }               // ID занятия
        public DateTime Date { get; set; }        // Дата
        public int TimeSlot { get; set; }         // Пара
        public string Group { get; set; }         // Группа
        public string Subject { get; set; }       // Предмет
        public string Teacher { get; set; }       // Преподаватель
        public string Room { get; set; }          // Аудитория / кабинет
        public int Duration { get; set; } = 1;    // Продолжительность (часы)
    }
}
