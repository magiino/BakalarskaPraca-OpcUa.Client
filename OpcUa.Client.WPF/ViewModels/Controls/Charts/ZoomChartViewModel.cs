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
        public ObservableCollection<VariableModel> Variables { get; set; }
        public List<VariableModel> SelectedVariables
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
        public ZoomingOptions ZoomingMode { get; set; } = ZoomingOptions.Xy;
        public ICommand ToggleZoomOptionCommand { get; }
        public ICommand ResetZoomCommand { get; }
        #endregion

        #region Constructor
        public ZoomChartViewModel(IUnitOfWork unityOfWork, Messenger messenger)
        {
            _unityOfWork = unityOfWork;

            XFormatter = value => new DateTime((long)value).ToString("dd MMM H:mm:ss");
            OnLoad();

            ToggleZoomOptionCommand = new MixRelayCommand(ToggleZoomOption);
            ResetZoomCommand = new MixRelayCommand((obj) => IoC.Messenger.Send(new SendResetAxises()));
            messenger.Register<SendManageArchivedValue>(msg => ManageArchiveVariables(msg.Delete, msg.Variable));
        } 
        #endregion

        #region Private Methods
        private void ShowChosenVariables(IEnumerable<VariableModel> selectedVariables)
        {
            SeriesCollection.Clear();

            foreach (var variable in selectedVariables)
            {
                var values = new ChartValues<DateTimePoint>(_unityOfWork.Records.Find(x => x.VariableId == variable.Id).OrderBy(x => x.ArchiveTime).Select(x => new DateTimePoint()
                {
                    Value = Convert.ToDouble(x.Value),
                    DateTime = x.ArchiveTime
                }));

                SeriesCollection.Add(
                    new StepLineSeries()
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
        
        private void ManageArchiveVariables(bool delete, VariableModel variable)
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

        private void ToggleZoomOption(object parameter)
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
            var variables = Mapper.VariableEntitiesToVariableListModels( _unityOfWork.Variables.Find(x => x.ProjectId == IoC.AppManager.ProjectId) );
            if (variables == null) return;
            Variables = new ObservableCollection<VariableModel>(variables);
        } 
        #endregion
    }
}