using Serial_COM.Common;
using Serial_COM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static Serial_COM.Models.Parser;

namespace Serial_COM.ViewModels
{
    public class MainVM : INotifyPropertyChanged
    {
        #region [프로퍼티]

        private EnvironmentSet _environmentSet;
        private ObservableCollection<string> _portNames;
        private ObservableCollection<int> _baudRates;

        private string _selectedPort;
        private int _selectedBaudRate;

        #endregion

        #region [OnPropertyChanged]

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// [LstPortNames]
        /// </summary>
        public ObservableCollection<string> LstPortNames
        {
            get => _portNames;
            set
            {
                if (_portNames != value)
                {
                    _portNames = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [LstBaudRates]
        /// </summary>
        public ObservableCollection<int> LstBaudRates
        {
            get => _baudRates;
            set
            {
                if (_baudRates != value)
                {
                    _baudRates = value;
                    OnPropertyChanged();
                }

            }

        }

        public string SelectedPort
        {
            get => _selectedPort;
            set
            {
                _selectedPort = value;
                OnPropertyChanged();
            }
        }

        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set
            {
                _selectedBaudRate = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region [ICommand]

        public ICommand StartSerialCommand { get; set; }

        #endregion

        #region 생성자 (Initialize)

        public MainVM()
        {
            StartSerialCommand = new RelayCommand(StartSerial);
            _environmentSet = new EnvironmentSet();
            LstPortNames = _environmentSet.GetPortNames();
            LstBaudRates = _environmentSet.GetBaudRates();
        }

        private void StartSerial()
        {
            if (_environmentSet != null)
            {
                _environmentSet.MessageReceived += OnMessageReceived;
                _environmentSet.Open(SelectedPort, SelectedBaudRate);
            }

        }

        #endregion


        private void OnMessageReceived(byte[] messageListen, DateTime currentTime)
        {
            try
            {
                Parser parser = new Parser();
                CCUtoCPCField parserData = parser.Parse(messageListen);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

    }

}
