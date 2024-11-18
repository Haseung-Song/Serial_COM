using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;

namespace Serial_COM.Models
{
    public class EnvironmentSet
    {
        #region [프로퍼티]

        public SerialPort serialPort;
        public event Action<byte[]> MessageReceived;
        public bool isSingleRead;

        private readonly byte[] encryptionKey = GenerateKey();
        private readonly string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        #endregion

        #region 생성자 (Initialize)

        public EnvironmentSet()
        {
            serialPort = new SerialPort();
        }

        #endregion

        #region [함수 및 기능]

        /// <summary>
        /// [GetPortNames()] 함수
        /// 시스템 [포트 이름] 가져오기 (기능)
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// [GetBaudRates()] 함수
        /// 정의된 [포트 속도] 가져오기 (기능)
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// [OpenToClose] 메서드
        /// [Connect/Disconnect] (기능)
        /// </summary>
        /// <param name="portName">포트 이름</param>
        /// <param name="baudRate">포트 속도</param>
        /// <param name="singleReadMode">읽기 모드</param>
        /// <returns></returns>
        public bool OpenToClose(string portName, int baudRate, bool singleReadMode)
        {
            if (serialPort != null)
            {
                try
                {
                    // [시리얼 포트] (열기)
                    if (!serialPort.IsOpen)
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
                        if (baudRate == 115200)
                        {
                            isSingleRead = singleReadMode;
                            serialPort.DataReceived += OnDataReceived;
                        }
                        return true;
                    }
                    // [시리얼 포트] (닫기)
                    else
                    {
                        serialPort.Close();
                        Console.WriteLine("Serial_COM closed successfully on " + portName + " port.");
                        serialPort.DataReceived -= OnDataReceived;
                        serialPort.DiscardInBuffer();  // [수신 버퍼] 데이터 삭제
                        serialPort.DiscardOutBuffer(); // [전송 버퍼] 데이터 삭제
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

        /// <summary>
        /// [GetLogFilePath()] 메서드
        /// [로그 파일 전체 경로 반환] 및 [디렉토리 생성]
        /// </summary>
        /// <param name="logFileName">로그 파일명</param>
        /// <returns>로그 파일 전체 경로</returns>
        private string GetLogFilePath(string logFileName = "SerialComLog.txt")
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory); // 로그 폴더 생성
            }
            return Path.Combine(logDirectory, logFileName);
        }

        /// <summary>
        /// [LogMessageData()] 메서드
        /// [로그 메시지] 기록 (기능)
        /// </summary>
        /// <param name="logMsg">로그 메시지</param>
        /// <param name="logFileName">로그 파일명</param>
        private void LogMessageData(string logMsg, string logFileName = "SerialComLog.txt", bool addEmptyLine = false)
        {
            string logFilePath = GetLogFilePath(logFileName);
            string timeStampedMsg = $"{DateTime.Now:[yyyy-MM-dd HH:mm:ss]} - {logMsg}";
            if (addEmptyLine)
            {
                timeStampedMsg += Environment.NewLine;
            }
            File.AppendAllText(logFilePath, timeStampedMsg + Environment.NewLine);
        }

        /// <summary>
        /// [IntializeLogFile()] 메서드
        /// [로그 파일] 초기화 (기능)
        /// </summary>
        /// <param name="logFileName">로그 파일명</param>
        public void IntializeLogFile(string logFileName = "SerialComLog.txt")
        {
            string logFilePath = GetLogFilePath(logFileName);
            File.WriteAllText(logFilePath, string.Empty); // 로그 파일 초기화!
        }

