using Stx.Net.RoomBased;
using System;
using System.Windows.Forms;

namespace StxClientTest
{
    public partial class FormCreateRoom : Form
    {
        public RoomTemplate RoomTemplate { get; private set; } = null;

        public FormCreateRoom()
        {
            InitializeComponent();
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            RoomTemplate = new RoomTemplate(textBoxName.Text, (int)numericMaxPlayers.Value, checkBoxLocked.Checked ? textBoxPassword.Text : string.Empty);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void checkBoxLocked_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPassword.Enabled = checkBoxLocked.Checked;

            if (!checkBoxLocked.Checked)
                textBoxPassword.Text = "";
        }

        private void FormCreateRoom_Load(object sender, EventArgs e)
        {

        }
    }
}
