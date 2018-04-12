using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class ZoomChartViewModel : BaseViewModel
    {

        #region Private Fields
        private readonly IUnitOfWork _unityOfWork;
        #endregion

        #region Public Properties
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public ObservableCollection<VariableEntity> Variables { get; set; }
        public List<VariableEntity> SelectedVariables
        {
            set => ShowChosenVariables(value);
        }

        public Func<double, string> XFormatter { get; set; }

        public double XAxisMin { get; set; } = double.NaN;
        public double XAxisMax { get; set; } = double.NaN;

        public double YAxisMin { get; set; } = double.NaN;
        public double YAxisMax { get; set; } = double.NaN;
        #endregion

        #region Commands
        public ZoomingOptions ZoomingMode { get; } = ZoomingOptions.Xy;
        public ICommand ResetZoomCommand { get; }
        #endregion

        #region Constructor
        public ZoomChartViewModel(IUnitOfWork unityOfWork, Messenger messenger)
        {
            _unityOfWork = unityOfWork;

            XFormatter = value => new DateTime((long)value).ToString("dd MMM H:mm:ss");
            OnLoad();

            ResetZoomCommand = new MixRelayCommand((obj) => IoC.Messenger.Send(new SendResetAxises()));
            messenger.Register<SendManageArchivedValue>(msg => ManageArchiveVariables(msg.Delete, msg.Variable));
        } 
        #endregion

        #region Private Methods
        private void ShowChosenVariables(IEnumerable<VariableEntity> selectedVariables)
        {
            SeriesCollection.Clear();

            foreach (var variable in selectedVariables)
            {
                var values = new ChartValues<DateTimePoint>(variable.Records.Select(x => new DateTimePoint()
                {
                    Value = Convert.ToDouble(x.Value),
                    DateTime = x.ArchiveTime
                }));

                // Get unsaved variables
                var localValues = new ChartValues<DateTimePoint>(_unityOfWork.Records.Find(x => x.VariableId == variable.Id).Select(x => new DateTimePoint()
                {
                    Value = Convert.ToDouble(x.Value),
                    DateTime = x.ArchiveTime
                }));

                values.AddRange(localValues);

                SeriesCollection.Add(
                    new LineSeries()
                    {
                        Title = variable.Name,
                        Values = values,
                        PointGeometrySize = 15,
                        PointGeometry = DefaultGeometries.Cross,
                        Fill = Brushes.Transparent
                    }
                );
            }
        }
        
        private void ManageArchiveVariables(bool delete, VariableEntity variable)
        {
            if (delete)
            {
                Variables.Remove(variable);
                var seriesToDelete = SeriesCollection.SingleOrDefault(x => x.Title == variable.Name);
                SeriesCollection.Remove(seriesToDelete);
            }
            else
            {
                Variables.Add(variable);
            }
        }
        private void OnLoad()
        {
            var variables = _unityOfWork.Variables.Find(x => x.ProjectId == IoC.AppManager.ProjectId);
            if (variables == null) return;
            Variables = new ObservableCollection<VariableEntity>(variables);
        } 
        #endregion
    }
}