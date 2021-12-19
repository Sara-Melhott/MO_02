using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MO_02
{
    public partial class Form5 : Form
    {
        Task task;
        public Form5(Task task)
        {
            this.task = task;
            InitializeComponent();
        }
        /// <summary>
        /// Кнопка "В главное меню"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
