using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiplomaSchool
{
    public partial class AddForm : Form
    {
        public AddForm()
        {
            InitializeComponent();
        }

        internal DialogResult dialogResult;
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public int TeacherId { get; set; }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (subjectTextBox.Text != "" && classTextBox.Text != "" && lastNameTextBox.Text != ""
                && firstNameTextBox.Text != "" && surnameTextBox.Text != "" && topicTextBox.Text != "")
            {
                //bool canSave = true;
                GetIndexesFromTables();
                dialogResult = MessageBox.Show("Сохранить данные о занятии?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes) // при сохранении закрываем форму
                    Close();
            }
            else
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void GetIndexesFromTables()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();
                string subjectIdQuery = "SELECT id FROM school_subjects WHERE subject = @subject";
                using (SQLiteCommand findSubId = new SQLiteCommand(subjectIdQuery, connection))
                {
                    findSubId.Parameters.AddWithValue("@subject", subjectTextBox.Text);
                    object result = findSubId.ExecuteScalar();
                    if (result != null)
                    {
                        SubjectId = Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new MyException("Такого предмета не существует!\nПовторите ввод или обратитесь к администартору");
                    }
                }

                string classIdQuery = "SELECT id FROM classes WHERE class = @class";
                using (SQLiteCommand findClassId = new SQLiteCommand(classIdQuery, connection))
                {
                    findClassId.Parameters.AddWithValue("@class", classTextBox.Text);
                    object result = findClassId.ExecuteScalar();
                    if (result != null)
                    {
                        ClassId = Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new MyException("Такого класса не существует!\nПовторите ввод или обратитесь к администартору");
                    }
                }

                string teacherIdQuery = "SELECT id FROM teachers WHERE " +
                    "last_name = @last_name AND first_name = @first_name AND surname = @surname";
                using (SQLiteCommand findTeacherId = new SQLiteCommand(teacherIdQuery, connection))
                {
                    findTeacherId.Parameters.AddWithValue("@last_name", lastNameTextBox.Text);
                    findTeacherId.Parameters.AddWithValue("@first_name", firstNameTextBox.Text);
                    findTeacherId.Parameters.AddWithValue("@surname", surnameTextBox.Text);
                    object result = findTeacherId.ExecuteScalar();
                    if (result != null)
                    {
                        TeacherId = Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new MyException("Такого учителя не существует!\nПовторите ввод или обратитесь к администартору");
                    }
                }
                connection.Close();
            }
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
