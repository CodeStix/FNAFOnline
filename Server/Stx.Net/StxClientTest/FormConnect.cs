using Stx.Net;
using Stx.Utilities;
using System;
using System.Windows.Forms;

namespace StxClientTest
{
    public partial class FormConnect : Form
    {
        private JsonConfig<ConnectInfo> config = new JsonConfig<ConnectInfo>("connectInfo.json");

        public ConnectInfo ConnectInfo { get; set; }

        private readonly string[] RandomNames = 
        {
            "Donny",            "Estefana",            "Megan",            "Erline",            "Dorinda",            "Trenton",            "Soowwww",            "Latanya",            "Gavin",            "Larissa",            "Irana",            "Weldon",            "Nana",            "Shonta",            "Julian",            "Josephine",            "Elke",            "Terrilyn",            "Candy",            "Eloise",            "Manuel",            "Jonie",            "Nida",            "Calista",            "Letisha",            "Sharyn",            "Kimberely",            "Sterling",            "Penelope",            "Rananana",            "Sjonnie",
            "Pannekoek",
            "Chocopot",
            "DylanIsGayMaal",
            "IkWilFietsen",
            "Kutkoffie",
            "RobbeWasHere",
            "FakeCodeStix",
            "Minion",
            "RainForest",
            "Paasei2",
            "Komkommer",
            "Potato",
            "AldiLover",
            "DubbelePizza"
        };

        private Random random = new Random();

        public FormConnect()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            ConnectInfo.host = textBoxHost.Text;
            ConnectInfo.port = ushort.Parse(textBoxPort.Text);
            ConnectInfo.clientID = comboBoxClientID.Text;
            ConnectInfo.authToken = comboBoxAuthToken.Text;
            ConnectInfo.appKey = textBoxAppKey.Text;
            ConnectInfo.appName = textBoxAppName.Text;
            ConnectInfo.appVersion = textBoxAppVersion.Text;
            ConnectInfo.useName = checkBoxUseName.Checked;
            ConnectInfo.name = textBoxName.Text;

            config.Settings = ConnectInfo;
            config.Save();

            DialogResult = DialogResult.OK;
        }

        private void FormConnect_Load(object sender, EventArgs e)
        {
            ConnectInfo = config.Settings;

            textBoxHost.Text = ConnectInfo.host;
            textBoxPort.Text = ConnectInfo.port.ToString();
            comboBoxClientID.Text = ConnectInfo.clientID;
            comboBoxAuthToken.Text = ConnectInfo.authToken;
            textBoxAppKey.Text = ConnectInfo.appKey;
            textBoxAppName.Text = ConnectInfo.appName;
            textBoxAppVersion.Text = ConnectInfo.appVersion;
            checkBoxUseName.Checked = ConnectInfo.useName;
            textBoxName.Text = ConnectInfo.name;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonRandomClient_Click(object sender, EventArgs e)
        {
            comboBoxClientID.Text = Guid.NewGuid().ToString();
            comboBoxAuthToken.Text = Guid.NewGuid().ToString();

            textBoxName.Text = RandomNames[random.Next(RandomNames.Length)] + random.Next(100, 1000).ToString();
            checkBoxUseName.Checked = true;
        }
    }

    public class ConnectInfo
    {
        public string host = "localhost";
        public ushort port = 1987;
        public string clientID = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        public string authToken = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        public string appKey = "ffffffff-ffff-ffff-ffff-ffffffffffff";
        public string appName = StxNet.DefaultApplicationName;
        public string appVersion = StxNet.DefaultApplicationVersion;
        public bool useName = false;
        public string name = "ThisIsMyName";

        public ConnectionStartInfo GetStartInfo()
        {
            ConnectionStartInfo s = new ConnectionStartInfo(host, port, appKey, false);
            s.ClientID = clientID;
            s.AuthorizationToken = authToken;
            s.ApplicationName = appName;
            s.ApplicationVersion = appVersion;
            if (useName)
                s.ClientName = name;

            return s;
        }
    }
}