        /// <summary>
        /// [OnDataReceived]: [수신 데이터] 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Parser parser = new Parser();
                // 1) [ReadByte()] 메서드 : 바이트를 한 번에 각각 [한 바이트씩] 수신
                // ## [isSingleRead = SingleReadMode] ##
                if (isSingleRead)
                {
                    while (serialPort.BytesToRead > 0)
                    {
                        int currentByte = serialPort.ReadByte();
                        byte byteData = (byte)currentByte;
                        byte[] decodingData = parser.CheckEachDecodingDataCondition(byteData);
                        if (decodingData != null)
                        {
                            LogMessageData($"OriginDecodedData (Hex) : {BitConverter.ToString(decodingData)}");
                            LogMessageData("", addEmptyLine: true); // 빈 줄 추가
                            byte[] encryptedEachData = Encryption(decodingData); // 암호화
                            byte[] decryptedEachData = Decryption(encryptedEachData); // 복호화
                            LogMessageData($"EncryptedEachData (Hex): {BitConverter.ToString(encryptedEachData)}"); // [암호화] 로그 데이터
                            LogMessageData($"DecryptedEachData (Hex): {BitConverter.ToString(decryptedEachData)}"); // [복호화] 로그 데이터
                            LogMessageData("", addEmptyLine: true); // 빈 줄 추가
                            MessageReceived?.Invoke(decryptedEachData);
                            Console.Write("Message (Single) (Encrypted): ");
                            foreach (byte dd in encryptedEachData)
                            {
                                Console.Write($"{dd:x2} ");
                            }
                            Console.WriteLine();
                            Console.Write("Message (Single) (Decrypted): ");
                            foreach (byte dd in decryptedEachData)
                            {
                                Console.Write($"{dd:x2} ");
                            }
                            Console.WriteLine();
                        }

                    }

                }
                // 2) [BytesToRead] 메서드 : [수신 버퍼]에 있는 바이트를 통으로 수신
                // ## [isSingleRead = !SingleReadMode] ##
                else
                {
                    int bytesReader = serialPort.BytesToRead;
                    byte[] buffer = new byte[bytesReader];
                    int bytesBuffer = serialPort.Read(buffer, 0, bytesReader);
                    byte[] decodingData = parser.CheckFullDecodingDataCondition(buffer);
                    if (decodingData != null)
                    {
                        LogMessageData($"OriginDecodedData (Hex) : {BitConverter.ToString(decodingData)}");
                        LogMessageData("", addEmptyLine: true); // 빈 줄 추가
                        byte[] encryptedFullData = Encryption(decodingData); // 암호화
                        byte[] decryptedFullData = Decryption(encryptedFullData); // 복호화
                        LogMessageData($"EncryptedEachData (Hex): {BitConverter.ToString(encryptedFullData)}"); // [암호화] 로그 데이터
                        LogMessageData($"DecryptedEachData (Hex): {BitConverter.ToString(decryptedFullData)}"); // [복호화] 로그 데이터
                        LogMessageData("", addEmptyLine: true); // 빈 줄 추가
                        MessageReceived?.Invoke(decryptedFullData);
                        Console.Write("Message (Buffer) (Encrypted): ");
                        foreach (byte dd in encryptedFullData)
                        {
                            Console.Write($"{dd:x2} ");
                        }
                        Console.WriteLine();
                        Console.Write("Message (Buffer) (Decrypted): ");
                        foreach (byte dd in decryptedFullData)
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
            }

        }

        /// <summary>
        /// [난수 생성기]
        /// </summary>
        /// <returns></returns>
        private static byte[] GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                return aes.Key; // 자동으로 유효한 길이로 생성
            }

        }

        /// <summary>
        /// [EncryptData]: 데이터 암호화
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        private byte[] Encryption(byte[] plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.GenerateIV();
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length); // IV 저장
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plainText, 0, plainText.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }

            }

        }

        /// <summary>
        /// [DecryptData]: 데이터 복호화
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        private byte[] Decryption(byte[] cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    byte[] iv = new byte[16]; // AES 기본 IV 크기
                    int ivBuffer = ms.Read(iv, 0, iv.Length);
                    aes.IV = iv;
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (MemoryStream decryptedStream = new MemoryStream())
                        {
                            cs.CopyTo(decryptedStream);
                            return decryptedStream.ToArray();
                        }

                    }

                }

            }

        }
        #endregion
    }

}
