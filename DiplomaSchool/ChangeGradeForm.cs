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
    public partial class ChangeGradeForm : Form
    {
        public ChangeGradeForm()
        {
            InitializeComponent();
            lastNameTextBox.ReadOnly = true;
            firstNameTextBox.ReadOnly = true;
            surnameTextBox.ReadOnly = true;
        }

        internal DialogResult dialogResult;
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lastNameTextBox.Text != "" && firstNameTextBox.Text != "" && surnameTextBox.Text != ""
                && quarterComboBox.Text != "0" && gradeLessonComboBox.Text != "0")
            {                
                dialogResult = MessageBox.Show("Сохранить введенные оценки?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes) // при сохранении закрываем форму
                    Close();
            }
            else
            {
                MessageBox.Show("Четверть и оценка за урок не должны быть 0!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
