using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace OpcUa.Client.Core
{
    public class ZoomChartViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _unityOfWork;
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public ObservableCollection<VariableEntity> Variables { get; set; }

        public List<VariableEntity> SelectedVariables
        {
            set => ShowChosenVariables(value);
        }

        public double XAxisMin { get; set; } = double.NaN;
        public double XAxisMax { get; set; } = double.NaN;

        public double YAxisMin { get; set; } = double.NaN;
        public double YAxisMax { get; set; } = double.NaN;

        public Func<double, string> XFormatter { get; set; }
        public ZoomingOptions ZoomingMode { get; set; } = ZoomingOptions.Xy;

        public ICommand ResetZoomCommand { get; set; }

        public ZoomChartViewModel(IUnitOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;

            XFormatter = value => new DateTime((long)value).ToString("dd MMM H:mm:ss");
            OnLoad();

            ResetZoomCommand = new RelayCommand(() => IoC.Messenger.Send(new SendResetAxises()) );
            MessengerInstance.Register<SendManageArchivedValue>(msg => ManageArchiveVariables(msg.Delete, msg.Variable));
        }

        private void ShowChosenVariables(List<VariableEntity> selectedVariables)
        {
            SeriesCollection.Clear();

            foreach (var variable in selectedVariables)
            {
                var values = new ChartValues<DateTimePoint>(variable.Records.Select(x => new DateTimePoint()
                {
                    Value = Convert.ToDouble(x.Value),
                    DateTime = x.ArchiveTime
                }));

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
    }
}
