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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

         
        private void button1_Click(object sender, EventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordTextBox.Text;

            if (login != "" && password != "")
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                {

                    if (UserExist(login, password, connection))
                    {
                        string userType = GetUserType(login, password, connection);
                        string query = "";
                        if (userType == "Учитель")
                        {
                            query = "SELECT Users.id, login, password, user_type, teachers.last_name, " +
                                "teachers.first_name, teachers.surname FROM Users " +
                                "JOIN teachers ON Users.teacher_id = teachers.id " +
                                "WHERE login = @login AND password = @password ";
                        }
                        else if (userType == "Ученик")
                        {
                            query = "SELECT Users.id, login, password, user_type, pupils.last_name, " +
                                "pupils.first_name, pupils.surname FROM Users " +
                                "JOIN pupils ON Users.pupil_id = pupils.id " +
                                "WHERE login = @login AND password = @password ";
                        }
                        else if (userType == "Администратор")
                        {
                            query = "SELECT Users.id, login, password, user_type FROM Users " +
                                "WHERE login = @login AND password = @password ";
                        }                        
                        
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@login", login);
                            command.Parameters.AddWithValue("@password", password);
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Program.currentUser.Id = reader.GetInt32(0);
                                    Program.currentUser.Login = reader.GetString(1);
                                    Program.currentUser.Password = reader.GetString(2);
                                    Program.currentUser.UserType = reader.GetString(3);
                                    if (userType != "Администратор")
                                    {
                                        Program.currentUser.UserLName = reader.GetString(4);
                                        Program.currentUser.UserFName = reader.GetString(5);
                                        Program.currentUser.UserSurname = reader.GetString(6);
                                    }                                    
                                }
                            }
                        }
                        connection.Close();                        
                        
                        this.Hide();
                        Form1 form1 = new Form1();
                        form1.ShowDialog();
                        if (form1.changeUser == DialogResult.Yes)
                        {
                            loginTextBox.Text = "";
                            passwordTextBox.Text = "";
                            this.Show();
                        }
                        else
                        {
                            Close();
                        }

                    }
                    else
                    {
                        MessageBox.Show("Неверные логин или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }            
            else
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);            
            }   
        }

        private string GetUserType(string login, string password, SQLiteConnection connection)
        {
            string userType = null;
            connection.Open();
            string query = "SELECT user_type FROM Users WHERE login = @login AND password = @password";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userType = reader.GetString(0); // Получаем значение UserType
                    }
                }
            }
            connection.Close();
            return userType;
        }

        private bool UserExist(string login, string password, SQLiteConnection connection)
        {
            connection.Open();
            string existQuery = "SELECT EXISTS(SELECT 1 FROM users WHERE login = @login" +
                " AND password = @password)";
            using (SQLiteCommand command = new SQLiteCommand(existQuery, connection))
            {
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);                
                int result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return result != 0;
            }
        }
    }
}
