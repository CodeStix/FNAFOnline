using Ookii.Dialogs;
using Stx.Net;
using Stx.Net.RoomBased;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StxClientTest
{
    public partial class FormMatchmaking : Form
    {
        public string ShouldJoin { get; private set; } = null;

        private MatchmakingQuery query = MatchmakingQuery.Default;
        private Client client;
        private ServerFunctions server;

        public FormMatchmaking(Client client)
        {
            this.client = client;
            server = new ServerFunctions(client);

            InitializeComponent();

            labelQuery.Text = "Current query: " + query.ToString();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonJoin_Click(object sender, EventArgs e)
        {
            server.JoinRoomAsync(ShouldJoin, (state, room) =>
            {
                BeginInvoke(new Action(() => 
                {
                    labelResults.Text = "Join: " + state.ToString();

                    if (state <= PacketResponseStatus.Okey)
                    {
                        DialogResult = DialogResult.OK;
                    }
                }));
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormMatchmakingQuery queryForm = new FormMatchmakingQuery();

            if (queryForm.ShowDialog() == DialogResult.OK)
            {
                query = queryForm.Query;
                labelQuery.Text = "Current query: " + query.ToString();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshRooms();
        }

        private void RefreshRooms()
        {
            listRooms.Items.Clear();
            buttonRefresh.Enabled = false;
            buttonChangeQuery.Enabled = false;

            server.GetMatchmakingAsync(query, (state, r) =>
            {
                BeginInvoke(new Action(() =>
                {
                    labelResults.Text = $"Matches on server: { r.MatchedRooms }\nReturned rooms: { r.Rooms.Count }\nTotal rooms on server: { r.TotalRooms }";

                    Room mostSuitable = r.MostSuitableResult;

                    foreach (Room room in r.Ordered)
                    {
                        var vv = listRooms.Items.Add(new ListViewItem(new string[] 
                        {
                            room.ID,
                            room.Name,
                            $"{ room.PlayerCount }/{ room.MaxPlayers }",
                            room.Locked ? "Yes" : "Nope",
                            room.RoomCode,
                            room.State.ToString()
                        }));

                        if (mostSuitable != null && mostSuitable == room)
                            vv.BackColor = Color.Lime;
                        if (room.IsFull)
                            vv.BackColor = Color.Orange;
                        else if (room.PlayerCount > 0)
                            vv.BackColor = Color.LightYellow;

                        if (room.State == GameState.InGame)
                            vv.ForeColor = Color.LightGray;
                    }

                    buttonRefresh.Enabled = true;
                    buttonChangeQuery.Enabled = true;

                    labelPage.Text = "Page " + query.Page;
                    buttonNextPage.Enabled = true;
                    buttonPreviousPage.Enabled = true;
                }));
            });
        }

        private void listRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonJoin.Enabled = false;
            ShouldJoin = null;

            if (listRooms.SelectedIndices.Count == 1)
            {
                var v = listRooms.Items[listRooms.SelectedIndices[0]];
                ShouldJoin = v.SubItems[0].Text;

                buttonJoin.Enabled = true;
            }
        }

        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            query = new MatchmakingQuery(query);

            labelQuery.Text = "Current query: " + query.ToString();
            buttonPreviousPage.Enabled = query.Page > 0;

            RefreshRooms();
        }

        private void buttonPreviousPage_Click(object sender, EventArgs e)
        {
            query = new MatchmakingQuery(query, -1);

            labelQuery.Text = "Current query: " + query.ToString();
            buttonPreviousPage.Enabled = query.Page > 0;

            RefreshRooms();
        }

        private void buttonJoinRandom_Click(object sender, EventArgs e)
        {
            server.JoinRandomRoomAsync(query, (state, room) =>
            {
                BeginInvoke(new Action(() =>
                {
                    labelResults.Text = "Random join: " + state.ToString();

                    if (state <= PacketResponseStatus.Okey)
                    {
                        DialogResult = DialogResult.OK;
                    }
                }));
            });
        }

        private void buttonJoinRandomOrNew_Click(object sender, EventArgs e)
        {
            FormCreateRoom form = new FormCreateRoom();

            if (form.ShowDialog() == DialogResult.OK)
            {
                server.JoinRandomRoomAsync(query, (state, room) =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        labelResults.Text = "Random or new join: " + state.ToString();

                        if (state <= PacketResponseStatus.Okey)
                        {
                            DialogResult = DialogResult.OK;
                        }
                    }));
                }, form.RoomTemplate);
            }
        }

        private void buttonJoinCode_Click(object sender, EventArgs e)
        {
            InputDialog i = new InputDialog()
            {
                WindowTitle = "Join room with code",
                MainInstruction = "Please specify the room code:",
                MaxLength = 4,
                Input = "abcd"
            };

            if (string.IsNullOrWhiteSpace(i.Input))
                return;

            if (i.ShowDialog() == DialogResult.OK)
            {
                server.JoinRoomWithCodeAsync(i.Input, (state, room) =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        labelResults.Text = "Join with code: " + state.ToString();

                        if (state <= PacketResponseStatus.Okey)
                        {
                            DialogResult = DialogResult.OK;
                        }
                    }));
                });
            }
        }

        private void buttonJoinNewRoom_Click(object sender, EventArgs e)
        {
            FormCreateRoom form = new FormCreateRoom();

            if (form.ShowDialog() == DialogResult.OK)
            {
                server.JoinNewRoomAsync(form.RoomTemplate, (state, room) =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        labelResults.Text = "Join new room: " + state.ToString();

                        if (state <= PacketResponseStatus.Okey)
                        {
                            DialogResult = DialogResult.OK;
                        }
                    }));
                });
            }
        }
    }
}
