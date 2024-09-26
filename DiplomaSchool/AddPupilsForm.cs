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
    public partial class AddPupilsForm : Form
    {
        public AddPupilsForm()
        {
            InitializeComponent();
        }
        public DialogResult dialogResult;
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lastNameTextBox.Text != "" && firstNameTextBox.Text != "" && classTextBox.Text != "")
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
    }
}
