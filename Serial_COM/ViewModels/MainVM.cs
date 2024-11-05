using Serial_COM.Common;
using Serial_COM.Models;
using System;
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

        private readonly EnvironmentSet _environmentSet;
        private ObservableCollection<string> _portNames;
        private ObservableCollection<int> _baudRates;

        private string _selectingPort;
        private int _selectedBaudRate;
        private bool _isEngineStarted;
        private bool _isEngineRestarted;
        private bool _isEngineKilled;
        private bool _isPowerSwitchOn;

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
            get => _selectingPort;
            set
            {
                if (_selectingPort != value)
                {
                    _selectingPort = value;
                    OnPropertyChanged();
                }

            }

        }

        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set
            {
                if (_selectedBaudRate != value)
                {
                    _selectedBaudRate = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsPowerSwitchOn]
        /// </summary>
        public bool IsPowerSwitchOn
        {
            get => _isPowerSwitchOn;
            set
            {
                if (_isPowerSwitchOn != value)
                {
                    _isPowerSwitchOn = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsEngineStarted]
        /// </summary>
        public bool IsEngineStarted
        {
            get => _isEngineStarted;
            set
            {
                if (_isEngineStarted != value)
                {
                    _isEngineStarted = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsEngineRestarted]
        /// </summary>
        public bool IsEngineRestarted
        {
            get => _isEngineRestarted;
            set
            {
                if (_isEngineRestarted != value)
                {
                    _isEngineRestarted = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsEngineKilled]
        /// </summary>
        public bool IsEngineKilled
        {
            get => _isEngineKilled;
            set
            {
                if (_isEngineKilled != value)
                {
                    _isEngineKilled = value;
                    OnPropertyChanged();
                }

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
                parser.RunExample();
                CCUtoCPCField parserData = parser.Parse(messageListen);
                if (parserData != null)
                {
                    IsPowerSwitchOn = parserData.PowerSwitch == 1;

                    IsEngineStarted = parserData.EngineStart == 1;
                    IsEngineRestarted = parserData.EngineRestart == 1;
                    IsEngineKilled = parserData.EngineKill == 1;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

    }

}
