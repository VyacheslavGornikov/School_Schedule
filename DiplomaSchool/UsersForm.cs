using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiplomaSchool
{
    public partial class UsersForm : Form
    {
        BindingList<User> UsersList = new BindingList<User>();
        public UsersForm()
        {
            InitializeComponent();
            LoadUsersData();
            dataGridView1.DataSource = UsersList;
        }

        public int TeacherId { get; set; }
        public int PupilId { get; set; }
        
        private void LoadUsersData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    UsersList.Clear();

                    string query = "SELECT * FROM Users";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader.GetInt32(0) != 1)
                                {
                                    User user = new User
                                    (
                                        reader.GetInt32(0), // Id
                                        reader.GetString(1), // Login
                                        reader.GetString(2), // Password
                                        reader.GetString(3), // UserType
                                        null, // UserLName
                                        null, // UserFName
                                        null // UserSurname
                                    );                                    
                                    string selectQuery = "";
                                    if (user.UserType == "Учитель")
                                    {
                                        int teacher_id = reader.GetInt32(5);
                                        selectQuery = "SELECT teachers.last_name, teachers.first_name, " +
                                                            "teachers.surname FROM Users\r\n" +
                                                            "JOIN teachers ON teacher_id = teachers.id " +
                                                            $"WHERE teachers.id = {teacher_id}";                                        
                                    }
                                    else if (user.UserType == "Ученик")
                                    {
                                        int pupil_id = reader.GetInt32(4);
                                        selectQuery = "SELECT pupils.last_name, pupils.first_name, " +
                                                    "pupils.surname FROM Users\r\n" +
                                                    "JOIN pupils ON pupil_id = pupils.id " +
                                                    $"WHERE pupils.id = {pupil_id}";
                                    }
                                    using (SQLiteCommand command1 = new SQLiteCommand(selectQuery, connection))
                                    {
                                        using (SQLiteDataReader reader1 = command1.ExecuteReader())
                                        {
                                            if (reader1.Read())
                                            {
                                                if (reader.GetInt32(0) != 1)
                                                {
                                                    user.UserLName = reader1.GetString(0);
                                                    user.UserFName = reader1.GetString(1);
                                                    user.UserSurname = reader1.GetString(2);
                                                }
                                            }
                                        }
                                    }
                                    UsersList.Add(user);
                                }                                
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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddUsersForm addUsers = new AddUsersForm();
            try
            {
                addUsers.ShowDialog();
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bool repeat = true;
                while (repeat)
                {
                    try
                    {
                        addUsers.ShowDialog();
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

            if (addUsers.dialogResult == DialogResult.Yes)
            {
                string login = addUsers.loginTextBox.Text;
                string password = addUsers.passwordTextBox.Text;
                string userType = addUsers.userTypeComboBox.Text;
                string lastName = addUsers.lastNameTextBox.Text;
                string firstName = addUsers.firstNameTextBox.Text;
                string surname = addUsers.surnameTextBox.Text;
                string className = addUsers.classTextBox.Text;
                //string query = "";
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                {                    
                    if (userType == "Учитель")
                    {
                        if (!TeacherExist(lastName, firstName, surname, connection))
                        {
                            InsertNewTeacher(lastName, firstName, surname, connection);
                        }
                        GetTeacherId(lastName, firstName, surname, connection);
                        InsertTeacherToUser(login, password, userType, connection);

                    }
                    else if (userType == "Ученик")
                    {
                        if (!PupilExist(lastName, firstName, surname, connection))
                        {
                            InsertNewPupil(lastName, firstName, surname, className, connection);
                        }
                        GetPupilId(lastName, firstName, surname, connection);
                        InsertPupilToUsers(login, password, userType, connection);
                    }                   
                }                             
            }
            LoadUsersData();
        }

        private void InsertPupilToUsers(string login, string password, string userType, SQLiteConnection connection)
        {
            connection.Open();
            string query = "INSERT INTO Users (login, password, user_type, pupil_id) " +
                                        "VALUES(@login, @password, @user_type, @pupil_id) ";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@user_type", userType);
                command.Parameters.AddWithValue("@pupil_id", PupilId);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        private static void InsertNewPupil(string lastName, string firstName, string surname, string className, SQLiteConnection connection)
        {
            connection.Open();
            int classId = GetClassId(className, connection); 
            string insertPupil = "INSERT INTO pupils (last_name, first_name, surname, class_id) " +
                                            "VALUES(@last_name, @first_name, @surname, @class_id)";
            using (SQLiteCommand command = new SQLiteCommand(insertPupil, connection))
            {
                command.Parameters.AddWithValue("@last_name", lastName);
                command.Parameters.AddWithValue("@first_name", firstName);
                command.Parameters.AddWithValue("@surname", surname);
                command.Parameters.AddWithValue("@class_id", classId);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        private static int GetClassId(string className, SQLiteConnection connection)
        {
            string classIdQuery = "SELECT id FROM classes WHERE class = @class";
            using (SQLiteCommand findClassId = new SQLiteCommand(classIdQuery, connection))
            {
                findClassId.Parameters.AddWithValue("@class", className);
                object result = findClassId.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    throw new MyException("Такого класса не существует!\nПовторите ввод или обратитесь к администартору");                    
                }
            }
        }

        private static void InsertNewTeacher(string lastName, string firstName, string surname, SQLiteConnection connection)
        {
            connection.Open();
            string insertTeacher = "INSERT INTO teachers (last_name, first_name, surname) " +
                "VALUES(@last_name, @first_name, @surname)";
            using (SQLiteCommand command = new SQLiteCommand(insertTeacher, connection))
            {
                command.Parameters.AddWithValue("@last_name", lastName);
                command.Parameters.AddWithValue("@first_name", firstName);
                command.Parameters.AddWithValue("@surname", surname);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        private void InsertTeacherToUser(string login, string password, string userType, SQLiteConnection connection)
        {
            connection.Open();
            string query = "INSERT INTO Users (login, password, user_type, teacher_id) " +
                "VALUES(@login, @password, @user_type, @teacher_id)";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@user_type", userType);
                command.Parameters.AddWithValue("@teacher_id", TeacherId);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        private void GetPupilId(string lastName, string firstName, string surname, SQLiteConnection connection)
        {
            connection.Open();
            string idQuery = "SELECT id FROM pupils WHERE last_name = @last_name " +
                                            "AND first_name = @first_name AND surname = @surname";
            using (SQLiteCommand command = new SQLiteCommand(idQuery, connection))
            {
                command.Parameters.AddWithValue("@last_name", lastName);
                command.Parameters.AddWithValue("@first_name", firstName);
                command.Parameters.AddWithValue("@surname", surname);

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    PupilId = Convert.ToInt32(result);
                }
            }
            connection.Close();
        }

        private void GetTeacherId(string lastName, string firstName, string surname, SQLiteConnection connection)
        {
            connection.Open();
            string idQuery = "SELECT id FROM teachers WHERE last_name = @last_name " +
                                            "AND first_name = @first_name AND surname = @surname";
            using (SQLiteCommand command = new SQLiteCommand(idQuery, connection))
            {
                command.Parameters.AddWithValue("@last_name", lastName);
                command.Parameters.AddWithValue("@first_name", firstName);
                command.Parameters.AddWithValue("@surname", surname);

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    TeacherId = Convert.ToInt32(result);
                }
            }
            connection.Close();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            AddUsersForm addUsers = new AddUsersForm();
            GetDataToForm(addUsers);
            ElementsSettings(addUsers);
            addUsers.ShowDialog();
            if (addUsers.dialogResult == DialogResult.Yes)
            {
                int rowIndex = dataGridView1.SelectedRows[0].Index;
                int updateId = UsersList[rowIndex].Id;
                string updateQuery = "UPDATE Users SET login = @login, password = @password ";
                updateQuery += $"WHERE id = {updateId}";

                string login = addUsers.loginTextBox.Text;
                string password = addUsers.passwordTextBox.Text;
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);                        
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                LoadUsersData();
            }
        }

        private static void ElementsSettings(AddUsersForm addUsers)
        {
            addUsers.userTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            addUsers.userTypeComboBox.DropDownHeight = 1;
            addUsers.lastNameTextBox.ReadOnly = true;
            addUsers.firstNameTextBox.ReadOnly = true;
            addUsers.surnameTextBox.ReadOnly = true;
        }

        private void GetDataToForm(AddUsersForm form)
        {
            form.loginTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
            form.passwordTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[2].Value);
            form.userTypeComboBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[3].Value);
            form.lastNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[4].Value);
            form.firstNameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[5].Value);
            form.surnameTextBox.Text = Convert.ToString(dataGridView1.CurrentRow.Cells[6].Value);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить пользователя?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    int rowIndex = dataGridView1.SelectedRows[0].Index;
                    int deleteId = UsersList[rowIndex].Id;
                    using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                    {
                        connection.Open();

                        string query = "DELETE FROM Users WHERE id = @Id";

                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", deleteId);
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }
                    LoadUsersData();
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали строку для удаления!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
