﻿using Serial_COM.Common;
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
        private ObservableCollection<string> _lstBoxItem;
        private bool _isPortConnected;
        private List<string> _portNames;
        private List<int> _baudRates;
        private string _selectingPort;
        private int _selectedBaudRate;

        private bool _isPowerSwitch;

        private bool _isEngineStart;
        private bool _isEngineRestart;
        private bool _isEngineKill;
        private bool _isTakeOff;
        private bool _isReturnToBase;
        private bool _isAltitudeKnob;
        private bool _isHeadingKnob;
        private bool _isSpeedKnob;
        private bool _isDrop;
        private bool _isOption1;
        private bool _isCapture;
        private bool _isEOandIR;
        private bool _isOption2;
        private bool _isGimbalStick;
        private bool _isZoomKnob;
        private bool _isFocusKnob;

        private int _altitudeKnobChange;
        private double _altitudeKnobSum;
        private double _totalAltitudeChange;
        private bool _isAltitudeOn;
        private int _headingKnobChange;
        private double _headingKnobSum;
        private double _totalHeadingChange;
        private bool _isHeadingOn;
        private int _speedKnobChange;
        private double _speedKnobSum;
        private double _totalSpeedChange;
        private bool _isSpeedOn;
        private int _yawChange;
        private double _elipseYawX;
        private uint _throttleChange;
        private double _elipseThrottleY;
        private int _rollChange;
        private double _elipseRollX;
        private int _pitchChange;
        private double _elipsePitchY;
        private int _zoomChange;
        private int _focusChange;
        private int _joyStickXChange;
        private double _elipseJoyStickX;
        private int _joyStickYChange;
        private double _elipseJoyStickY;

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
        /// [LstBoxItem]
        /// </summary>
        public ObservableCollection<string> LstBoxItem
        {
            get => _lstBoxItem;
            set
            {
                if (_lstBoxItem != value)
                {
                    _lstBoxItem = value;
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

        /// <summary>
        /// [SelectedPort]
        /// </summary>
        public string SelectedPort
        {
            get => _selectingPort = "COM11";
            set
            {
                if (_selectingPort != value)
                {
                    _selectingPort = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [SelectedBaudRate]
        /// </summary>
        public int SelectedBaudRate
        {
            get => _selectedBaudRate = 115200;
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
        /// [IsPowerSwitch]
        /// [Byte #0.] 7번째 비트
        /// </summary>
        public bool IsPowerSwitch
        {
            get => _isPowerSwitch;
            set
            {
                if (_isPowerSwitch != value)
                {
                    _isPowerSwitch = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsEngineStart]
        /// [Byte #1.] 7번째 비트
        /// </summary>
        public bool IsEngineStart
        {
            get => _isEngineStart;
            set
            {
                if (_isEngineStart != value)
                {
                    _isEngineStart = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsEngineRestart]
        /// [Byte #1.] 6번째 비트
        /// </summary>
        public bool IsEngineRestart
        {
            get => _isEngineRestart;
            set
            {
                if (_isEngineRestart != value)
                {
                    _isEngineRestart = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsEngineKill]
        /// [Byte #1.] 5번째 비트
        /// </summary>
        public bool IsEngineKill
        {
            get => _isEngineKill;
            set
            {
                if (_isEngineKill != value)
                {
                    _isEngineKill = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsTakeOffOrNot]
        /// [Byte #1.] 4번째 비트
        /// </summary>
        public bool IsTakeOff
        {
            get => _isTakeOff;
            set
            {
                if (_isTakeOff != value)
                {
                    _isTakeOff = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsReturnToBase]
        /// [Byte #1.] 3번째 비트
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

        /// <summary>
        /// [IsAltitudeKnob]
        /// [Byte #1.] 2번째 비트
        /// </summary>
        public bool IsAltitudeKnob
        {
            get => _isAltitudeKnob;
            set
            {
                if (_isAltitudeKnob != value)
                {
                    _isAltitudeKnob = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsHeadingKnob]
        /// [Byte #1.] 1번째 비트
        /// </summary>
        public bool IsHeadingKnob
        {
            get => _isHeadingKnob;
            set
            {
                if (_isHeadingKnob != value)
                {
                    _isHeadingKnob = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsSpeedKnob]
        /// [Byte #1.] 0번째 비트
        /// </summary>
        public bool IsSpeedKnob
        {
            get => _isSpeedKnob;
            set
            {
                if (_isSpeedKnob != value)
                {
                    _isSpeedKnob = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [AltitudeKnobChange]
        /// [Byte #2.]
        /// </summary>
        public int AltitudeKnobChange
        {
            get => _altitudeKnobChange;
            set
            {
                if (_altitudeKnobChange != value)
                {
                    _altitudeKnobChange = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [AltitudeKnobSum]
        /// </summary>
        public double AltitudeKnobSum
        {
            get => _altitudeKnobSum;
            set
            {
                if (_altitudeKnobSum != value)
                {
                    //if (value < 0)
                    //    _altitudeKnobSum = 0;
                    //else
                    //    _altitudeKnobSum = value;
                    _altitudeKnobSum = value < 0 ? 0 : value;
                    OnPropertyChanged();
                }

            }

        }

        private const int MaxAltitudeChange = 60000;

        /// <summary>
        /// [TotalAltitudeChange]
        /// </summary>
        public double TotalAltitudeChange
        {
            get => _totalAltitudeChange;
            set
            {
                if (_totalAltitudeChange != value)
                {
                    //if (value > MaxAltitudeChange)
                    //    _totalAltitudeChange = MaxAltitudeChange;
                    //else
                    //    _totalAltitudeChange = value;
                    _totalAltitudeChange = value > MaxAltitudeChange ? MaxAltitudeChange : value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsAltitudeOn]
        /// </summary>
        public bool IsAltitudeOn
        {
            get => _isAltitudeOn;
            set
            {
                if (_isAltitudeOn != value)
                {
                    _isAltitudeOn = value;
                    OnPropertyChanged();
                    TransmitMessage();
                }

            }

        }

        /// <summary>
        /// [HeadingKnobChange]
        /// [Byte #3.]
        /// </summary>
        public int HeadingKnobChange
        {
            get => _headingKnobChange;
            set
            {
                if (_headingKnobChange != value)
                {
                    _headingKnobChange = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [HeadingKnobSum]
        /// </summary>
        public double HeadingKnobSum
        {
            get => _headingKnobSum;
            set
            {
                if (_headingKnobSum != value)
                {
                    //if (value < 0)
                    //    _headingKnobSum = 0;
                    //else
                    //    _headingKnobSum = value;
                    _headingKnobSum = value < 0 ? 0 : value;
                    OnPropertyChanged();
                }

            }

        }

        private const double MaxHeadingChange = 3599.0;

        /// <summary>
        /// [TotalHeadingChange]
        /// </summary>
        public double TotalHeadingChange
        {
            get => _totalHeadingChange;
            set
            {
                if (_totalHeadingChange != value)
                {
                    //if (value > MaxHeadingChange)
                    //    _totalHeadingChange = 359.9;
                    //else
                    //    _totalHeadingChange = value / 10;
                    _totalHeadingChange = value > MaxHeadingChange ? MaxHeadingChange / 10 : value / 10;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsHeadingOn]
        /// </summary>
        public bool IsHeadingOn
        {
            get => _isHeadingOn;
            set
            {
                if (_isHeadingOn != value)
                {
                    _isHeadingOn = value;
                    OnPropertyChanged();
                    TransmitMessage();
                }

            }

        }

        /// <summary>
        /// [SpeedKnobChange]
        /// [Byte #4.]
        /// </summary>
        public int SpeedKnobChange
        {
            get => _speedKnobChange;
            set
            {
                if (_speedKnobChange != value)
                {
                    _speedKnobChange = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [SpeedKnobSum]
        /// </summary>
        public double SpeedKnobSum
        {
            get => _speedKnobSum;
            set
            {
                if (_speedKnobSum != value)
                {
                    //if (value < 0)
                    //    _speedKnobSum = 0;
                    //else
                    //    _speedKnobSum = value;
                    _speedKnobSum = value < 0 ? 0 : value;
                    OnPropertyChanged();
                }

            }

        }

        private const double MaxSpeedChange = 1100.0;

        /// <summary>
        /// [TotalSpeedChange]
        /// </summary>
        public double TotalSpeedChange
        {
            get => _totalSpeedChange;
            set
            {
                if (_totalSpeedChange != value)
                {
                    //if (value > MaxHeadingChange)
                    //    _totalSpeedChange = 110.0;
                    //else
                    //    _totalSpeedChange = value / 10;
                    _totalSpeedChange = value > MaxSpeedChange ? MaxSpeedChange / 10 : value / 10;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsSpeedOn]
        /// </summary>
        public bool IsSpeedOn
        {
            get => _isSpeedOn;
            set
            {
                if (_isSpeedOn != value)
                {
                    _isSpeedOn = value;
                    OnPropertyChanged();
                    TransmitMessage();
                }

            }

        }

        /// <summary>
        /// [YawChange]
        /// [Byte #5.]
        /// </summary>
        public int YawChange
        {
            get => _yawChange;
            set
            {
                if (_yawChange != value)
                {
                    _yawChange = value;
                    int elipseYawX = value > 80 ? 80 : value;
                    ElipseYawX = 60 + (elipseYawX * 0.6);
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ElipseYawX]
        /// </summary>
        public double ElipseYawX
        {
            get => _elipseYawX;
            set
            {
                if (_elipseYawX != value)
                {
                    _elipseYawX = value >= 5 && value < 80 ? value - 5 : value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ThrottleChange]
        /// [Byte #6.]
        /// </summary>
        public uint ThrottleChange
        {
            get => _throttleChange;
            set
            {
                if (_throttleChange != value)
                {
                    _throttleChange = value;
                    uint elipseThrottleY = value > 105 ? 105 : value;
                    ElipseThrottleY = (107 - elipseThrottleY) * 1.07;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ElipseThrottleY]
        /// </summary>
        public double ElipseThrottleY
        {
            get => _elipseThrottleY;
            set
            {
                if (_elipseThrottleY != value)
                {
                    _elipseThrottleY = value >= 5 ? value - 5 : value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [RollChange]
        /// [Byte #7.]
        /// </summary>
        public int RollChange
        {
            get => _rollChange;
            set
            {
                if (_rollChange != value)
                {
                    _rollChange = value;
                    int elipseRollX = value > 80 ? 80 : value;
                    ElipseRollX = 60 + (elipseRollX * 0.6);
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ElipseRollX]
        /// </summary>
        public double ElipseRollX
        {
            get => _elipseRollX;
            set
            {
                if (_elipseRollX != value)
                {
                    _elipseRollX = value >= 5 && value < 80 ? value - 5 : value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [PitchChange]
        /// [Byte #8.]
        /// </summary>
        public int PitchChange
        {
            get => _pitchChange;
            set
            {
                if (_pitchChange != value)
                {
                    _pitchChange = value;
                    int elipsePitchY = value > 80 ? 80 : value;
                    ElipsePitchY = 60 + (elipsePitchY * 0.6);
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ElipsePitchY]
        /// </summary>
        public double ElipsePitchY
        {
            get => _elipsePitchY;
            set
            {
                if (_elipsePitchY != value)
                {
                    _elipsePitchY = value >= 5 && value < 80 ? value - 5 : value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsDrop]
        /// [Byte #9.] 7번째(MSB) 비트
        /// </summary>
        public bool IsDrop
        {
            get => _isDrop;
            set
            {
                if (_isDrop != value)
                {
                    _isDrop = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsOption1]
        /// [Byte #9.] 6번째 비트
        /// </summary>
        public bool IsOption1
        {
            get => _isOption1;
            set
            {
                if (_isOption1 != value)
                {
                    _isOption1 = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsCapture]
        /// [Byte #9.] 5번째 비트
        /// </summary>
        public bool IsCapture
        {
            get => _isCapture;
            set
            {
                if (_isCapture != value)
                {
                    _isCapture = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsEOandIR]
        /// [Byte #9.] 4번째 비트
        /// </summary>
        public bool IsEOandIR
        {
            get => _isEOandIR;
            set
            {
                if (_isEOandIR != value)
                {
                    _isEOandIR = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsOption2]
        /// [Byte #9.] 3번째 비트
        /// </summary>
        public bool IsOption2
        {
            get => _isOption2;
            set
            {
                if (_isOption2 != value)
                {
                    _isOption2 = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsGimbalStick]
        /// [Byte #9.] 2번째 비트
        /// </summary>
        public bool IsGimbalStick
        {
            get => _isGimbalStick;
            set
            {
                if (_isGimbalStick != value)
                {
                    _isGimbalStick = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsZoomKnob]
        /// [Byte #9.] 1번째 비트
        /// </summary>
        public bool IsZoomKnob
        {
            get => _isZoomKnob;
            set
            {
                if (_isZoomKnob != value)
                {
                    _isZoomKnob = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [IsFocusKnob]
        /// [Byte #9.] 0번째(LSB) 비트
        /// </summary>
        public bool IsFocusKnob
        {
            get => _isFocusKnob;
            set
            {
                if (_isFocusKnob != value)
                {
                    _isFocusKnob = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ZoomChange]
        /// [Byte #10.]
        /// </summary>
        public int ZoomChange
        {
            get => _zoomChange;
            set
            {
                if (_zoomChange != value)
                {
                    _zoomChange = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [FocusChange]
        /// [Byte #10.]
        /// </summary>
        public int FocusChange
        {
            get => _focusChange;
            set
            {
                if (_focusChange != value)
                {
                    _focusChange = value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ZoyStickXChange]
        /// [Byte #11.]
        /// </summary>
        public int JoyStickXChange
        {
            get => _joyStickXChange;
            set
            {
                if (_joyStickXChange != value)
                {
                    _joyStickXChange = value;
                    int elipseJoyStickX = value > 80 ? 80 : value;
                    ElipseJoyStickX = 60 + (elipseJoyStickX * 0.6);
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ElipseJoyStickX]
        /// </summary>
        public double ElipseJoyStickX
        {
            get => _elipseJoyStickX;
            set
            {
                if (_elipseJoyStickX != value)
                {
                    _elipseJoyStickX = value >= 5 && value < 80 ? value - 5 : value;
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [JoyStickYChange]
        /// [Byte #12.]
        /// </summary>
        public int JoyStickYChange
        {
            get => _joyStickYChange;
            set
            {
                if (_joyStickYChange != value)
                {
                    _joyStickYChange = value;
                    int elipseJoyStickY = value > 80 ? 80 : value;
                    ElipseJoyStickY = 60 + (elipseJoyStickY * 0.6);
                    OnPropertyChanged();
                }

            }

        }

        /// <summary>
        /// [ElipseJoyStickY]
        /// </summary>
        public double ElipseJoyStickY
        {
            get => _elipseJoyStickY;
            set
            {
                if (_elipseJoyStickY != value)
                {
                    _elipseJoyStickY = value >= 5 && value < 80 ? value - 5 : value;
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

        public ICommand AltitudeOnOffCommand { get; set; }

        public ICommand HeadingOnOffCommand { get; set; }

        public ICommand SpeedOnOffCommand { get; set; }

        #endregion

        #region 생성자 (Initialize)

        public MainVM()
        {
            StartSerialCommand = new RelayCommand(StartSerial);
            AltitudeOnOffCommand = new RelayCommand(AltitudeToggle);
            HeadingOnOffCommand = new RelayCommand(HeadingToggle);
            SpeedOnOffCommand = new RelayCommand(SpeedToggle);
            EnvironmentSet = new EnvironmentSet();
            LstBoxItem = new ObservableCollection<string>();
            LstPortNames = EnvironmentSet.GetPortNames();
            LstBaudRates = EnvironmentSet.GetBaudRates();
            ElipseYawX = 60;
            ElipseThrottleY = 60;
            ElipseRollX = 60;
            ElipsePitchY = 60;
            ElipseJoyStickX = 60;
            ElipseJoyStickY = 60;
        }

        private void SpeedToggle()
        {
            IsSpeedOn = !IsSpeedOn;
        }

        private void HeadingToggle()
        {
            IsHeadingOn = !IsHeadingOn;
        }

        private void AltitudeToggle()
        {
            IsAltitudeOn = !IsAltitudeOn;
        }

        private void TransmitMessage()
        {
            CPCtoCCUField field = new CPCtoCCUField
            {
                IsAltitudeOn = IsAltitudeOn,
                IsHeadingOn = IsHeadingOn,
                IsSpeedOn = IsSpeedOn,
                AltitudeTotalSum = TotalAltitudeChange,
                HeadingTotalSum = TotalHeadingChange * 10,
                SpeedTotalSum = TotalSpeedChange * 10
            };
            OnMessageTransmit(field, DateTime.Now);
        }

        #endregion

        #region [버튼 및 기능]

        private void StartSerial()
        {
            if (EnvironmentSet == null || SelectedPort == null || SelectedBaudRate == 0)
            {
                return;
            }
            IsPortConnected = EnvironmentSet.OpenToClose(SelectedPort, SelectedBaudRate);
            if (IsPortConnected)
            {
                EnvironmentSet.MessageReceived += OnMessageReceived;
                AddLogMessage($"Connected to {SelectedPort}");
            }
            else
            {
                AddLogMessage($"Disconnected");
            }

        }

        private void OnMessageTransmit(CPCtoCCUField field, DateTime currentTime)
        {
            try
            {
                Parser parser = new Parser();
                int msgLen = 0;
                byte[] data = parser.ParseSender(field, ref msgLen);
                byte[] encodingData = parser.CheckDataCondition2(data);
                if (encodingData != null)
                {
                    EnvironmentSet.serialPort.Write(encodingData, 0, encodingData.Length);
                    Console.WriteLine("Data transmitted: " + BitConverter.ToString(encodingData));
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                Debug.WriteLine("Message parsing completed at " + "[" + currentTime + "]");
                Debug.WriteLine("");
            }

        }

        private void OnMessageReceived(byte[] messageListen, DateTime currentTime)
        {
            try
            {
                Parser parser = new Parser();
                parser.RunExample();
                CCUtoCPCField parserData = parser.ParseReceiver(messageListen);
                if (parserData != null)
                {
                    IsPowerSwitch = parserData.PowerSwitch == 1;
                    IsEngineStart = parserData.EngineStart == 1;
                    IsEngineRestart = parserData.EngineRestart == 1;
                    IsEngineKill = parserData.EngineKill == 1;
                    IsTakeOff = parserData.TakeOff == 1;
                    IsReturnToBase = parserData.ReturnToBase == 1;
                    IsAltitudeKnob = parserData.AltitudeKnob == 1;
                    IsHeadingKnob = parserData.HeadingKnob == 1;
                    IsSpeedKnob = parserData.SpeedKnob == 1;
                    AltitudeKnobChange = parserData.AltitudeKnobChange;
                    AltitudeKnobSum += AltitudeKnobChange;
                    TotalAltitudeChange = AltitudeKnobSum;
                    TransmitMessage();
                    HeadingKnobChange = parserData.HeadingKnobChange;
                    HeadingKnobSum += HeadingKnobChange;
                    TotalHeadingChange = HeadingKnobSum;
                    TransmitMessage();
                    SpeedKnobChange = parserData.SpeedKnobChange;
                    SpeedKnobSum += SpeedKnobChange;
                    TotalSpeedChange = SpeedKnobSum;
                    TransmitMessage();
                    YawChange = parserData.YawChange;
                    ThrottleChange = parserData.ThrottleChange;
                    RollChange = parserData.RollChange;
                    PitchChange = parserData.PitchChange;
                    IsDrop = parserData.Drop == 1;
                    IsOption1 = parserData.Option1 == 1;
                    IsCapture = parserData.Capture == 1;
                    IsEOandIR = parserData.EOandIR == 1;
                    IsOption2 = parserData.Option2 == 1;
                    IsGimbalStick = parserData.GimbalStick == 1;
                    IsZoomKnob = parserData.ZoomKnob == 1;
                    IsFocusKnob = parserData.FocusKnob == 1;
                    ZoomChange = parserData.ZoomChange;
                    FocusChange = parserData.FocusChange;
                    JoyStickXChange = parserData.JoyStickXChange;
                    JoyStickYChange = parserData.JoyStickYChange;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                //Debug.WriteLine("Message parsing completed at " + "[" + currentTime + "]");
                //Debug.WriteLine("");
            }

        }

        private void AddLogMessage(string msg)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss-ffff");
            LstBoxItem.Add($"[{timestamp}] {msg}");
        }
        #endregion
    }

}
