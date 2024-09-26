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
    public partial class TeachersForm : Form
    {
        BindingList<Teacher> TeachersList = new BindingList<Teacher>();
        public TeachersForm()
        {
            InitializeComponent();
            LoadTeachersData();
            dataGridView1.DataSource = TeachersList;
        }

        private void LoadTeachersData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    TeachersList.Clear();

                    string query = "SELECT * FROM teachers";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Teacher teacher = new Teacher
                                    (
                                        reader.GetInt32(0), // Id
                                        reader.GetString(1), // TeacherLName
                                        reader.GetString(2), // TeacherFName
                                        reader.GetString(3) // TeacherSurname
                                    );
                                TeachersList.Add(teacher);
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
            AddTeachersForm addTeachers = new AddTeachersForm();
            addTeachers.ShowDialog();
            if (addTeachers.dialogResult == DialogResult.Yes)
            {
                string lastName = Convert.ToString(addTeachers.lastNameTextBox.Text);
                string firstName = Convert.ToString(addTeachers.firstNameTextBox.Text);
                string surname = Convert.ToString(addTeachers.surnameTextBox.Text);
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                {
                    if (!TeacherExist(lastName, firstName, surname, connection))
                    {
                        connection.Open();
                        string insertQuery = "INSERT INTO teachers (last_name, first_name, surname) " +
                            "VALUES(@last_name, @first_name, @surname)";
                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@last_name", lastName);
                            command.Parameters.AddWithValue("@first_name", firstName);
                            command.Parameters.AddWithValue("@surname", surname);
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }

                }
            }
            LoadTeachersData();
        }

        private bool TeacherExist(string lastName, string firstName, string surname, SQLiteConnection connection)
        {
            connection.Open();
            string existQuery = "SELECT EXISTS(SELECT 1 FROM teachers WHERE last_name = @last_name" +
                " AND first_name = @first_name AND surname = @surname)";
            using (SQLiteCommand command = new SQLiteCommand(existQuery, connection))
            {
                command.Parameters.AddWithValue("@last_name", lastName);
                command.Parameters.AddWithValue("@first_name", firstName);
                command.Parameters.AddWithValue("@surname", surname);
                int result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return result != 0;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                AddTeachersForm changeTeachers = new AddTeachersForm();
                changeTeachers.lastNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[0].Value);
                changeTeachers.firstNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                changeTeachers.surnameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                changeTeachers.ShowDialog();
                if (changeTeachers.dialogResult == DialogResult.Yes)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    int updateId = TeachersList[rowIndex].Id;
                    string updateQuery = "UPDATE teachers SET last_name = @last_name, " +
                        "first_name = @first_name, surname = @surname ";
                    updateQuery += $"WHERE id = {updateId}";

                    string lastName = Convert.ToString(changeTeachers.lastNameTextBox.Text);
                    string firstName = Convert.ToString(changeTeachers.firstNameTextBox.Text);
                    string surname = Convert.ToString(changeTeachers.surnameTextBox.Text);

                    using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                    {
                        connection.Open();

                        using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@last_name", lastName);
                            command.Parameters.AddWithValue("@first_name", firstName);
                            command.Parameters.AddWithValue("@surname", surname);
                            command.ExecuteNonQuery();
                        }
                    }
                    LoadTeachersData();
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали строку!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    int deleteId = TeachersList[rowIndex].Id;
                    using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                    {
                        connection.Open();

                        string query = "DELETE FROM teachers WHERE id = @Id";

                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", deleteId);
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }
                    LoadTeachersData();
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали строку для удаления!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
