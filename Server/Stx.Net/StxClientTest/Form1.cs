using Ookii.Dialogs;
using Stx.Logging;
using Stx.Net;
using Stx.Utilities;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using Stx.Net.RoomBased;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace StxClientTest
{
    public partial class FormMain : Form, ILogger
    {
        private Client client;
        private ServerFunctions server;

        private Packet currentPacket;

        public FormMain()
        {
            ThreadSafeData.MultiThreadOverride = false;
            StxNet.DefaultLogger = this;
            StxNet.ImgurApplicationClientId = "50dac57b4589f6d";
            StxNet.ImgurApplicationClientSecret = "523c33170f952d5af8464c4f00e51c44743efe46";

            InitializeComponent();

            listRequests.ShowGroups = true;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ThreadSafeData.MultiThreadOverride = false;

            FormConnect fc = new FormConnect();

            if (fc.ShowDialog() != DialogResult.OK)
            {
                Environment.Exit(0);
                return;
            }

            //DebugHandler.OnError += ErrorHandler_OnError;

            ConnectionStartInfo csi = fc.ConnectInfo.GetStartInfo();
            csi.ConnectOnConstruct = false;
            client = new Client(csi);

            client.OnReceived += Client_OnReceive;
            client.OnConnected += Client_Connected;
            client.OnDisconnected += Client_OnDisconnected;
            client.OnUpdateRequired += Client_OnUpdateRequired;
            client.OnCannotConnect += Client_OnCannotConnect;
            client.OnAnnouncement += Client_OnAnnouncement;
            client.PacketCompleter = Client_PacketCompleter;

            server = new ServerFunctions(client);
            currentPacket = new Packet(client.NetworkID);

            labelClientID.Text = "Your ClientID: " + client.NetworkID;
            comboBoxFieldType.SelectedIndex = 2;

            client.ConnectAsync();
        }

        private void Client_PacketCompleter(RequestPacket forPacket, string requiredKey, Type requiredKeyType, Action<bool> submit)
        {
            if (requiredKeyType == typeof(string))
            {
                string input = "";
                if (AskString(out input, "Packet completion", $"Packet requires key ({ requiredKeyType }){ requiredKey },\nplease specify its value:"))
                {
                    forPacket.Data.Add(requiredKey, input);

                    submit.Invoke(true);
                    return;
                }
                else
                {
                    submit.Invoke(false);
                    return;
                }
            }
            else
            {
                BeginInvoke(new Action(() =>
                {
                    MessageBox.Show($"Received request for packet completion but required type({ requiredKeyType }) was not supported!\n{ forPacket.ToString() }", "Packet completion problem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }));

                submit.Invoke(false);
                return;
            }
        }

        private void Client_OnAnnouncement(string message)
        {
            BeginInvoke(new Action(() =>
            {
                MessageBox.Show("Server announcement:\n" + message, "Announcement", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void Client_OnCannotConnect(Exception ex)
        {
            BeginInvoke(new Action(() =>
            {
                MessageBox.Show("Cannot connect to the server:\n" + ex.Message, "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        private void Client_OnUpdateRequired(string updateDownloadLocation)
        {
            BeginInvoke(new Action(() =>
            {
                MessageBox.Show("You have to download the new version:\n" + updateDownloadLocation, "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }));
        }

        private void Client_OnDisconnected(DisconnectReason? reason)
        {
            BeginInvoke(new Action(() => 
            {
                MessageBox.Show("You have been disconnected from the server! Reason: " + reason, "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        private void Client_Connected(bool firstTime)
        {
            BeginInvoke(new Action(() => buttonDisconnect.Enabled = true));

            Log("Connection succesful; FirstTime = " + firstTime, LoggedImportance.Information);
        }

        private void Client_OnReceive(Packet p)
        {
            //AddIncoming($"Received data: ");
            //foreach (string key in p.Data.Keys)
            //{
            //    AddIncoming($"\t{ key } = { p.Data[key] }");
            //}
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(client.NetworkID);
        }

        private bool AskString(out string input, string windowTitle, string description)
        {
            InputDialog i2 = new InputDialog()
            {
                WindowTitle = windowTitle,
                MainInstruction = description
            };
            i2.ShowDialog();
            input = i2.Input;
            return !string.IsNullOrEmpty(i2.Input);
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBoxData.SelectedIndex >= 0)
            {
                string value = listBoxData.Items[listBoxData.SelectedIndex].ToString();
                value = value.Substring(0, value.IndexOf(':'));
                currentPacket.Data.Remove(value);

                listBoxData.Items.RemoveAt(listBoxData.SelectedIndex); 
            }
            else
            {
                listBoxData.Items.Clear();
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            textBoxRequest.Text = "Matchmaking";
            comboBoxFieldType.SelectedIndex = 2;
            textBoxFieldKey.Text = "Query";
            listBoxData.Items.Clear();
            listRequests.Items.Clear();
            numericSendCount.Value = 1;

            currentPacket = new Packet(client.NetworkID);
        }

        private static int requestID = 0;

        public void SendRequest()
        {
            var data = currentPacket.Data;
            int current = requestID++;

            RequestPacket rp = new RequestPacket(client.NetworkID, textBoxRequest.Text, (obj) => RequestPacket.RequestPacketStatus<object>(obj, (state, o) =>
            {
                int c = current;

                BeginInvoke(new Action(() =>
                {
                    var v = listRequests.Groups[0].Items[c];

                    if (state == PacketResponseStatus.Okey || state == PacketResponseStatus.Responded)
                        v.BackColor = System.Drawing.Color.LimeGreen;
                    else if (state == PacketResponseStatus.Failed)
                        v.BackColor = System.Drawing.Color.OrangeRed;
                    else if (state == PacketResponseStatus.UnknownRequest)
                        v.BackColor = System.Drawing.Color.Orange;

                    v.SubItems[3].Text = RequestPacket.LastSendReceiveTime.TotalMilliseconds + " ms";
                    v.SubItems[4].Text = state.ToString();
                    v.SubItems[5].Text = o?.ToString() ?? "null";
                }));
            }));
            currentPacket = rp;
            currentPacket.Data = data;

            var vv = listRequests.Items.Add(new ListViewItem(new string[] { current.ToString(), rp.RequestID, rp.RequestItemName, "∞", "Pending", "" }));
            vv.Group = listRequests.Groups[0];
            vv.BackColor = System.Drawing.Color.LightYellow;

            Send();
        }

        private void buttonRequest_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < numericSendCount.Value; i++)
                SendRequest();

            //Reset();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Send();
            //Reset();
        }

        private void Send()
        {
            client.SendToServer(currentPacket);
        }

        private void buttonAddRoom_Click(object sender, EventArgs e)
        {
            string name;

            if (AskString(out name, "Add Room Field", "Please specify room name:"))
            {
                RoomTemplate rt = new RoomTemplate(name, 4);

                currentPacket.Data.Add("RoomTemplate", rt);
                listBoxData.Items.Add($"RoomTemplate: " + rt);
            }
        }

        private void buttonLeaveRoom_Click(object sender, EventArgs e)
        {
            Room.LeaveCurrent(client, (s, obj) =>
            {
                if (s <= PacketResponseStatus.Okey)
                    Log("Left was successful", LoggedImportance.Successful);
                else
                    Log("Left failed.", LoggedImportance.Warning);
            });
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.StopConnection();
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            buttonDisconnect.Enabled = false;

            client.StopConnection();
        }

        private void checkBoxAnswerPing_CheckedChanged(object sender, EventArgs e)
        {
            client.IgnorePings = !checkBoxAnswerPing.Checked;
        }

        private void buttonAddField_Click(object sender, EventArgs e)
        {
            string type = comboBoxFieldType.SelectedItem.ToString();
            string key = textBoxFieldKey.Text;

            object value = null;

            /*
            string
            int
            MatchmakingQuery
            ClientRoomStatus
            RoomTemplate
            */

            switch (type)
            {
                case "string":
                    string str;
                    if (AskString(out str, $"Add { type } field: { key }", "Please specify value:"))
                        value = str;
                    break;

                case "int":
                    string intAsString;
                    if (AskString(out intAsString, $"Add { type } field: { key }", "Please specify value:"))
                    {
                        int i;
                        if (int.TryParse(intAsString, out i))
                            value = i;
                    }
                    break;

                case "MatchmakingQuery":
                    FormMatchmakingQuery queryForm = new FormMatchmakingQuery();
                    if (queryForm.ShowDialog() == DialogResult.OK)
                        value = queryForm.Query;
                    break;

                case "ClientRoomStatus":
                    string statusAsString;
                    if (AskString(out statusAsString, $"Add { type } field: { key }", "Please specify value:"))
                    {
                        ClientRoomStatus status;
                        if (Enum.TryParse(statusAsString, out status))
                            value = status;
                    }
                    break;

                case "RoomTemplate":
                    FormCreateRoom roomForm = new FormCreateRoom();
                    if (roomForm.ShowDialog() == DialogResult.OK)
                        value = roomForm.RoomTemplate;
                    break;

                default:
                    return;
            }

            if (value != null)
            {
                currentPacket.Data.Add(key, value);
                listBoxData.Items.Add($"{ key }: { value } ({ type })");
            }
        }

        private ClientRoomStatus nextStatus = ClientRoomStatus.Ready;

        private void button1_Click(object sender, EventArgs e)
        {
            server.ChangeInRoomStatusAsync(nextStatus, (state, newStatus) =>
            {
                BeginInvoke(new Action(() =>
                {
                    if (state == PacketResponseStatus.Responded)
                    {
                        if (newStatus == ClientRoomStatus.Ready)
                        {
                            buttonReady.Text = "Ready!";
                            buttonReady.ForeColor = Color.Green;
                            nextStatus = ClientRoomStatus.NotReady;
                        }
                        else if (newStatus == ClientRoomStatus.NotReady)
                        {
                            buttonReady.Text = "Not Ready.";
                            buttonReady.ForeColor = Color.Red;
                            nextStatus = ClientRoomStatus.Ready;
                        }
                    }
                }));
            });
        }

        private void buttonChat_Click(object sender, EventArgs e)
        {
            new FormChat(client).Show();
        }
         
        public void Log(string message, LoggedImportance importance = LoggedImportance.Information, [CallerMemberName] string caller = "Global")
        {
            BeginInvoke(new Action(() =>
            {
                var vv = listRequests.Items.Add(new ListViewItem(new string[] { "", $"[{ importance }]", caller, "∞", "", message }));
                vv.Group = listRequests.Groups[1];
                vv.BackColor = Color.Pink;
            }));
        }

        public void LogException(Exception ex, string note = null, [CallerMemberName] string caller = "Global")
        {
            MessageBox.Show($"Exception (caller: { caller }): { note }\n\n{ ex.Message }\n{ ex.StackTrace }", "Exception thrown!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            BeginInvoke(new Action(() =>
            {
                var vv = listRequests.Items.Add(new ListViewItem(new string[] { "", "[Exception]", note, "∞", caller, ex.Message }));
                vv.Group = listRequests.Groups[1];
                vv.BackColor = Color.DarkRed;
            }));
        }

        private void buttonMatchmaking_Click(object sender, EventArgs e)
        {
            FormMatchmaking form = new FormMatchmaking(client);

            form.ShowDialog();
        }

        private void textBoxRequest_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonRequest.PerformClick();
            }
        }

        private void textBoxFieldKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonAddField.PerformClick();
            }
        }

        private void buttonReconnect_Click(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);

            /*FormConnect form = new FormConnect();

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (client.Connected)
                    client.Disconnect();

                buttonDisconnect.Enabled = false;

                Thread.Sleep(500);

                client = new Client(form.ConnectInfo.GetStartInfo());
            }*/
        }

        private void buttonSetName_Click(object sender, EventArgs e)
        {
            string input = "";
            if (AskString(out input, "Set Name", "Please enter a new valid non-taken name:"))
            {
                server.SetName(input, (state) =>
                {
                    if (state == PacketResponseStatus.Okey)
                        BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show("Name was updated successfully!", "Set Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                });
            }
        }

        private void buttonSetAvatar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Please select new avatar image.";
            ofd.Filter = "*.png|*.png";
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                server.SetAndUploadAvatar(ofd.FileName, (state) =>
                {
                    if (state == PacketResponseStatus.Okey)
                        BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show("Avatar was updated successfully!", "Set Avatar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                });
            }
        }

        private async void buttonShowAvatar_Click(object sender, EventArgs e)
        {
            ClientInfo info = await server.GetClientInfo();

            if (info == null)
                return;

            if (info.HasAvatar)
            {
                Process.Start(info.AvatarUrl);
            }
            else
            {
                BeginInvoke(new Action(() =>
                {
                    MessageBox.Show("You do not have an avatar!", "Show Avatar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }));
            }

        }
    }
}
