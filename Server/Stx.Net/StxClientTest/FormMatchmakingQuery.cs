using Stx.Net.RoomBased;
using System;
using System.Windows.Forms;

namespace StxClientTest
{
    public partial class FormMatchmakingQuery : Form
    {
        public MatchmakingQuery Query { get; private set; } = null;

        public FormMatchmakingQuery()
        {
            InitializeComponent();
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            GameState? state = null;
            if (comboBoxState.SelectedItem.ToString() != "Any")
            {
                GameState gs;

                if (Enum.TryParse(comboBoxState.SelectedItem.ToString(), out gs))
                    state = gs;
            }

            Query = new MatchmakingQuery()
            {
                GameState = state,
                MatchedID = textBoxID.Text.Trim(),
                MatchedName = textBoxName.Text.Trim(),
                OnlyNotFull = checkBoxOnlyNotFull.Checked,
                OnlyUnlocked = checkBoxOnlyUnlocked.Checked,
                Page = (ushort)numericPage.Value,
                RequiredRoomCode = textBoxRoomCode.Text.Trim(),
                RequiredRoomTag = textBoxTag.Text.Trim(),
                ResultsPerPage = (ushort)numericResults.Value
            };
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormMatchmakingQuery_Load(object sender, EventArgs e)
        {
            comboBoxState.SelectedIndex = 0;
        }
    }
}
