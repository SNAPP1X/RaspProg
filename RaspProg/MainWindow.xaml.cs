using Microsoft.Win32;
using OfficeOpenXml;
using RaspProg.Models;
using RaspProg.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RaspProg
{
    public partial class MainWindow : Window
    {
        List<Lesson> lessons = new();
        ExcelService excel = new();
        TeacherService teacherService = new();
        List<Lesson> filtered = new();

        private const string PlaceholderText = "Имя преподавателя";

        public MainWindow()
        {
            InitializeComponent();
            TeacherBox.Text = PlaceholderText;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                lessons = excel.LoadLessons(dialog.FileName);
                ResultText.Text = $"Загружено: {lessons.Count} уроков";
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string name = TeacherBox.Text.Trim();
            if (string.IsNullOrEmpty(name) || name == PlaceholderText)
            {
                ResultText.Text = "Введите имя преподавателя!";
                return;
            }

            // Фильтруем уроки по преподавателю
            filtered = teacherService.Filter(lessons, name);

            if (filtered.Count == 0)
                ResultText.Text = "Преподаватель не найден!";
            else
                ResultText.Text = $"Найдено {filtered.Count} уроков";
        }

        private void Hours_Click(object sender, RoutedEventArgs e)
        {
            if (filtered.Count == 0)
            {
                ResultText.Text = "Сначала найдите преподавателя!";
                return;
            }

            int hours = filtered.Sum(l => l.Duration);
            ResultText.Text = $"Часы: {hours}";
        }

        private void TeacherBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TeacherBox.Text == PlaceholderText)
                TeacherBox.Text = "";
        }

        private void TeacherBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TeacherBox.Text))
                TeacherBox.Text = PlaceholderText;
        }

        private void GenerateFile_Click(object sender, RoutedEventArgs e)
        {
            if (filtered == null || filtered.Count == 0)
            {
                ResultText.Text = "Сначала найдите преподавателя!";
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = "Отчет.xlsx"
            };

            if (saveDialog.ShowDialog() != true) return;
            string path = saveDialog.FileName;

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using var package = new ExcelPackage();
                var sheet = package.Workbook.Worksheets.Add("Отчет");

                // Заголовки
                sheet.Cells[1, 1].Value = "№";
                sheet.Cells[1, 2].Value = "Группа";
                sheet.Cells[1, 3].Value = "Предмет";
                sheet.Cells[1, 4].Value = "Количество занятий";
                sheet.Cells[1, 5].Value = "Часов";
                sheet.Cells[1, 6].Value = "Преподаватель";

                int row = 2;
                int count = 1;

                // Группировка по группе и предмету (игнорируем номер пары)
                var grouped = filtered
                    .GroupBy(l => new { l.Group, l.Subject })
                    .Select(g => new
                    {
                        Group = g.Key.Group,
                        Subject = g.Key.Subject,
                        Count = g.Count(),               // количество занятий
                        Hours = g.Sum(l => l.Duration)   // суммарные часы
                    })
                    .OrderBy(x => x.Group)
                    .ThenBy(x => x.Subject)
                    .ToList();

                foreach (var item in grouped)
                {
                    sheet.Cells[row, 1].Value = count++;
                    sheet.Cells[row, 2].Value = item.Group;
                    sheet.Cells[row, 3].Value = item.Subject;
                    sheet.Cells[row, 4].Value = item.Count;
                    sheet.Cells[row, 5].Value = item.Hours;
                    sheet.Cells[row, 6].Value = TeacherBox.Text;
                    row++;
                }

                package.SaveAs(new System.IO.FileInfo(path));
                ResultText.Text = $"Файл успешно сохранен: {path}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
            }
        }
    }
}