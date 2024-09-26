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
    public partial class PupilsForm : Form
    {
        BindingList<Pupil> PupilsList = new BindingList<Pupil>();

        public int ClassId { get; set; }

        public PupilsForm()
        {
            InitializeComponent();
            LoadPupilsData();
            dataGridView1.DataSource = PupilsList;
        }

        private void LoadPupilsData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    PupilsList.Clear();

                    string query = "SELECT pupils.id, last_name, first_name, surname, classes.class " +
                        "FROM pupils\r\nJOIN classes ON class_id = classes.id";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Pupil pupil = new Pupil
                                    (
                                        reader.GetInt32(0), // Id
                                        reader.GetString(1), // PupilLName
                                        reader.GetString(2), // PupilFName
                                        reader.GetString(3), // PupilSurname
                                        reader.GetString(4) // ClassName
                                    );
                                PupilsList.Add(pupil);
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
            AddPupilsForm addPupils = new AddPupilsForm();           
            addPupils.ShowDialog();
            
            if (addPupils.dialogResult == DialogResult.Yes)
            {
                string lastName = Convert.ToString(addPupils.lastNameTextBox.Text);
                string firstName = Convert.ToString(addPupils.firstNameTextBox.Text);
                string surname = Convert.ToString(addPupils.surnameTextBox.Text);
                string className = Convert.ToString(addPupils.classTextBox.Text);
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                {
                    GetClassId(className, connection);
                    if (!PupilExist(lastName, firstName, surname, connection))
                    {
                        connection.Open();
                        string insertQuery = "INSERT INTO pupils (last_name, first_name, surname, class_id) " +
                            "VALUES(@last_name, @first_name, @surname, @class_id)";
                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@last_name", lastName);
                            command.Parameters.AddWithValue("@first_name", firstName);
                            command.Parameters.AddWithValue("@surname", surname);
                            command.Parameters.AddWithValue("@class_id", ClassId);
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }

                }
            }
            LoadPupilsData();
        }

        private void GetClassId(string className, SQLiteConnection connection)
        {
            connection.Open();
            string classIdQuery = "SELECT id FROM classes WHERE class = @class";
            using (SQLiteCommand findClassId = new SQLiteCommand(classIdQuery, connection))
            {
                findClassId.Parameters.AddWithValue("@class", className);
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
            connection.Close();
        }

        private bool PupilExist(string lastName, string firstName, string surname, SQLiteConnection connection)
        {
            connection.Open();
            string existQuery = "SELECT EXISTS(SELECT 1 FROM pupils WHERE last_name = @last_name" +
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

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                AddPupilsForm changePupils = new AddPupilsForm();
                changePupils.lastNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[0].Value);
                changePupils.firstNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                changePupils.surnameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
                changePupils.classTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[3].Value);
                changePupils.ShowDialog();
                if (changePupils.dialogResult == DialogResult.Yes)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    int updateId = PupilsList[rowIndex].Id;
                    string updateQuery = "UPDATE pupils SET last_name = @last_name, " +
                        "first_name = @first_name, surname = @surname, class_id = @class_id ";
                    updateQuery += $"WHERE id = {updateId}";

                    string lastName = Convert.ToString(changePupils.lastNameTextBox.Text);
                    string firstName = Convert.ToString(changePupils.firstNameTextBox.Text);
                    string surname = Convert.ToString(changePupils.surnameTextBox.Text);
                    string className = Convert.ToString(changePupils.classTextBox.Text);                    

                    using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                    {                        
                        GetClassId(className, connection);
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@last_name", lastName);
                            command.Parameters.AddWithValue("@first_name", firstName);
                            command.Parameters.AddWithValue("@surname", surname);
                            command.Parameters.AddWithValue("@class_id", ClassId);
                            command.ExecuteNonQuery();
                        }
                    }
                    LoadPupilsData();
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
                    int deleteId = PupilsList[rowIndex].Id;
                    using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                    {
                        connection.Open();

                        string query = "DELETE FROM pupils WHERE id = @Id";

                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", deleteId);
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }
                    LoadPupilsData();
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали строку для удаления!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
