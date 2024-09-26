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
    public partial class SubjectsForm : Form
    {
        BindingList<Subject> SubjectsList = new BindingList<Subject>();
        public SubjectsForm()
        {
            InitializeComponent();
            LoadSubjectsData();
            dataGridView1.DataSource = SubjectsList;
        }

        private void LoadSubjectsData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    SubjectsList.Clear();

                    string query = "SELECT * FROM school_subjects";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Subject subject = new Subject
                                    (
                                        reader.GetInt32(0), // Id
                                        reader.GetString(1) // ClassName                                                                               
                                    );
                                SubjectsList.Add(subject);
                            }
                        }
                    }
                    connection.Close();
                    dataGridView1.ClearSelection();
                }

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddSingleForm addSingle = new AddSingleForm();
            addSingle.ShowDialog();
            if (addSingle.dialogResult == DialogResult.Yes)
            {
                string subjectName = Convert.ToString(addSingle.valueTextBox.Text);
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                {
                    if (!SubjectExist(subjectName, connection))
                    {
                        connection.Open();
                        string insertQuery = "INSERT INTO school_subjects (subject) VALUES(@subject)";
                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@subject", subjectName);
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }

                }
            }
            LoadSubjectsData();
        }

        private bool SubjectExist(string subjectName, SQLiteConnection connection)
        {
            connection.Open();
            string existQuery = "SELECT EXISTS(SELECT 1 FROM school_subjects WHERE subject = @subject)";
            using (SQLiteCommand command = new SQLiteCommand(existQuery, connection))
            {
                command.Parameters.AddWithValue("@subject", subjectName);
                int result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return result != 0;
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                AddSingleForm addSingle = new AddSingleForm();
                addSingle.valueTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[0].Value);
                addSingle.ShowDialog();
                if (addSingle.dialogResult == DialogResult.Yes)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    int updateId = SubjectsList[rowIndex].Id;
                    string updateQuery = "UPDATE school_subjects SET subject = @subject ";
                    updateQuery += $"WHERE id = {updateId}";

                    string subjectName = Convert.ToString(addSingle.valueTextBox.Text);


                    using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                    {
                        connection.Open();

                        using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@subject", subjectName);
                            command.ExecuteNonQuery();
                        }
                    }

                    LoadSubjectsData();
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали строку!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
