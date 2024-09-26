using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiplomaSchool
{
    public partial class AddUsersForm : Form
    {
        public AddUsersForm()
        {
            InitializeComponent();
            classLabel.Visible = false;
            classTextBox.Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public DialogResult dialogResult;
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (loginTextBox.Text != "" && passwordTextBox.Text != "" && userTypeComboBox.SelectedItem != null
                && lastNameTextBox.Text != "" && firstNameTextBox.Text != "")
            {
                dialogResult = MessageBox.Show("Сохранить данные?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void userTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (userTypeComboBox.SelectedIndex == 0)
            {
                classLabel.Visible = false;
                classTextBox.Visible = false;
            }
            else
            {
                classLabel.Visible = true;
                classTextBox.Visible = true;
            }
        }
    }
}
