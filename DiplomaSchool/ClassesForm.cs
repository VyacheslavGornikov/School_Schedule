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
    public partial class ClassesForm : Form
    {
        BindingList<Class> ClassesList = new BindingList<Class>(); 
        public ClassesForm()
        {
            InitializeComponent();
            LoadClassesData();
            dataGridView1.DataSource = ClassesList;
        }

        private void LoadClassesData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    ClassesList.Clear();
                    
                    string query = "SELECT * FROM classes";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Class className = new Class
                                    (
                                        reader.GetInt32(0), // Id
                                        reader.GetString(1) // ClassName                                                                               
                                    );
                                ClassesList.Add(className);
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
                string className = Convert.ToString(addSingle.valueTextBox.Text);
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                {
                    if (!ClassExist(className, connection))
                    {
                        connection.Open();
                        string insertQuery = "INSERT INTO classes (class) VALUES(@class)";
                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@class", className);
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                       
                }
            }
            LoadClassesData();            
        }

        private bool ClassExist(string className, SQLiteConnection connection)
        {
            connection.Open();
            string existQuery = "SELECT EXISTS(SELECT 1 FROM classes WHERE class = @class)";
            using (SQLiteCommand command = new SQLiteCommand(existQuery, connection))
            {
                command.Parameters.AddWithValue("@class", className);                
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
                    int updateId = ClassesList[rowIndex].Id;
                    string updateQuery = "UPDATE classes SET class = @class ";
                    updateQuery += $"WHERE id = {updateId}";
                    
                    string className = Convert.ToString(addSingle.valueTextBox.Text);
                    

                    using (SQLiteConnection connection = new SQLiteConnection(ConnectionParameters.ConnectionString()))
                    {
                        connection.Open();

                        using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                        {                            
                            command.Parameters.AddWithValue("@class", className);                  
                            command.ExecuteNonQuery();
                        }
                    }

                    LoadClassesData();
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали строку!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
