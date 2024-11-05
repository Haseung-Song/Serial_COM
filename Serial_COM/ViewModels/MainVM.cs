using Serial_COM.Common;
using Serial_COM.Models;
using System;
using System.Collections.Generic;
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
        private bool _isPortConnected;
        private List<string> _portNames;
        private List<int> _baudRates;
        private string _selectingPort;
        private int _selectedBaudRate;
        private bool _isEngineStarted;
        private bool _isEngineRestarted;
        private bool _isEngineKilled;
        private bool _isPowerSwitchOn;
        private bool _isTakeOffOrNot;
        private bool _isReturnToBase;

        #endregion

        #region [OnPropertyChanged]

        public event PropertyChangedEventHandler PropertyChanged;

        public EnvironmentSet EnvironmentSet
        {
            get => _environmentSet;
            private set
            {
                if (_environmentSet != value)
                {
                    _environmentSet = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [Connection_BtnText]
        /// </summary>
        public string Connection_BtnText => IsPortConnected ? "Disconnect" : "Connect";

        /// <summary>
        /// [IsPortConnected]
        /// </summary>
        public bool IsPortConnected
        {
            get => _isPortConnected;
            set
            {
                if (_isPortConnected != value)
                {
                    _isPortConnected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Connection_BtnText));
                }

            }

        }

        /// <summary>
        /// [LstPortNames]
        /// </summary>
        public List<string> LstPortNames
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
        public List<int> LstBaudRates
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

        /// <summary>
        /// [IsTakeOffOrNot]
        /// </summary>
        public bool IsTakeOffOrNot
        {
            get => _isTakeOffOrNot;
            set
            {
                if (_isTakeOffOrNot != value)
                {
                    _isTakeOffOrNot = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsReturnToBase]
        /// </summary>
        public bool IsReturnToBase
        {
            get => _isReturnToBase;
            set
            {
                if (_isReturnToBase != value)
                {
                    _isReturnToBase = value;
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
            EnvironmentSet = new EnvironmentSet();
            IsPortConnected = false;
            LstPortNames = EnvironmentSet.GetPortNames();
            LstBaudRates = EnvironmentSet.GetBaudRates();
        }

        private void StartSerial()
        {
            if (EnvironmentSet != null)
            {
                bool IsPortOpen = EnvironmentSet.OpenToClose(SelectedPort, SelectedBaudRate);
                if (IsPortOpen)
                {
                    IsPortConnected = true;
                    EnvironmentSet.MessageReceived += OnMessageReceived;
                }
                else
                {
                    IsPortConnected = false;
                }

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
                    IsTakeOffOrNot = parserData.TakeOff == 1;
                    IsReturnToBase = parserData.ReturnToBase == 1;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

    }

}
