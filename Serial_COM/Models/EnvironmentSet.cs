using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Windows;

namespace Serial_COM.Models
{
    public class EnvironmentSet
    {
        public SerialPort serialPort;
        public event Action<byte[], DateTime> MessageReceived;

        public EnvironmentSet()
        {
            serialPort = new SerialPort();
        }

        public List<string> GetPortNames()
        {
            List<string> lstPortNames = new List<string>();
            string[] ports = SerialPort.GetPortNames();
            foreach (string portName in ports)
            {
                lstPortNames.Add(portName);
            }
            // 숫자 부분으로 정렬  (COM3, COM4...)
            List<string> lstSortedPN = new List<string>(lstPortNames.OrderBy(item => int.Parse(item.Substring(3))));
            return lstSortedPN;
        }

        public List<int> GetBaudRates()
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
            List<int> lstSortedBR = new List<int>(lstBaudRates.OrderBy(item => item));
            return lstSortedBR;
        }

        public bool OpenToClose(string portName, int baudRate)
        {
            if (serialPort != null)
            {
                try
                {
                    // [시리얼 포트] (열기)
                    if (!serialPort.IsOpen)
                    {
                        if (portName == "COM11")
                        {
                            if (baudRate == 115200)
                            {
                                serialPort = new SerialPort(portName, baudRate)
                                {
                                    DataBits = 8, // Data bits: 8
                                    Parity = Parity.None, // Parity: None
                                    StopBits = StopBits.One,  // Stop bits: 1
                                    Handshake = Handshake.None, // Flow Control: None
                                    ReadTimeout = 500, // 데이터 읽기 타임아웃 설정 (0.5초)
                                    WriteTimeout = 500 // 데이터 쓰기 타임아웃 설정 (0.5초)
                                };
                                serialPort.Open();
                                _ = MessageBox.Show($"'{portName}' 포트가 열렸습니다.", "통신 성공", MessageBoxButton.OK, MessageBoxImage.Information);
                                Console.WriteLine("Serial_COM opened successfully on " + portName + " port.");
                                serialPort.DataReceived += OnDataReceived;
                                return true;
                            }
                            else
                            {
                                _ = MessageBox.Show($"'{portName}' 포트가 닫혔습니다.", "통신 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                                throw new InvalidOperationException($"'{portName}' 포트가 닫혔습니다. 알맞은 'Baudrate:115200'으로 설정하세요.");
                            }

                        }
                        else
                        {
                            serialPort = new SerialPort(portName, baudRate);
                            serialPort.Open();
                            _ = MessageBox.Show($"'{portName}' 포트가 열렸습니다.", "통신 성공", MessageBoxButton.OK, MessageBoxImage.Information);
                            Console.WriteLine("Serial_COM opened successfully on " + portName + " port.");
                            return true;
                        }

                    }
                    // [시리얼 포트] (닫기)
                    else
                    {
                        serialPort.Close();
                        Console.WriteLine("Serial_COM closed successfully on " + portName + " port.");
                        serialPort.Dispose();
                        return false;
                    }

                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.WriteLine(ex.ToString());
                    _ = MessageBox.Show($"'{portName}' 포트에 대한 액세스가 거부되었습니다.", "통신 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Debug.WriteLine(ex.ToString());
                    _ = MessageBox.Show($"'Baudrate'가 설정되지 않았습니다.", "통신 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    return false;
                }

            }
            return false;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int bytesToRead = serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                int bytesToSave = serialPort.Read(buffer, 0, bytesToRead);
                Debug.WriteLine($"Total [{bytesToSave} bytes] reads from '{serialPort.PortName}' port.");
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

    }

}
