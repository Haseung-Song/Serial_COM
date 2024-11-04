using Soletop.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Serial_COM.Models
{
    public class Parser
    {
        private const byte STX = 0x02;
        private const byte DLE = 0x10;
        private const byte ETX = 0x03;

        /// <summary>
        /// [예시 데이터 함수]
        /// </summary>
        public void RunExample()
        {
            // 예시 데이터 [exampleData1] 설정: 목적지 0x10, 송신지 0x03, 메시지: 0x38, 0x39
            byte[] examplesData1 = { STX, DLE, 0x02, DLE, 0x10, DLE, 0x03, 0x38, 0x39, DLE, ETX };
            byte[] filteredData1 = CheckDataCondition(examplesData1);
            //Console.WriteLine("Original Data: " + BitConverter.ToString(examplesData1));
            //Console.WriteLine("Filtered Data (with DLE Escaping): " + BitConverter.ToString(filteredData1));

            // Original Data: 02-10-02-10-10-10-03-38-39-10-03 (일치)
            // Filtered Data (with DLE Escaping): 02-02-10-03-38-39-10-03 (일치)

            // 예시 데이터 [exampleData2] 설정: 목적지 0x22, 송신지 0x33, 메시지: 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x10, 0x11
            byte[] examplesData2 = { STX, 0x08, 0x22, 0x33, 0x00, 0x01, DLE, 0x02, DLE, 0x03, 0x04, 0x05, DLE, 0x10, 0x11, ETX };
            byte[] filteredData2 = CheckDataCondition(examplesData2);
            //Console.WriteLine("Original Data: " + BitConverter.ToString(examplesData2));
            //Console.WriteLine("Filtered Data (with DLE Escaping): " + BitConverter.ToString(filteredData2));

            // Original Data: 02-08-22-33-00-01-10-02-10-03-04-05-10-10-11-03 (일치)
            // Filtered Data (with DLE Escaping): 02-08-22-33-00-01-02-03-04-05-10-11-19-03 (일치)
        }

        public CCUtoCPCField Parse(byte[] data)
        {
            // filteredData = [CheckDataCondition] 함수에서 [DLE]를 제외한 실제 메시지를 포함!
            // msgData = [filteredData]에서 [STX, Length, Destination, Source] 4바이트를 제외!
            // 이후, [Checksum, ETX] 제외한 총 합 [6]을 빼준 값을 반환했을 때, 온전한 msgData! 
            try
            {
                byte[] filteredData = CheckDataCondition(data);
                byte[] msgData = filteredData.Skip(4).Take(filteredData.Length - 6).ToArray();
                using (ByteStream stream = new ByteStream(msgData, 0, msgData.Length))
                {
                    CCUtoCPCField field = new CCUtoCPCField
                    {
                        // # [GetBits] 메서드 #
                        // 바이트 스트림의 특정 위치에서 비트 추출 역할
                        // 첫 번째 인자: 시작 위치. (0 바이트부터 시작)
                        // 두 번째 인자: 추출할 바이트 수.
                        // 세 번째 인자: 비트 시작 위치.
                        // 네 번째 인자: 추출할 비트 수.

                        // [Byte #0.]
                        // 7    번째 비트를 추출
                        PowerSwitch = (byte)stream.GetBits(0, 1, 7, 1),

                        // [Byte #1.]
                        // 7    번째 비트를 추출
                        EngineStart = (byte)stream.GetBits(1, 1, 7, 1),
                        // 6    번째 비트를 추출
                        EngineRestart = (byte)stream.GetBits(1, 1, 6, 1),
                        // 5    번째 비트를 추출
                        EngineKill = (byte)stream.GetBits(1, 1, 5, 1),
                        // 4    번째 비트를 추출
                        TakeOff = (byte)stream.GetBits(1, 1, 4, 1),
                        // 3    번째 비트를 추출
                        ReturnToBase = (byte)stream.GetBits(1, 1, 3, 1),
                        // 2    번째 비트를 추출
                        AltitudeKnob = (byte)stream.GetBits(1, 1, 2, 1),
                        // 1    번째 비트를 추출
                        HeadingKnob = (byte)stream.GetBits(1, 1, 1, 1),
                        // 0    번째 비트를 추출
                        SpeedKnob = (byte)stream.GetBits(1, 1, 0, 1),
                    };
                    // [Byte #0.]
                    Console.Write(field.PowerSwitch + " ");

                    // [Byte #1.]
                    Console.Write(field.EngineStart + " ");
                    Console.Write(field.EngineRestart + " ");
                    Console.Write(field.EngineKill + " ");
                    Console.Write(field.TakeOff + " ");
                    Console.Write(field.ReturnToBase + " ");
                    Console.Write(field.AltitudeKnob + " ");
                    Console.Write(field.HeadingKnob + " ");
                    Console.WriteLine(field.SpeedKnob + " ");

                    return field;
                }

            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"ArgumentException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Exception: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// *. [프로토콜 샘플코드] .*
        /// #. [DecodingPacket] 함수: [DLE](Data Link Escape) 프로토콜을 사용하는 패킷을 디코딩 하는 방식
        /// 1. [패킷 시작] 부분 확인: STX 수신 시, [s_IsStartPacket] 변수를 1로 설정해 [패킷 시작]을 표시
        /// 2. [데이터]: 이후, 버퍼 [s_RcvBuff]에 저장
        /// 3. [DLE] 수신 후, [s_IsCtrlText] 플래그를 설정, 이는, 다음 바이트가 실제 데이터임을 알림
        /// 4. [DLE] 뒤에 오는 [STX], [ETX], [DLE] 등 제어 문자는 [데이터] 취급
        /// 5. [ETX] 확인: 즉, [수신된 메시지]의 끝을 의미하므로 [체크섬 계산] 단계로 돌입
        /// 6. [체크섬 계산]: [수신 체크섬] 값과 [계산 체크섬] 값을 비교
        /// 7. [체크섬 비교]: [일치] 시 [EPRS_OK] 출력, [불일치] 시 [EPRS_ERROR] 출력
        /// 8. [구조체 저장]: [정상] 수신 패킷은 [pstPacket] 구조체에 저장
        /// 9. [데이터 구분]: 각 필드(len, dest, src)로 데이터를 구분
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public byte[] CheckDataCondition(byte[] check)
        {
            List<byte> filteredData = new List<byte>();
            bool isCtrlText = false; // [DLE] Flag 설정
            // 1. [STX](0x02) 추가
            filteredData.Add(STX);
            // 2. [STX](0x02) 이후
            // [제어 문자] 및 [일반 메시지] 구분 코드!
            for (int i = 1; i < check.Length - 1; i++)
            {
                byte currentByte = check[i];
                // 1) [isCtrlText == true], [제어 문자] [DLE]로 구분
                // 2) [!isCtrlText](false), [일반 메시지] [0x10]으로 구분
                // 즉, [Flag] 설정 = [제어 문자], [일반 메시지] 구분 목적
                if (currentByte == DLE && !isCtrlText)
                {
                    isCtrlText = true; // [DLE] Flag 값 = true
                    continue; // 이후, 다음 [DLE]로 이동
                }
                // [DLE] Flag 설정 시, [제어 문자]로 간주 이후
                // [일반 메시지] 추가 && [DLE] Flag 값 = false
                if (isCtrlText)
                {
                    filteredData.Add(currentByte);
                    isCtrlText = false;
                }
                // [DLE] Flag 설정을 하지 않을 시,
                // [일반 메시지] 간주 && => [일반 메시지] 추가
                else
                {
                    filteredData.Add(currentByte);
                }

            }
            // 3. [Length] ~ [Message] 이후, [Checksum 단계]: [XOR] 반복 계산!
            byte checkSum = CalculateChecksum(filteredData.Skip(1).ToArray());
            // 4. [Checksum]  추가
            filteredData.Add(checkSum);
            // 5. [ETX](0x03) 추가
            filteredData.Add(ETX);
            return filteredData.ToArray();
        }

        /// <summary>
        /// [체크섬 계산]
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private byte CalculateChecksum(byte[] message)
        {
            byte checkSum = 0;
            foreach (byte msg in message)
            {
                checkSum ^= msg; // [RAW] 데이터만 XOR 연산
            }
            return checkSum;
        }

        public class CCUtoCPCField
        {
            /// <summary>
            /// [파워 스위치]
            /// [Power]
            /// [Byte #0.] 7번째(MSB) 비트
            /// </summary>
            public byte PowerSwitch { get; set; }

            /// <summary>
            /// [비행조종장치 스위치]
            /// [Engine Start]
            /// [Byte #1.] 7번째(MSB) 비트
            /// </summary>
            public byte EngineStart { get; set; }

            /// <summary>
            /// [비행조종장치 스위치]
            /// [Engine Restart]
            /// [Byte #1.] 6번째 비트
            /// </summary>
            public byte EngineRestart { get; set; }

            /// <summary>
            /// [비행조종장치 스위치]
            /// [Engine Kill]
            /// [Byte #1.] 5번째 비트
            /// </summary>
            public byte EngineKill { get; set; }

            /// <summary>
            /// [비행조종장치 스위치]
            /// [Take off]
            /// [Byte #1.] 4번째 비트
            /// </summary>
            public byte TakeOff { get; set; }

            /// <summary>
            /// [비행조종장치 스위치]
            /// [Return to Base]
            /// [Byte #1.] 3번째 비트
            /// </summary>
            public byte ReturnToBase { get; set; }

            /// <summary>
            /// [비행조종장치 스위치]
            /// [Altitude Knob]
            /// [Byte #1.] 2번째 비트
            /// </summary>
            public byte AltitudeKnob { get; set; }

            /// <summary>
            /// [비행조종장치 스위치]
            /// [Altitude Knob]
            /// [Byte #1.] 1번째 비트
            /// </summary>
            public byte HeadingKnob { get; set; }

            /// <summary>
            /// [비행조종장치 스위치]
            /// [Altitude Knob]
            /// [Byte #1.] 0번째(LSB) 비트
            /// </summary>
            public byte SpeedKnob { get; set; }
        }

    }

}
