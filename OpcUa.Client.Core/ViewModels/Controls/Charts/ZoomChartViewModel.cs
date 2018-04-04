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

        //public VariableEntity SelectedVariable
        //{
        //    set => ShowChosenVariable(value);
        //}

        public List<VariableEntity> SelectedVariables
        {
            set => ShowChosenVariables(value);
        }

        public double XAxisMin { get; set; } = double.NaN;
        public double XAxisMax { get; set; } = double.NaN;

        public double YAxisMin { get; set; } = double.NaN;
        public double YAxisMax { get; set; } = double.NaN;

        public Func<double, string> XFormatter { get; set; }
        public ZoomingOptions ZoomingMode { get; set; }

        public ICommand ResetZoomCommand { get; set; }
        public ICommand ToogleZoomModeCommand { get; set; }

        public ZoomChartViewModel(IUnitOfWork unityOfWork)
        {
            _unityOfWork = unityOfWork;

            XFormatter = value => new DateTime((long)value).ToString("dd MMM H:mm:ss");

            OnLoad();

            ResetZoomCommand = new RelayCommand(ResetZoom);
            ToogleZoomModeCommand = new RelayCommand(ToogleZoomMode);
        }

        //private void ShowChosenVariable(VariableEntity selectedVariable)
        //{
        //    SeriesCollection.Clear();

        //    var values = new ChartValues<DateTimePoint>(selectedVariable.Records.Select(x => new DateTimePoint()
        //    {
        //        Value = Convert.ToDouble(x.Value),
        //        DateTime = x.ArchiveTime
        //    }));

        //    SeriesCollection.Add(
        //        new LineSeries()
        //        {
        //            Title = selectedVariable.Name,
        //            Values = values,
        //            PointGeometrySize = 15,
        //            PointGeometry = DefaultGeometries.Cross,
        //            Fill = Brushes.Transparent
        //        }
        //    );
        //}

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

        private void ResetZoom()
        {
            XAxisMin = double.NaN;
            XAxisMax = double.NaN;
            YAxisMin = double.NaN;
            YAxisMax = double.NaN;
        }

        private void ToogleZoomMode()
        {
            switch (ZoomingMode)
            {
                case ZoomingOptions.None:
                    ZoomingMode = ZoomingOptions.X;
                    break;
                case ZoomingOptions.X:
                    ZoomingMode = ZoomingOptions.Y;
                    break;
                case ZoomingOptions.Y:
                    ZoomingMode = ZoomingOptions.Xy;
                    break;
                case ZoomingOptions.Xy:
                    ZoomingMode = ZoomingOptions.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnLoad()
        {
            var variables = _unityOfWork.Variables.Find(x => x.ProjectId == IoC.AppManager.ProjectId);
            Variables = new ObservableCollection<VariableEntity>(variables);
        }
    }
}
