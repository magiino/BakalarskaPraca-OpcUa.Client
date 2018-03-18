using PropertyChanged;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace OpcUA.Client.Core
{
    /// <summary>
    /// A base view model that fires Property Changed events as needed
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class BaseViewModel : ViewModelBase
    {
    //    /// <summary>
    //    /// The event that is fired when any child property changes its value
    //    /// </summary>
    //    public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
