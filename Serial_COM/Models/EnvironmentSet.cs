using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;

namespace Serial_COM.Models
{
    public class EnvironmentSet
    {
        public SerialPort _serialPort;

        public event Action<byte[], DateTime> MessageReceived;

        public ObservableCollection<string> GetPortNames()
        {
            List<string> lstPortNames = new List<string>();
            string[] ports = SerialPort.GetPortNames();
            foreach (string portName in ports)
            {
                lstPortNames.Add(portName);
            }
            // 숫자 부분으로 정렬 (COM3, COM4...)
            ObservableCollection<string> lstSortedPN =
                new ObservableCollection<string>(lstPortNames.OrderBy(item => int.Parse(item.Substring(3))));
            return lstSortedPN;
        }

        public ObservableCollection<int> GetBaudRates()
        {
            List<int> lstBaudRates = new List<int>
            {
                9600,
                19200,
                38400,
                57600,
                115200
            };
            // 숫자 순서대로 정렬 (9600, 19200...)
            ObservableCollection<int> lstSortedBR =
                new ObservableCollection<int>(lstBaudRates.OrderBy(item => item));
            return lstSortedBR;
        }

        public void Open(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate)
            {
                DataBits = 8, // Data bits: 8
                Parity = Parity.None, // Parity: None
                StopBits = StopBits.One,  // Stop bits: 1
                Handshake = Handshake.None, // Flow Control: None
                ReadTimeout = 500, // 데이터 읽기 타임아웃 설정 (0.5초)
                WriteTimeout = 500 // 데이터 쓰기 타임아웃 설정 (0.5초)
            };
            /// <summary>
            /// [시리얼 포트] (열기)
            /// </summary>
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
            _serialPort.DataReceived += OnDataReceived;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int bytesToRead = _serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _ = _serialPort.Read(buffer, 0, bytesToRead);
                Parser parser = new Parser();
                byte[] filteredData = parser.CheckDataCondition(buffer);
                foreach (byte fd in filteredData)
                {
                    Console.Write($"{fd:X2} ");
                }
                Console.WriteLine("");
                MessageReceived?.Invoke(buffer, DateTime.Now); // [수신 데이터] + [수신 메시지] 전달 이벤트 호출
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        /// <summary>
        /// [시리얼 포트] (닫기)
        /// </summary>
        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            Console.WriteLine("Serial_COM closed successfully on " + _serialPort.PortName);
        }

    }

}
