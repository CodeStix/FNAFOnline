using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StxClientTest
{
    public partial class FormListSelect : Form
    {
        private List<object> list = new List<object>();

        public object Selected { get; private set; } = null;

        public FormListSelect(List<object> toList)
        {
            this.list = toList;

            InitializeComponent();
        }

        private void FormListSelect_Load(object sender, EventArgs e)
        {
            for(int i = 0; i < list.Count; i++)
            {
                listBox1.Items.Add($"{ i }: " + list[i]);
            }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            Selected = list[listBox1.SelectedIndex];
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            buttonSelect.Enabled = listBox1.SelectedIndex >= 0;
        }
    }
}
