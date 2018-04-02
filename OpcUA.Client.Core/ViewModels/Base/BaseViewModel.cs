using System.ComponentModel;
using PropertyChanged;
using GalaSoft.MvvmLight.Messaging;

namespace OpcUa.Client.Core
{
    /// <summary>
    /// A base view model that fires Property Changed events as needed
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The event that is fired when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        // TODO Prejst na ineho messengera
        public Messenger MessengerInstance = IoC.Messenger;
    }
}
