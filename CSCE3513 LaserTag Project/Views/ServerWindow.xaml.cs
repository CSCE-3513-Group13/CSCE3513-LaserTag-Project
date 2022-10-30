using CSCE3513_LaserTag_Project.Configs;
using CSCE3513_LaserTag_Project.Messages;
using CSCE3513_LaserTag_Project.Networking;
using CSCE3513_LaserTag_Project.SQL;
using NLog.Targets.Wrappers;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CSCE3513_LaserTag_Project.Utils;
using NLog.Layouts;
using NLog.Targets;
using NLog.Fluent;
using System.IO;
using Path = System.IO.Path;
using System.Windows.Threading;
using System.Threading;

namespace CSCE3513_LaserTag_Project.Views
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        private SQLManager conn;
        private NetworkListener listener;
        private NetworkSender sender;

        private Random rand = new Random((int)DateTime.Now.Ticks);
        public static string ApplicationLocation { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        public static ServerConfigs Configs => _AppConfigs?.Data;
        public static SharedPersistent<ServerConfigs> _AppConfigs;
        public static Dispatcher serverDispatch;

        private static Logger log;

        //This is the main entry point for server code
        public ServerWindow()
        {
            ServerWindow._AppConfigs = LoadConfigs();
            Target.Register<FlowDocumentTarget>("FlowDocument");
            serverDispatch = this.Dispatcher;
            //Set the UI datacontext to this class
            this.DataContext = Configs;
            this.ContentRendered += ServerWindow_ContentRendered;

            InitializeComponent();
        }

        private void ServerWindow_ContentRendered(object sender, EventArgs e)
        {
            AttachConsole();

            conn = new SQLManager();
            listener = new NetworkListener(serverRecieved, 7500);
            sender = new NetworkSender();

            Configs.updateClock();

            log = LogManager.GetCurrentClassLogger();
            log.Warn("Welcome to the CSCE LaserTag Server!");

            testFlow();
        }

        private void testFlow()
        {
            SolidColorBrush BlueBrush = new SolidColorBrush(Colors.Blue);
            SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);

            Run run1 = new Run();
            Run run2 = new Run();
            Run run3 = new Run();

            run1.Text = "Paragraph 1 with a red brush";
            run1.Foreground = RedBrush;

            run2.Text = "Paragraph 2 with a blue brush";
            run2.Foreground = BlueBrush;

            run3.Text = "Paragraph 3 and back to a red brush";
            run3.Foreground = RedBrush;
            // Add paragraphs to the FlowDocument.
            RedFlow.Blocks.Add(new Paragraph(run1));
            RedFlow.Blocks.Add(new Paragraph(run2));
            RedFlow.Blocks.Add(new Paragraph(run3));
        }

        private SharedPersistent<ServerConfigs> LoadConfigs()
        {
            string NewConfigPath = Path.Combine(ApplicationLocation, "LaserTag.cfg");
            return SharedPersistent<ServerConfigs>.Load(NewConfigPath);
        }

        private void AttachConsole()
        {
            const string target = "wpf";
            var doc = LogManager.Configuration.FindTargetByName<FlowDocumentTarget>(target)?.Document;
            if (doc == null)
            {
                var wrapped = LogManager.Configuration.FindTargetByName<WrapperTargetBase>(target);
                doc = (wrapped?.WrappedTarget as FlowDocumentTarget)?.Document;
            }
            ConsoleText.FontSize = 12;
            ConsoleText.Document = doc ?? new FlowDocument(new Paragraph(new Run("No target!")));
            ConsoleText.TextChanged += ConsoleText_TextChanged;
        }
        private void ConsoleText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (RichTextBox)sender;
            ConsoleText.ScrollToEnd();
        }





        //This function is called when we recieve data on the server!
        private void serverRecieved(MessageManager data)
        {
            log.Info($"ServerRecieved: {data.type}");
            switch (data.type)
            {
                case MessageManager.messageType.LoginRequest:
                    loginRequest(data);
                    break;

                case MessageManager.messageType.GameState:

                    break;



                default:
                    log.Info("Unkown network type!");
                    break;

            }
        }

        public void autoJoinTeam(PlayerItem player)
        {
            //AutoBalances team. We will be able to switch teams on command

            if (Configs.redPlayers.Count == 0)
            {
                Configs.redPlayers.Add(player);
                return;
            }

            if(Configs.redPlayers.Count > Configs.bluePlayers.Count)
            {
                Configs.bluePlayers.Add(player);
            }
            else
            {
                Configs.redPlayers.Add(player);
            }
            
        }
        public async void loginRequest(MessageManager data)
        {
            LoginRequest r = Utils.Utilities.Deserialize<LoginRequest>(data.messageData);


            if (string.IsNullOrEmpty(r.playerID) && string.IsNullOrEmpty(r.username))
                return;

            log.Info($"Login Request: {r.playerID} - {r.loggingIn}");
            //might as well re-verify


            bool foundAccount = conn.doesPlayerExsist(r.playerID, out PlayerItem tableOut);

            if (r.loggingIn && !foundAccount)
            {
                r.response = $"Player of id:{r.playerID} does not exsist!";
                r.foundAccount = false;

            } else if (r.loggingIn && foundAccount)
            {

                r.response = $"Welcome {tableOut.codename}! F:{tableOut.first_name} L:{tableOut.last_name}";
                this.Dispatcher.Invoke(() => autoJoinTeam(tableOut));
            }
            else if(!r.loggingIn)
            {
                int randNum = rand.Next(100000, 999999);
                await conn.addPlayer(randNum.ToString(), r.username, r.firstname, r.lastname, 0, true);

                r.response = $"User created with ID {randNum}!";
            }

            MessageManager.sendMessage(r, MessageManager.messageType.LoginRequest, data.listenerPort);
        }
    }



    [Target("flowDocument")]
    public sealed class FlowDocumentTarget : TargetWithLayout
    {
        private FlowDocument _document = new FlowDocument { Background = new SolidColorBrush(Colors.Black) };
        private readonly Paragraph _paragraph = new Paragraph();
        private readonly int _maxLines = 500;

        public FlowDocument Document => _document;

        public FlowDocumentTarget()
        {
            _document.Blocks.Add(_paragraph);
        }

        /// <inheritdoc />
        protected override void Write(LogEventInfo logEvent)
        {
            _document.Dispatcher.Invoke(() => Update(logEvent));
        }

        private void Update(LogEventInfo logEvent)
        {


            var message = $"{Layout.Render(logEvent)}\n";
            _paragraph.Inlines.Add(new Run(message) { Foreground = LogLevelColors[logEvent.Level] });

            // A massive paragraph slows the UI down
            if (_paragraph.Inlines.Count > _maxLines)
                _paragraph.Inlines.Remove(_paragraph.Inlines.FirstInline);
        }

        private static readonly Dictionary<LogLevel, SolidColorBrush> LogLevelColors = new Dictionary<LogLevel, SolidColorBrush>
        {
            [LogLevel.Trace] = new SolidColorBrush(Colors.DimGray),
            [LogLevel.Debug] = new SolidColorBrush(Colors.DarkGray),
            [LogLevel.Info] = new SolidColorBrush(Colors.White),
            [LogLevel.Warn] = new SolidColorBrush(Colors.Magenta),
            [LogLevel.Error] = new SolidColorBrush(Colors.Yellow),
            [LogLevel.Fatal] = new SolidColorBrush(Colors.Red),
        };
    }
}
