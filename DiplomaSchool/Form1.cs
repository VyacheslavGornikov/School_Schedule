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
    public partial class Form1 : Form
    {
        BindingList<Lesson> LessonsList = new BindingList<Lesson>();
        public Form1()
        {
            InitializeComponent();
            LoadLessonsData();
            dataGridView1.DataSource = LessonsList;
            UserAccessSettings();
        }

        private void UserAccessSettings()
        {
            if (Program.currentUser.UserType != "Администратор")
            {
                btnAdd.Visible = false;
                btnChange.Visible = false;
                btnDelete.Visible = false;
                добавитьToolStripMenuItem.Visible = false;
                изменитьToolStripMenuItem.Visible = false;
                удалитьToolStripMenuItem.Visible = false;
                btnClasses.Visible = false;
                btnTeachers.Visible = false;
                btnPupils.Visible = false;
                btnSubjects.Visible = false;
                btnUsers.Visible = false;
                классыToolStripMenuItem.Visible = false;
                учителяToolStripMenuItem.Visible = false;
                ученикиToolStripMenuItem.Visible = false;
                предметыToolStripMenuItem.Visible = false;
                пользователиToolStripMenuItem.Visible = false;
            }
        }

        private void LoadLessonsData(string whereQuery = "")
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    LessonsList.Clear();
                    string query = "SELECT lessons.id, lesson_date, lesson_number, lesson_time, " +
                        "school_subjects.subject, lesson_topic, homework, " +
                        "classes.class, \r\nteachers.last_name, teachers.first_name, " +
                        "teachers.surname, cabinet FROM lessons\r\n" +
                        "JOIN school_subjects ON subject_id = school_subjects.id\r\n" +
                        "JOIN classes ON class_id = classes.id\r\n" +
                        "JOIN teachers ON teacher_id = teachers.id\r\n " + whereQuery;                    
                    
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Lesson lesson = new Lesson
                                    (
                                        reader.GetInt32(0), // Id
                                        reader.GetDateTime(1), // Lesson_date
                                        reader.GetInt32(2), // Lesson_number
                                        reader.GetString(3), // Lesson_time
                                        reader.GetString(4), // Subject
                                        reader.GetString(5), // Lesson_topic
                                        reader.GetString(6), // Homework
                                        reader.GetString(7), // ClassName
                                        reader.GetString(8), // TeacherLName
                                        reader.GetString(9), // TeacherFName
                                        reader.GetString(10), // TeacherSurname
                                        reader.GetInt32(11) // Cabinet
                                    );
                                LessonsList.Add(lesson);
                            }
                        }
                    }
                    connection.Close();
                    dataGridView1.ClearSelection();
                }
                
            }
        }                     

        private void btnAdd_Click(object sender, EventArgs e)
        {

            AddForm addForm = new AddForm();
            try
            {
                addForm.ShowDialog();
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bool repeat = true;
                while (repeat)
                {
                    try
                    {
                        addForm.ShowDialog();
                        repeat = false;
                    }
                    catch (MyException ex1)
                    {
                        MessageBox.Show(ex1.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            if (addForm.dialogResult == DialogResult.Yes)
            {
                string insertQuery = "INSERT INTO lessons (lesson_date, lesson_number, " +
                    "lesson_time, subject_id, lesson_topic, homework, class_id, teacher_id, cabinet) " +
                    "VALUES (DATE(@lesson_date), @lesson_number, @lesson_time, @subject_id, @lesson_topic," +
                    "@homework, @class_id, @teacher_id, @cabinet)";

                FullRowUpdater(addForm, insertQuery);
                LoadLessonsData();
            }
        }

        private static void FullRowUpdater(AddForm form, string query)
        {
            DateTime lessonDate = Convert.ToDateTime(form.dateTimePicker.Value.ToShortDateString());
            int lessonNumber = Convert.ToInt32(form.lessonNumericUpDown.Value);
            string lessonTime = form.timeComboBox.Text;
            int subjectId = form.SubjectId;
            string lessonTopic = form.topicTextBox.Text;
            string homework = form.homeworkTextBox.Text;
            int classId = form.ClassId;
            int teacherId = form.TeacherId;
            int cabinet = Convert.ToInt32(form.cabinetNumericUpDown.Value);
            

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    
                    command.Parameters.AddWithValue("@lesson_date", lessonDate);
                    command.Parameters.AddWithValue("@lesson_number", lessonNumber);
                    command.Parameters.AddWithValue("@lesson_time", lessonTime);
                    command.Parameters.AddWithValue("@subject_id", subjectId);
                    command.Parameters.AddWithValue("@lesson_topic", lessonTopic);
                    command.Parameters.AddWithValue("@homework", homework);
                    command.Parameters.AddWithValue("@class_id", classId);
                    command.Parameters.AddWithValue("@teacher_id", teacherId);
                    command.Parameters.AddWithValue("@cabinet", cabinet);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                AddForm viewForm = new AddForm();
                GetLessonsDataToForm(viewForm);
                // Сделать недоступными для редактирования
                ElementsSettings(viewForm);
                viewForm.Show();
            }
            else
            {
                MessageBox.Show("Вы не выбрали строку для просмотра!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
               
        }

        private static void ElementsSettings(AddForm viewForm)
        {
            viewForm.dateTimePicker.Enabled = false; // Lesson_number
            viewForm.timeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            viewForm.timeComboBox.DropDownHeight = 1; // Lesson_time
            viewForm.subjectTextBox.ReadOnly = true; // Subject
            viewForm.classTextBox.ReadOnly = true; // ClassName
            viewForm.lastNameTextBox.ReadOnly = true; // TeacherLName
            viewForm.firstNameTextBox.ReadOnly = true; // TeacherFName
            viewForm.surnameTextBox.ReadOnly = true; // TeacherSurname
            viewForm.cabinetNumericUpDown.ReadOnly = true; // Cabinet
            viewForm.topicTextBox.ReadOnly = true; // Lesson_topic
            viewForm.homeworkTextBox.ReadOnly = true; // Homework
            viewForm.btnSave.Enabled = false;
        }

        private void GetLessonsDataToForm(AddForm form)
        {
            form.dateTimePicker.Value = Convert.ToDateTime(dataGridView1.CurrentRow.Cells[0].Value); // Lesson_date
            form.lessonNumericUpDown.Value = Convert.ToInt32(dataGridView1.CurrentRow.Cells[1].Value); // Lesson_number
            form.timeComboBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value); // Lesson_time
            form.subjectTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[3].Value); // Subject
            form.classTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[6].Value); // ClassName
            form.lastNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[7].Value); // TeacherLName
            form.firstNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[8].Value); // TeacherFName
            form.surnameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[9].Value); // TeacherSurname
            form.cabinetNumericUpDown.Value = Convert.ToInt32(dataGridView1.CurrentRow.Cells[10].Value); // Cabinet
            form.topicTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[4].Value); // Lesson_topic
            form.homeworkTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[5].Value); // Homework
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить выделенное занятие?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    int deleteId = LessonsList[rowIndex].Id;
                    using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                    {
                        connection.Open();

                        string query = "DELETE FROM lessons WHERE id = @Id";

                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", deleteId);
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }
                    LoadLessonsData();
                }                
            }
            else
            {
                MessageBox.Show("Вы не выбрали строку для удаления!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                AddForm editForm = new AddForm();
                GetLessonsDataToForm(editForm);               
                editForm.ShowDialog();
                if (editForm.dialogResult == DialogResult.Yes)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    int updateId = LessonsList[rowIndex].Id;
                    string updateQuery = "UPDATE lessons SET lesson_date = DATE(@lesson_date)," +
                        "lesson_number = @lesson_number, lesson_time = @lesson_time, subject_id = @subject_id," +
                        "lesson_topic = @lesson_topic, homework = @homework, class_id = @class_id, " +
                        "teacher_id = @teacher_id, cabinet = @cabinet ";
                    updateQuery += $"WHERE id = {updateId}";
                    FullRowUpdater(editForm, updateQuery);
                    LoadLessonsData();
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали строку для редактирования!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (searchKeyTextBox.Text != "" && searchParamComboBox.SelectedItem != null)
            {
                LessonsList.Clear();
                string searchGroup = searchParamComboBox.Text;
                string searchText = searchKeyTextBox.Text;
                string searchQuery = "";
                bool isFounded = false;
                
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                {
                    connection.Open();
                    if (searchGroup == "Класс")
                    {
                        int classId;
                        string classIdQuery = "SELECT id FROM classes WHERE class = @class";
                        using (SQLiteCommand findClassId = new SQLiteCommand(classIdQuery, connection))
                        {
                            findClassId.Parameters.AddWithValue("@class", searchText);
                            object result = findClassId.ExecuteScalar();
                            if (result != null)
                            {
                                classId = Convert.ToInt32(result);
                                searchQuery = $"WHERE class_id = {classId}";
                                isFounded = true;
                            }                            
                        }
                    }
                    else if (searchGroup == "Предмет")
                    {
                        int subjectId;
                        string subjectIdQuery = "SELECT id FROM school_subjects WHERE subject = @subject";
                        using (SQLiteCommand findSubId = new SQLiteCommand(subjectIdQuery, connection))
                        {
                            findSubId.Parameters.AddWithValue("@subject", searchText);
                            object result = findSubId.ExecuteScalar();
                            if (result != null)
                            {
                                subjectId = Convert.ToInt32(result);
                                searchQuery = $"WHERE subject_id = {subjectId}";
                                isFounded = true;
                            }                            
                        }
                    }
                    else if (searchGroup == "Дата")
                    {
                        DateTime date = DateTime.ParseExact(searchText, "dd.MM.yyyy", null);
                        string sqliteDateString = date.ToString("yyyy-MM-dd");
                        searchQuery = $"WHERE lesson_date = '{sqliteDateString}'";                        
                        isFounded = true;
                    }
                }
                if (isFounded)
                {
                    LoadLessonsData(searchQuery);
                }
                else
                {
                    MessageBox.Show($"Данные по ключу {searchText} не найдены", "Поиск", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Не заданы параметр поиска или ключевое значаение!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDataBase_Click(object sender, EventArgs e)
        {
            LoadLessonsData();
        }

        private void btnEstimates_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedRows[0].Index;
                int lessonId = LessonsList[rowIndex].Id;
                string className = LessonsList[rowIndex].ClassName;
                GradeForm gradesForm = new GradeForm(lessonId, className);                        
                gradesForm.infoDateTextBox.Text = Convert.ToString(LessonsList[rowIndex].Lesson_date.ToShortDateString());
                gradesForm.infoSubjectTextBox.Text = LessonsList[rowIndex].Subject;
                gradesForm.infoClassTextBox.Text = LessonsList[rowIndex].ClassName;
                gradesForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Вы не выбрали предмет для выставления оценок!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
                
        }

        private void btnClasses_Click(object sender, EventArgs e)
        {
            ClassesForm classesForm = new ClassesForm();
            classesForm.Show();
        }

        private void btnSubjects_Click(object sender, EventArgs e)
        {
            SubjectsForm subjectsForm = new SubjectsForm();
            subjectsForm.Show();
        }

        private void btnTeachers_Click(object sender, EventArgs e)
        {
            TeachersForm teachersForm = new TeachersForm();
            teachersForm.Show();
        }

        private void btnPupils_Click(object sender, EventArgs e)
        {
            PupilsForm pupilsForm = new PupilsForm();
            try
            {
                pupilsForm.Show();
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bool repeat = true;
                while (repeat)
                {
                    try
                    {
                        pupilsForm.ShowDialog();
                        repeat = false;
                    }
                    catch (MyException ex1)
                    {
                        MessageBox.Show(ex1.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult exit;
            exit = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (exit == DialogResult.Yes)
            {
                Close();
            }
        }
        public DialogResult changeUser;
        private void сменитьПользователяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeUser = MessageBox.Show("Хотите сменить пользователя?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (changeUser == DialogResult.Yes)
            {
                this.Close();
            }                      
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            UsersForm usersForm = new UsersForm();
            try
            {
                usersForm.ShowDialog();
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bool repeat = true;
                while (repeat)
                {
                    try
                    {
                        usersForm.ShowDialog();                        
                        repeat = false;
                    }
                    catch (MyException ex1)
                    {
                        MessageBox.Show(ex1.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            
        }
    }
}
