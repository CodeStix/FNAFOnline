using Stx.Net;
using Stx.Net.RoomBased;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StxClientTest
{
    public partial class FormChat : Form
    {
        private Client client;
        private ChatSourceType sourceType = ChatSourceType.Room;
        private ServerFunctions server;

        public FormChat(Client client)
        {
            this.client = client;
            server = new ServerFunctions(client);

            InitializeComponent();

            client.OnChat += Client_OnChat;

            comboBoxSource.SelectedIndex = 1;
        }

        private void buttonSendChat_Click(object sender, EventArgs e)
        {
            string message = textBoxMessage.Text;

            statusLabel.ForeColor = Color.Black;
            statusLabel.Text = "Sending...";

            textBoxMessage.Text = "";

            switch (comboBoxSource.SelectedItem.ToString())
            {
                case "Personal":
                    server.ChatPersonalAsync(message, textBoxReceiver.Text, (state) =>
                    {
                        DisplayPacketStatus(state);
                    });
                    break;

                case "Room":
                    server.ChatInRoomAsync(message, (state) =>
                    {
                        DisplayPacketStatus(state);
                    });
                    break;

                case "Global":
                    server.ChatGloballyAsync(message, (state) =>
                    {
                        DisplayPacketStatus(state);
                    });
                    break;

                default:
                    statusLabel.Text = "Chat type not supported.";
                    return;
            }
        }

        private void DisplayPacketStatus(PacketResponseStatus state)
        {
            BeginInvoke(new Action(() =>
            {
                if (state == PacketResponseStatus.Responded)
                    statusLabel.ForeColor = Color.Green;
                else
                    statusLabel.ForeColor = Color.Red;

                statusLabel.Text = state.ToString();
            }));
        }

        private void Client_OnChat(ChatEntry chatMessage)
        {
            BeginInvoke(new Action(() =>
            {
                if (chatMessage.SourceType == sourceType)
                    listBoxChat.Items.Add(chatMessage.ToString());
            }));
        }

        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.OnChat -= Client_OnChat;
        }

        private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            sourceType = (ChatSourceType)comboBoxSource.SelectedIndex;

            listBoxChat.Items.Add($"----- { sourceType.ToString() } Chat -----");
            statusLabel.ForeColor = Color.Black;
            statusLabel.Text = "Changed source to " + sourceType;

            buttonSendChat.Enabled = false;

            label2.Enabled = comboBoxSource.SelectedIndex == 0;
            textBoxReceiver.Enabled = comboBoxSource.SelectedIndex == 0;
        }

        private void textBoxMessage_TextChanged(object sender, EventArgs e)
        {
            buttonSendChat.Enabled = !string.IsNullOrWhiteSpace(textBoxMessage.Text) && 
                (comboBoxSource.SelectedIndex != 0 || !string.IsNullOrWhiteSpace(textBoxReceiver.Text));
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            listBoxChat.Items.Clear();
        }

        private void textBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            { 
                if (buttonSendChat.Enabled)
                    buttonSendChat.PerformClick();
            }
        }
    }
}
