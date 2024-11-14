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
        #region [프로퍼티]

        public SerialPort serialPort;
        public event Action<byte[]> MessageReceived;

        #endregion

        #region 생성자 (Initialize)

        public EnvironmentSet()
        {
            serialPort = new SerialPort();
        }

        #endregion

        #region [함수 및 기능]

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
                    // [시리얼 포트] (닫기)
                    else
                    {
                        serialPort.Close();
                        Console.WriteLine("Serial_COM closed successfully on " + portName + " port.");
                        serialPort.DataReceived -= OnDataReceived;
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
                    return false;
                }
                catch (ArgumentNullException ex)
                {
                    Debug.WriteLine(ex.ToString());
                    return false;
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine(ex.ToString());
                    _ = MessageBox.Show($"지정한 '{portName}' 포트가 올바른 직렬 포트로 확인되지 않습니다.", "통신 실패", MessageBoxButton.OK, MessageBoxImage.Error);
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
            Parser parser = new Parser();
            try
            {
                // 1) [BytesToRead] 함수 : [수신 버퍼]에 있는 바이트를 통으로 수신!
                //int bytesToRead = serialPort.BytesToRead;
                //byte[] buffer = new byte[bytesToRead];
                //int bytesToSave = serialPort.Read(buffer, 0, bytesToRead);
                //byte[] decodingData1 = parser.CheckFullDecodingDataCondition(buffer);
                //MessageReceived?.Invoke(decodingData1);
                //Console.Write("Message: ");
                //foreach (byte dd in decodingData1)
                //{
                //    Console.Write($"{dd:x2} ");
                //}
                //Console.WriteLine();
                // 2) [ReadByte()] 함수 : 바이트를 한 번에 각각 [한 바이트씩] 수신!
                while (serialPort.BytesToRead > 0)
                {
                    int currentByte = serialPort.ReadByte();
                    byte byteData = (byte)currentByte;
                    byte[] decodingData2 = parser.CheckEachDecodingDataCondition(byteData);
                    if (decodingData2 != null)
                    {
                        MessageReceived?.Invoke(decodingData2);
                        Console.Write("Message: ");
                        foreach (byte dd in decodingData2)
                        {
                            Console.Write($"{dd:x2} ");
                        }
                        Console.WriteLine();
                    }

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("Message received at " + "[" + DateTime.Now + "]");
                Console.WriteLine();
            }

        }
        #endregion
    }

}
