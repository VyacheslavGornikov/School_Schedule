using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaSchool
{
    
    internal class Lesson
    {
        public Lesson(int id, DateTime lesson_date, int lesson_number, string lesson_time, string subject, string lesson_topic, string homework, string className, string teacherLName, string teacherFName, string teacherSurname, int cabinet)
        {
            Id = id;
            Lesson_date = lesson_date;
            Lesson_number = lesson_number;
            Lesson_time = lesson_time;
            Subject = subject;
            Lesson_topic = lesson_topic;
            Homework = homework;
            ClassName = className;
            TeacherLName = teacherLName;
            TeacherFName = teacherFName;
            TeacherSurname = teacherSurname;
            Cabinet = cabinet;
        }

        [Browsable(false)]
        public int Id { get; }
        [DisplayName ("Дата")]
        public DateTime Lesson_date { get; set; }
        [DisplayName("Урок")]
        public int Lesson_number { get; set; }
        [DisplayName("Время")]
        public string Lesson_time { get; set; }
        [DisplayName("Предмет")]
        public string Subject { get; set; }
        [DisplayName("Тема урока")]
        public string Lesson_topic { get; set; }
        [DisplayName("Домашнее задание")]
        public string Homework { get; set;}
        [DisplayName("Класс")]
        public string ClassName { get; set; }
        [DisplayName("Фамилия учителя")]
        public string TeacherLName { get; set; }
        [DisplayName("Имя учителя")]
        public string TeacherFName { get; set; }
        [DisplayName("Отчество учителя")]
        public string TeacherSurname { get; set; }
        [DisplayName("Кабинет")]
        public int Cabinet { get; set; }

    }
}
