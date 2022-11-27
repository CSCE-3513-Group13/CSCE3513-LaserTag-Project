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
using System.Timers;
using Timer = System.Timers.Timer;
using static CSCE3513_LaserTag_Project.Messages.MessageManager;
using System.Windows.Markup;
using System.Numerics;

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
        public static Timer gameTimer = new Timer(1000);
        public static DateTime gameStop;

        public List<int> allClientPorts;

        //We use this for simple countdown variable
        private double remainderSeconds = 0;

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
            allClientPorts = new List<int>();

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

            //testFlow();
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

            Dispatcher.Invoke(() =>
            {
                switch (data.type)
                {
                    case MessageManager.messageType.LoginRequest:
                        loginRequest(data);
                        break;

                    case MessageManager.messageType.GameState:

                        break;

                    //new player joined or switch team request
                    case MessageManager.messageType.newPlayerActivated:
                        switchTeam(data);
                        break;



                    default:
                        log.Info("Unkown network type!");
                        break;

                }
            });
        }

        private void signalGameState(bool state, bool reset = false)
        {
            GameState gstate = new GameState();
            gstate.Start = DateTime.Now;
            gstate.End = gameStop;
            gstate.State = state;
            gstate.Reset = reset;

            log.Warn($"Signaling game state to {state}!");
            foreach (var client in allClientPorts)
            {
                MessageManager.sendMessage(gstate, MessageManager.messageType.GameState, client);
            }
        }


        private void switchTeam(MessageManager data)
        {
            newPlayerActivated r = Utils.Utilities.Deserialize<newPlayerActivated>(data.messageData);

            Configs.redPlayers.Remove(r.players[0].player);
            Configs.bluePlayers.Remove(r.players[0].player);

            if (r.players[0].team == MessageManager.Team.Red)
            {
                Configs.redPlayers.Add(r.players[0].player);
            }
            else
            {
                Configs.bluePlayers.Add(r.players[0].player);
            }

            log.Info($"{r.players[0].player.codename} switched to {r.players[0].team} team!");

            updateAllTeams();
        }





        public void autoJoinTeam(PlayerItem player)
        {
            //AutoBalances team. We will be able to switch teams on command

            if (Configs.redPlayers.Count == 0)
            {
                Configs.redPlayers.Add(player);
                updateAllTeams();
                return;
            }


            if (Configs.redPlayers.Count > Configs.bluePlayers.Count)
            {
                Configs.bluePlayers.Add(player);
            }
            else
            {
                Configs.redPlayers.Add(player);
            }


            updateAllTeams();
        }

        public void updateAllTeams()
        {
            newPlayerActivated act = new newPlayerActivated();
        

            foreach(var player in Configs.redPlayers)
            {
                playerTeam t = new playerTeam();
                t.player = player;
                t.team = Team.Red;

                act.players.Add(t);
            }

            foreach(var player in Configs.bluePlayers)
            {
                playerTeam t = new playerTeam();
                t.player = player;
                t.team = Team.Blue;

                act.players.Add(t);
            }


            log.Info($"Sending player joined to all {allClientPorts.Count} clients!");
            foreach (var client in allClientPorts)
            {
                MessageManager.sendMessage(act, MessageManager.messageType.newPlayerActivated, client);
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

            }
            else if (r.loggingIn && foundAccount)
            {
                //Add this client into all clients. (They successfully logged in)
                log.Warn($"Added {data.listenerPort} to all client ports!");

                //If this client hasnt joined, add them to all of our client lists
                if(!allClientPorts.Contains(data.listenerPort))
                    allClientPorts.Add(data.listenerPort);

                r.response = $"Welcome {tableOut.codename}! F:{tableOut.first_name} L:{tableOut.last_name}";
                r.foundAccount = true;
                r.firstname = tableOut.first_name;
                r.lastname = tableOut.last_name;
                r.username = tableOut.codename;
                r.score = tableOut.score.ToString();

                autoJoinTeam(tableOut);

            }
            else if (!r.loggingIn)
            {
                int randNum = rand.Next(100000, 999999);
                await conn.addPlayer(randNum.ToString(), r.username, r.firstname, r.lastname, 0, true);

                r.playerID = randNum.ToString();
                r.score = "0";
                r.response = $"User created with ID {randNum}!";

                allClientPorts.Add(data.listenerPort);
                autoJoinTeam(tableOut);
            }

            MessageManager.sendMessage(r, MessageManager.messageType.LoginRequest, data.listenerPort);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            log.Warn($"User pressed {e.Key}");

            if (e.Key == Key.F3)
                startGame();
            else if (e.Key == Key.F4)
                stopGame();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            startGame();
        }

        private void StopGameButton_Click(object sender, RoutedEventArgs e)
        {
            stopGame(true);


            //Resets players
            Configs.redPlayers.Clear();
            Configs.bluePlayers.Clear();
        }

        private void stopGame(bool reset = false)
        {
            try
            {
                gameTimer.Elapsed -= GameTimer_Elapsed;
                gameTimer.Stop();
            }
            catch
            {

            }
            finally
            {
                Configs.updateClock();
                signalGameState(false, reset);
            }
        }

        private void startGame()
        {
            gameTimer.Elapsed += GameTimer_Elapsed;

            remainderSeconds = Configs.timeLimit * 60 - 1;

            //use UTC if we are communicating across timezones (personal experience)
            gameStop = DateTime.Now.AddMinutes(Configs.timeLimit);
            gameTimer.Start();

            signalGameState(true);

        }
        private void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            

            if (gameStop < DateTime.Now)
            {
                gameTimer.Stop();
                gameTimer.Elapsed -= GameTimer_Elapsed;

                signalGameState(false);
                return;
            }

            //possibly try catch
            try
            {
                Dispatcher.Invoke(() => { networkGenerator(); });
            }catch(Exception ex)
            {
                log.Error(ex);
            }


            TimeSpan remainderTime = TimeSpan.FromSeconds(remainderSeconds);
            Configs.updateClock(remainderTime);

            remainderSeconds--;
        }



        SolidColorBrush BlueBrush = new SolidColorBrush(Colors.Blue);
        SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
        private void networkGenerator()
        {
            double rTeam = rand.NextDouble();


            PlayerItem red = null; //who fired
            PlayerItem blue = null; //who got hit


            int randomRedIndex = rand.Next(0, Configs.redPlayers.Count);
            red = Configs.redPlayers[randomRedIndex];

            int randomBlueIndex = rand.Next(0, Configs.bluePlayers.Count);
            blue = Configs.bluePlayers[randomBlueIndex];


            int damageDelt = rand.Next(1, Configs.playerDamage);

            

            Run run = new Run();
           

            GameAction a = null;
            if (rTeam > .5)
            {
                //Red team hit blue team
                string response = $"{red.codename} hit {blue.codename} for {damageDelt}";
                run.Text = response;
                run.Foreground = RedBrush;
                Configs.RedScore += damageDelt;
                a = new GameAction(response, GameAction.colorType.blue, Configs.RedScore, Configs.BlueScore);
            }
            else
            {
                //Blue team hit red team
                string response = $"{blue.codename} hit {red.codename} for {damageDelt}";
                run.Text = response;
                run.Foreground = BlueBrush;
                Configs.BlueScore += damageDelt;
                a = new GameAction(response, GameAction.colorType.red, Configs.RedScore, Configs.BlueScore);
            }

            RedFlow.Blocks.Add(new Paragraph(run));
            FeedBox.ScrollToEnd();

            foreach (var client in allClientPorts)
            {
                //log.Info($"Sending to {client}");
                MessageManager.sendMessage(a, MessageManager.messageType.GameAction, client);
            }
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
