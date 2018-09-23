

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Scavenger.Server.Domain;
using Scavenger.Server.Forms.TestClient;

namespace Scavenger.Server.Forms
{
    public partial class Form1 : Form
    {
        private readonly Queue<Position> _points = new Queue<Position>();
        private readonly Queue<double> _angles = new Queue<double>();
        private readonly IGuideService _guideService;
        private readonly TestGuideClient _guideClient = new TestGuideClient();
        private readonly IScavengerService _scavengerService;
        private readonly TestScavengerClient _scavengerClient = new TestScavengerClient();

        private Guid? _scavengerId;
        private Guid? _guideId;

        public Form1()
        {
            var ipAddress = "13.67.234.244";
            //var ipAddress = "192.168.0.109";
            InitializeComponent();
            _guideService = new GuideService(ipAddress);
            _scavengerService = new ScavengerService(ipAddress);

            _guideClient.OnScavengerMoved += _guideClient_OnScavengerMoved;
            _guideClient.OnLobbyReady += _guideClient_OnLobbyReady;
            _guideClient.OnScavengerChangedDirection += _guideClient_OnScavengerChangedDirection;

            _scavengerClient.OnLobbyReady += _scavengerClient_OnLobbyReady;

            timer1.Start();

        }

        private void _scavengerClient_OnLobbyReady(Guid scavengerId)
        {
            _scavengerId = scavengerId;
        }

        private void _guideClient_OnLobbyReady(Guid guideId)
        {
            _guideId = guideId;
        }

        private void _guideClient_OnScavengerChangedDirection(double direction)
        {
            _angles.Enqueue(direction);
        }

        private void _guideClient_OnScavengerMoved(Position position)
        {
            _points.Enqueue(position);
        }

        private void LogMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(
                    new MethodInvoker(
                        delegate () { LogMessage(message); }));
            }
            else
            {
                txtNotes.Text += message + "\r\n";
            }
        }

        private Position _currentPosition = null;
        private double _currentAngle;
        DataPoint _dataPoint = null;
        private DateTime? _expireAngle;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_scavengerId.HasValue)
            {
                button2.Enabled = true;
            }

            var directionChanged = false;
            var positionChanged = false;

            while (_points.Any())
            {
                positionChanged = true;
                _currentPosition = _points.Dequeue();
                chart1.Series[0].Points.AddXY(_currentPosition.X, _currentPosition.Y);
            }

            while (_angles.Any())
            {
                directionChanged = true;
                _currentAngle = _angles.Dequeue();
            }

            if (_currentPosition == null) return;

            if (!positionChanged && !directionChanged) return;

            _dataPoint = new DataPoint(_currentPosition.X + .5 * Math.Cos(_currentAngle),
                                       _currentPosition.Y + .5 * Math.Sin(_currentAngle));

            chart1.Series[1].Points.Add(_dataPoint);

            if (_expireAngle.HasValue && DateTime.Now >= _expireAngle.Value && chart1.Series[1].Points.Any())
            {
                for (int i = 5; i < chart1.Series[1].Points.Count; i++)
                {
                    chart1.Series[1].Points.RemoveAt(i);
                }
            }
            else
            {
                _expireAngle = DateTime.Now.AddSeconds(5);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _guideService.Start(_guideClient);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var actionList = GetPositionData((x, y, t) => new ScavengerAction { Execute = () => _scavengerService.Move(_scavengerId.Value, new Position(x, y)), Ticks = t })
                             .Concat(GetDirectionData((ang, t) => new ScavengerAction { Execute = () => _scavengerService.ChangeDirection(_scavengerId.Value, ang), Ticks = t }))
                             .OrderBy(a => a.Ticks);

            RunActionList(actionList);
        }

        private async void RunActionList(IOrderedEnumerable<ScavengerAction> actionList)
        {
            await Task.Run(() =>
            {
                var ticks = actionList.First().Ticks;
                foreach (var action in actionList)
                {
                    var waitTime = action.Ticks - ticks;
                    Thread.Sleep(new TimeSpan(waitTime));
                    action.Execute();
                    ticks = action.Ticks;
                }
            });
        }

        private IEnumerable<T> GetPositionData<T>(Func<double, double, long, T> p)
        {
            if (!File.Exists("PositionTracker.csv"))
                yield break;

            var positionTracker = File.OpenText("PositionTracker.csv");
            while (!positionTracker.EndOfStream)
            {
                var readLine = positionTracker.ReadLine();
                if (readLine == null) continue;

                var line = readLine.Split(',');
                yield return p(double.Parse(line[0]), double.Parse(line[1]), long.Parse(line[2]));
            }
        }

        private IEnumerable<T> GetDirectionData<T>(Func<double, long, T> p)
        {
            if (!File.Exists("DirectionTracker.csv"))
                yield break;

            var directionTracker = File.OpenText("DirectionTracker.csv");
            while (!directionTracker.EndOfStream)
            {
                var readLine = directionTracker.ReadLine();
                if (readLine == null) continue;

                var line = readLine.Split(',');
                yield return p(double.Parse(line[0]), long.Parse(line[1]));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _scavengerService.Start(_scavengerClient);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _guideService.ScavengerFoundEgg(_guideId.Value);
        }
    }

    internal class ScavengerAction
    {
        public Action Execute { get; set; }
        public long Ticks { get; set; }
    }
}

//private async Task LogPosition(Position position)
//{
//    using (var positionTracker = File.AppendText("PositionTracker.csv"))
//    {
//        await positionTracker.WriteLineAsync(position.X + "," + position.Y + "," + DateTime.Now.Ticks);
//    }
//}

//public async Task ChangeDirection(Guid scavengerId, double direction)
//{
//    await LogDirection(direction);
//    var grain = GrainClient.GrainFactory.GetGrain<IScavengerGrain>(scavengerId);
//    await grain.ChangeDirection(direction);
//}

//private async Task LogDirection(double direction)
//{
//    using (var directionTracker = File.AppendText("DirectionTracker.csv"))
//    {
//        await directionTracker.WriteLineAsync(direction + "," + DateTime.Now.Ticks);
//    }
//}