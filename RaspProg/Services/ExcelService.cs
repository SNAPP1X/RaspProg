using OfficeOpenXml;
using RaspProg.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace RaspProg.Services
{
    public class ExcelService
    {
        public List<Lesson> LoadLessons(string path)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var lessons = new List<Lesson>();

            using var package = new ExcelPackage(new FileInfo(path));
            var sheet = package.Workbook.Worksheets[0]; // первый лист

            if (sheet.Dimension == null) return lessons; // если лист пустой

            int rows = sheet.Dimension.End.Row;

            for (int i = 2; i <= rows; i++) // с 2 строки (первая — заголовки)
            {
                string idText = sheet.Cells[i, 1]?.Text?.Trim();
                string dateText = sheet.Cells[i, 2]?.Text?.Trim();
                string timeSlotText = sheet.Cells[i, 3]?.Text?.Trim();
                string groupText = sheet.Cells[i, 5]?.Text?.Trim();
                string roomText = sheet.Cells[i, 6]?.Text?.Trim();
                string subjectText = sheet.Cells[i, 7]?.Text?.Trim();
                string teacherText = sheet.Cells[i, 8]?.Text?.Trim();

                // Пропускаем строку, если нет даты, предмета или преподавателя
                if (string.IsNullOrWhiteSpace(dateText) ||
                    string.IsNullOrWhiteSpace(subjectText) ||
                    string.IsNullOrWhiteSpace(teacherText))
                    continue;

                // Парсим ID
                int.TryParse(idText, out int id);

                // Парсим дату
                if (!DateTime.TryParse(dateText, out DateTime date))
                    continue;

                // Парсим номер пары (TimeSlot)
                if (!int.TryParse(timeSlotText, out int timeSlot))
                    timeSlot = 1; // по умолчанию 1

                // Создаем объект Lesson
                lessons.Add(new Lesson
                {
                    ID = id,
                    Date = date,
                    TimeSlot = timeSlot,
                    Group = groupText ?? "",
                    Subject = subjectText ?? "",
                    Teacher = teacherText ?? "",
                    Room = roomText ?? "",
                    Duration = 1 // по умолчанию 1 час
                });
            }

            return lessons;
        }
    }
}