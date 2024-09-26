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
    public partial class AddSingleForm : Form
    {
        public AddSingleForm()
        {
            InitializeComponent();
        }
        public DialogResult dialogResult;
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (valueTextBox.Text != "")
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
