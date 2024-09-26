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
    public partial class GradeForm : Form
    {
        BindingList<Grade> GradesList = new BindingList<Grade>();
        List<int> pupilsIndexes = new List<int>();
        public GradeForm(int lessonIndex, string className)
        {
            InitializeComponent();
            infoClassTextBox.ReadOnly = true;
            infoDateTextBox.ReadOnly = true;
            infoSubjectTextBox.ReadOnly = true;
            LessonId = lessonIndex;
            ClassName = className;
            InsertPupilsToDataBase();
            LoadGradesData();
            dataGridView1.DataSource = GradesList;
            UserAccessSettings();
        }

        private void UserAccessSettings()
        {
            if (Program.currentUser.UserType == "Ученик")
            {
                btnGrade.Visible = false;
                операцииToolStripMenuItem.Visible = false;
            }
        }

        public int LessonId { get; set; }
        public string ClassName { get; set; }
        private void InsertPupilsToDataBase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                

                GetPupilsIndexes(connection);
                foreach (int pupilId in  pupilsIndexes)
                {
                    if (!RecordExistsForPupil(pupilId, connection))
                    {
                        
                        string insertQuery = "INSERT INTO class_journal (pupil_id, lesson_id)" +
                            "VALUES (@pupil_id, @lesson_id)";
                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@pupil_id", pupilId);
                            command.Parameters.AddWithValue("@lesson_id", LessonId);
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        
                    }
                }
            }
        }

        private bool RecordExistsForPupil(int pupilId, SQLiteConnection connection)
        {
            connection.Open();
            string existQuery = "SELECT EXISTS(SELECT 1 FROM class_journal WHERE pupil_id = @id " +
                "AND lesson_id = @lessonId)";
            using (SQLiteCommand command = new SQLiteCommand(existQuery, connection))
            {
                command.Parameters.AddWithValue("@id", pupilId);
                command.Parameters.AddWithValue("@lessonId", LessonId);
                int result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return result != 0;
            }            
        }

        private void GetPupilsIndexes(SQLiteConnection connection)
        {
            connection.Open();
            string query = "SELECT pupils.id FROM pupils \r\n" +
                                "JOIN classes ON class_id = classes.id\r\nWHERE classes.class = @class";
            using (SQLiteCommand findIndexes = new SQLiteCommand(query, connection))
            {
                findIndexes.Parameters.AddWithValue("@class", ClassName);
                using (SQLiteDataReader reader = findIndexes.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pupilsIndexes.Add(reader.GetInt32(0));
                    }
                }
            }
            connection.Close();
        }

        

        private void LoadGradesData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    GradesList.Clear();
                    
                    string query = "SELECT class_journal.id, pupils.last_name, " +
                        "pupils.first_name, pupils.surname, quarter,\r\nlesson_grade, " +
                        "homework_grade FROM class_journal\r\n" +
                        "JOIN lessons ON lesson_id = lessons.id\r\n" +
                        "JOIN pupils ON pupil_id = pupils.id\r\n" +
                        "JOIN classes ON pupils.class_id = classes.id\r\n" +
                        $"WHERE lesson_id = {LessonId} AND classes.class = '{ClassName}'";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Grade grade = new Grade
                                    (
                                        reader.GetInt32(0), // Id
                                        reader.GetString(1), // PupilLName
                                        reader.GetString(2), // PupilFName
                                        reader.GetString(3), // PupilSurname
                                        reader.GetInt32(4), // Quarter
                                        reader.GetString(5), // Lesson_grade
                                        reader.GetInt32(6) // Homework_grade                                        
                                    );
                                GradesList.Add(grade);
                            }
                        }
                    }
                    connection.Close();
                    dataGridView1.ClearSelection();
                }

            }
        }

        private void btnGrade_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                ChangeGradeForm changeGrade = new ChangeGradeForm();
                GetGradesDataToForm(changeGrade);
                changeGrade.ShowDialog();
                if (changeGrade.dialogResult == DialogResult.Yes)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    int updateId = GradesList[rowIndex].Id;
                    string updateQuery = "UPDATE class_journal SET quarter = @quarter," +
                        "lesson_grade = @lesson_grade, homework_grade = @homework_grade ";
                    updateQuery += $"WHERE id = {updateId}";

                    RowUpdater(changeGrade, updateQuery);
                    LoadGradesData();
                }
            }
        }

        private static void RowUpdater(ChangeGradeForm form, string query)
        {
            int quarter = Convert.ToInt32(form.quarterComboBox.SelectedItem);
            string lessonGrade = Convert.ToString(form.gradeLessonComboBox.SelectedItem);
            int homeworkGrade = Convert.ToInt32(form.homeworkComboBox.SelectedItem);

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@quarter", quarter);
                    command.Parameters.AddWithValue("@lesson_grade", lessonGrade);
                    command.Parameters.AddWithValue("@homework_grade", homeworkGrade);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void GetGradesDataToForm(ChangeGradeForm form)
        {
            form.lastNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[0].Value);
            form.firstNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
            form.surnameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
            form.quarterComboBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[3].Value);
            form.gradeLessonComboBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[4].Value);
            form.homeworkComboBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[5].Value);
        }
    }
}
