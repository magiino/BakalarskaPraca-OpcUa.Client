using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class LiveChartViewModel : BaseViewModel
    {
        private readonly UaClientApi _uaClientApi;
        private readonly Messenger _messenger;

        private ReferenceDescription _selectedNode;
        private readonly Subscription _subscription;

        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public ObservableCollection<VariableLiveChartModel> Variables { get; set; } = new ObservableCollection<VariableLiveChartModel>();
        public VariableLiveChartModel SelectedVariable { get; set; }

        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public double AxisMax { get; set; }
        public double AxisMin { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public LiveChartViewModel(UaClientApi uaClientApi, Messenger messenger)
        {
            _uaClientApi = uaClientApi;
            _messenger = messenger;

            _subscription = _uaClientApi.Subscribe(2000, "LiveCharts");

            AddCommand = new RelayCommand(AddVariable);
            RemoveCommand = new RelayCommand(RemoveVariable);

            // Nastavenia grafu
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");
            AxisStep = TimeSpan.FromSeconds(10).Ticks;
            AxisUnit = TimeSpan.TicksPerSecond;
            SetAxisLimits(DateTime.Now);

            _messenger.Register<SendSelectedRefNode>(msg => _selectedNode = msg.ReferenceNode);
        }

        private void AddVariable(object parameter)
        {
            var variable = new VariableLiveChartModel()
            {
                Name = _selectedNode.DisplayName.ToString(),
                NodeId = _selectedNode.NodeId.ToString()
            };

            SeriesCollection.Add(
                new LineSeries()
                {
                    Title = variable.Name,
                    Values = new ChartValues<DateTimePoint>(),
                    PointGeometrySize = 15,
                    PointGeometry = DefaultGeometries.Cross,
                    Fill = Brushes.Transparent
                }
            );

            var item = _uaClientApi.CreateMonitoredItem(variable.Name, variable.NodeId, 500, null, 4);
            _uaClientApi.AddMonitoredItem(item, _subscription);
            item.Notification += Notification_MonitoredItem;

            Variables.Add(variable);
        }

        private void RemoveVariable(object parameter)
        {
            SeriesCollection.RemoveAt(Variables.IndexOf(SelectedVariable));
            Variables.Remove(SelectedVariable);
            SelectedVariable = null;
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(2).Ticks; // lets force the axis to be 5 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(60).Ticks; // and 20 seconds behind
        }

        #region Can use methods

        public bool AddNotificationCanUse()
        {
            if (_selectedNode == null)
                return false;
            else if (_selectedNode.NodeClass != NodeClass.Variable)
                return false;
            else return true;
        }

        public bool RemoveNotificationCanUse()
        {
            return SelectedVariable != null;
        }

        #endregion

        #region CallBack Methods

        /// <summary>
        /// Callback method for updating values of subscibed nodes
        /// </summary>
        /// <param name="monitoredItem"></param>
        /// <param name="e"></param>
        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (!(e.NotificationValue is MonitoredItemNotification notification))
                return;


            var value = notification.Value;

            var variable = Variables.SingleOrDefault(x => x.Name == monitoredItem.DisplayName);
           
            if (variable == null) return;

            DateTime time = value.SourceTimestamp;
            if (value.SourceTimestamp < DateTime.Now)
                time = DateTime.Now;

            // OSetrit ak niekto da stringovu premennu
            var index = Variables.IndexOf(variable);
            SeriesCollection[index].Values.Add(new DateTimePoint()
            {
                Value = Convert.ToDouble(value.Value),
                DateTime = time,
            });

            SetAxisLimits(DateTime.Now);

            if (SeriesCollection[index].Values.Count > 100)
                SeriesCollection[index].Values.RemoveAt(0);
        }

        #endregion
    }
}
