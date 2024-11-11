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
            byte[] filteredData1 = CheckDecodingDataCondition(examplesData1);

            //Console.WriteLine("Original Data: " + BitConverter.ToString(examplesData1));
            //Console.WriteLine("Filtered Data (with DLE Escaping): " + BitConverter.ToString(filteredData1));

            // Original Data: 02-10-02-10-10-10-03-38-39-10-03 (일치)
            // Filtered Data (with DLE Escaping): 02-02-10-03-38-39-10-03 (일치)

            // 예시 데이터 [exampleData2] 설정: 목적지 0x22, 송신지 0x33, 메시지: 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x10, 0x11
            byte[] examplesData2 = { STX, 0x08, 0x22, 0x33, 0x00, 0x01, DLE, 0x02, DLE, 0x03, 0x04, 0x05, DLE, 0x10, 0x11, ETX };
            byte[] filteredData2 = CheckDecodingDataCondition(examplesData2);

            //Console.WriteLine("Original Data: " + BitConverter.ToString(examplesData2));
            //Console.WriteLine("Filtered Data (with DLE Escaping): " + BitConverter.ToString(filteredData2));

            // Original Data: 02-08-22-33-00-01-10-02-10-03-04-05-10-10-11-03 (일치)
            // Filtered Data (with DLE Escaping): 02-08-22-33-00-01-02-03-04-05-10-11-19-03 (일치)
        }

        /// <summary>
        /// [(Message Parsing) 송신부]
        /// </summary>
        /// <param name="field"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] ParseSender(CPCtoCCUField field, ref int len)
        {
            try
            {
                int nCnt = 0;
                byte[] data = new byte[8];
                // 비행조종장치 LCD SET (Altitude)
                // [Byte #0.]
                // 7번째 비트(MSB)를 추출
                if (field.IsAltitudeOn)
                {
                    data[nCnt] |= 1 << 7;
                }
                // 비행조종장치 LCD SET (Heading)
                // [Byte #0.]
                // 6번째 비트를 추출
                if (field.IsHeadingOn)
                {
                    data[nCnt] |= 1 << 6;
                }
                // 비행조종장치 LCD SET (Speed)
                // [Byte #0.]
                // 5번째 비트를 추출
                if (field.IsSpeedOn)
                {
                    data[nCnt] |= 1 << 5;
                }
                nCnt++; // Next Byte 진행

                // 비행조종장치 고도(Altitude) 표시 값
                ushort altitudeValue = (ushort)field.TotalAltitudeChange;
                // [Byte #2.] 상위 바이트(MSB)
                data[nCnt++] = (byte)((altitudeValue >> 8) & 0xFF);
                // [Byte #3.] 하위 바이트(LSB)
                data[nCnt++] = (byte)(altitudeValue & 0xFF);

                // 비행조종장치 헤딩(Heading) 표시 값
                ushort headingValue = (ushort)field.TotalHeadingChange;
                // [Byte #4.] 상위 바이트(MSB)
                data[nCnt++] = (byte)((headingValue >> 8) & 0xFF);
                // [Byte #5.] 하위 바이트(LSB)
                data[nCnt++] = (byte)(headingValue & 0xFF);

                // 비행조종장치 속도(Speed) 표시 값
                ushort speedValue = (ushort)field.TotalSpeedChange;
                // [Byte #6.] 상위 바이트(MSB)
                data[nCnt++] = (byte)((speedValue >> 8) & 0xFF);
                // [Byte #7.] 하위 바이트(LSB)
                data[nCnt++] = (byte)(speedValue & 0xFF);

                len = nCnt;

                return data;
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"ArgumentException: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Exception: {ex.Message}");
                return null;
            }

        }

        /// <summary>
        /// [(Message Parsing) 수신부]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CCUtoCPCField ParseReceiver(byte[] data)
        {
            // filteredData = [CheckDataCondition] 함수에서 [DLE]를 제외한 실제 메시지를 포함!
            // msgData = [filteredData]에서 [STX, Length, Destination, Source] 4바이트를 제외!
            // 이후, [Checksum, ETX] 제외한 총 합 [6]을 빼준 값을 반환했을 때, 온전한 msgData! 
            try
            {
                byte[] decodingData = CheckDecodingDataCondition(data);
                byte[] msgData = decodingData.Skip(4).Take(decodingData.Length - 6).ToArray();
                using (ByteStream stream = new ByteStream(msgData, 0, msgData.Length))
                {
                    CCUtoCPCField field = new CCUtoCPCField
                    {
                        // 1) [GetBits] 메서드: [단일 바이트] 추출
                        // 바이트 스트림의 특정 위치 => 비트 추출 목적
                        // 첫 번째 인자: 시작 위치. (0 바이트부터 시작)
                        // 두 번째 인자: 추출할 바이트 수.
                        // 세 번째 인자: 비트 시작 위치.
                        // 네 번째 인자: 추출할 비트 수.

                        // 2) [GetByte] 메서드: [단일 바이트] 추출
                        // 바이트 스트림 특정 위치 => 1 Byte 추출 목적
                        // 첫 번째 인자: 시작 위치. (0 바이트부터 시작)

                        // [Byte #0.]
                        // 7    번째 비트(MSB)를 추출
                        PowerSwitch = (byte)stream.GetBits(0, 1, 7, 1),

                        // [Byte #1.]
                        // 7    번째 비트(MSB)를 추출
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
                        // 0    번째 비트(LSB)를 추출
                        SpeedKnob = (byte)stream.GetBits(1, 1, 0, 1),

                        // [Byte #2.]
                        AltitudeKnobChange = (sbyte)stream.GetByte(2),

                        // [Byte #3.]
                        HeadingKnobChange = (sbyte)stream.GetByte(3),

                        // [Byte #4.]
                        SpeedKnobChange = (sbyte)stream.GetByte(4),

                        // [Byte #5.]
                        YawChange = (sbyte)stream.GetByte(5),

                        // [Byte #6.]
                        ThrottleChange = (byte)stream.GetByte(6),

                        // [Byte #7.]
                        RollChange = (sbyte)stream.GetByte(7),

                        // [Byte #8.]
                        PitchChange = (sbyte)stream.GetByte(8),

                        // [Byte #9.]
                        // 7    번째 비트(MSB)를 추출
                        Drop = (byte)stream.GetBits(9, 1, 7, 1),
                        // 6    번째 비트를 추출
                        Option1 = (byte)stream.GetBits(9, 1, 6, 1),
                        // 5    번째 비트를 추출
                        Capture = (byte)stream.GetBits(9, 1, 5, 1),
                        // 4    번째 비트를 추출
                        EOandIR = (byte)stream.GetBits(9, 1, 4, 1),
                        // 3    번째 비트를 추출
                        Option2 = (byte)stream.GetBits(9, 1, 3, 1),
                        // 2    번째 비트를 추출
                        GimbalStick = (byte)stream.GetBits(9, 1, 2, 1),
                        // 1    번째 비트를 추출
                        ZoomKnob = (byte)stream.GetBits(9, 1, 1, 1),
                        // 0    번째 비트(LSB)를 추출
                        FocusKnob = (byte)stream.GetBits(9, 1, 0, 1),

                        // [Byte #10.]
                        ZoomChange = (sbyte)stream.GetByte(10),

                        // [Byte #11.]
                        FocusChange = (sbyte)stream.GetByte(11),

                        // [Byte #12.]
                        JoyStickXChange = (sbyte)stream.GetByte(12),

                        // [Byte #13.]
                        JoyStickYChange = (sbyte)stream.GetByte(13),
                    };
                    return field;
                }

            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"ArgumentException: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Exception: {ex.Message}");
                return null;
            }

        }

        /// <summary>
        /// [GetEncodingData]
        /// </summary>
        /// <param name="field"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] GetEncodingData(CPCtoCCUField field, ref int len)
        {
            byte[] data = ParseSender(field, ref len);
            return data == null ? null : CheckEncodingDataCondition(data);
        }

        /// <summary>
        /// [CheckEncodingDataCondition]
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public byte[] CheckEncodingDataCondition(byte[] check)
        {
            List<byte> encodingData = new List<byte>();
            bool isCtrlText;
            byte checkSum = 0x00; // Checksum 초기화
            byte dstID = 0xC1; // CPC (dstID) 식별자
            byte srcID = 0xA5; // CCU (srcID) 식별자
            // 1. [STX](0x02) 추가
            encodingData.Add(STX);
            // 2. [STX](0x02) 이후
            byte msgLen = (byte)check.Length;
            isCtrlText = CheckCtrlText(msgLen);
            if (isCtrlText)
            {
                encodingData.Add(DLE);
            }
            encodingData.Add(msgLen);
            checkSum ^= msgLen;
            // 3. [DstID] 추가 (DLE 처리 포함)
            isCtrlText = CheckCtrlText(dstID);
            if (isCtrlText)
            {
                encodingData.Add(DLE);
            }
            encodingData.Add(dstID);
            checkSum ^= dstID;
            // 4. [SrcID] 추가 (DLE 처리 포함)
            isCtrlText = CheckCtrlText(srcID);
            if (isCtrlText)
            {
                encodingData.Add(DLE);
            }
            encodingData.Add(srcID);
            checkSum ^= srcID;
            // 5. [MsgData] 추가(DLE 처리 포함)
            foreach (byte msgData in check)
            {
                isCtrlText = CheckCtrlText(msgData);
                if (isCtrlText)
                {
                    encodingData.Add(DLE);
                }
                encodingData.Add(msgData);
                checkSum ^= msgData;
            }
            // 6. [Checksum] 추가 (DLE 처리 포함)
            isCtrlText = CheckCtrlText(checkSum);
            if (isCtrlText)
            {
                encodingData.Add(DLE);
            }
            encodingData.Add(checkSum);
            // 7. [ETX](0x03) 추가
            encodingData.Add(ETX);
            return encodingData.ToArray();
        }

        /// <summary>
        /// [제어문자 여부 확인]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool CheckCtrlText(byte data)
        {
            return data == STX || data == ETX || data == DLE;
        }

        /// <summary>
        /// [CheckDecodingDataCondition]
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
        public byte[] CheckDecodingDataCondition(byte[] check)
        {
            List<byte> decodingData = new List<byte>();
            // [DLE] Flag 설정
            bool isCtrlText = false;
            // 1. [STX](0x02) 추가
            decodingData.Add(STX);
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
                    decodingData.Add(currentByte);
                    isCtrlText = false;
                }
                // [DLE] Flag 설정을 하지 않을 시,
                // [일반 메시지] 간주 && => [일반 메시지] 추가
                else
                {
                    decodingData.Add(currentByte);
                }

            }
            // 3. [Length] ~ [Message] 이후, [Checksum 단계]: [XOR] 반복 계산
            byte checkSum = CalculateChecksum(decodingData.Skip(1).ToArray());
            // 4. [Checksum]  추가
            decodingData.Add(checkSum);
            // 5. [ETX](0x03) 추가
            decodingData.Add(ETX);
            return decodingData.ToArray();
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

        /// <summary>
        /// [메시지 정의(CPCtoCCU)]
        /// </summary>
        public class CPCtoCCUField
        {
            public bool IsAltitudeOn { get; set; }

            public bool IsHeadingOn { get; set; }

            public bool IsSpeedOn { get; set; }

            public byte Altitude { get; set; }

            public byte Heading { get; set; }

            public byte Speed { get; set; }

            public double TotalAltitudeChange { get; set; }

            public double TotalHeadingChange { get; set; }

            public double TotalSpeedChange { get; set; }
        }

        /// <summary>
        /// [메시지 정의(CCUtoCPC)]
        /// </summary>
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

            /// <summary>
            /// [비행조종장치 고도(Altitude) 노브]
            /// [Altitude Knob Change]
            /// [Byte #2.]
            /// </summary>
            public sbyte AltitudeKnobChange { get; set; }

            /// <summary>
            /// [비행조종장치 헤딩(Heading) 노브]
            /// [Heading Knob Change]
            /// [Byte #3.]
            /// </summary>
            public sbyte HeadingKnobChange { get; set; }

            /// <summary>
            /// [비행조종장치 속도(Speed) 노브]
            /// [Speed Knob Change]
            /// [Byte #4.]
            /// </summary>
            public sbyte SpeedKnobChange { get; set; }

            /// <summary>
            /// [비행조종장치 조이스틱 Yaw]
            /// [Speed Knob Change]
            /// [Byte #5.]
            /// </summary>
            public sbyte YawChange { get; set; }

            /// <summary>
            /// [비행조종장치 조이스틱 Throttle]
            /// [Speed Knob Change]
            /// [Byte #6.]
            /// </summary>
            public byte ThrottleChange { get; set; }

            /// <summary>
            /// [비행조종장치 조이스틱 Roll]
            /// [Speed Knob Change]
            /// [Byte #7.]
            /// </summary>
            public sbyte RollChange { get; set; }

            /// <summary>
            /// [비행조종장치 조이스틱 Pitch]
            /// [Speed Knob Change]
            /// [Byte #8.]
            /// </summary>
            public sbyte PitchChange { get; set; }

            /// <summary>
            /// [임무조종장치 스위치]
            /// [Drop]
            /// [Byte #9.] 7번째(MSB) 비트
            /// </summary>
            public byte Drop { get; set; }

            /// <summary>
            /// [임무조종장치 스위치]
            /// [Option1]
            /// [Byte #9.] 6번째 비트
            /// </summary>
            public byte Option1 { get; set; }

            /// <summary>
            /// [임무조종장치 스위치]
            /// [Capture]
            /// [Byte #9.] 5번째 비트
            /// </summary>
            public byte Capture { get; set; }

            /// <summary>
            /// [임무조종장치 스위치]
            /// [EOandIR]
            /// [Byte #9.] 4번째 비트
            /// </summary>
            public byte EOandIR { get; set; }

            /// <summary>
            /// [임무조종장치 스위치]
            /// [Option2]
            /// [Byte #9.] 3번째 비트
            /// </summary>
            public byte Option2 { get; set; }

            /// <summary>
            /// [임무조종장치 스위치]
            /// [Gimbal Stick]
            /// [Byte #9.] 2번째 비트
            /// </summary>
            public byte GimbalStick { get; set; }

            /// <summary>
            /// [임무조종장치 스위치]
            /// [ZoomKnob]
            /// [Byte #9.] 1번째 비트
            /// </summary>
            public byte ZoomKnob { get; set; }

            /// <summary>
            /// [임무조종장치 스위치]
            /// [FocusKnob]
            /// [Byte #9.] 0번째(LSB) 비트
            /// </summary>
            public byte FocusKnob { get; set; }

            /// <summary>
            /// [임무조종장치 줌 노브]
            /// [ZoomChange]
            /// [Byte #10.]
            /// </summary>
            public sbyte ZoomChange { get; set; }

            /// <summary>
            /// [임무조종장치 포커스 노브]
            /// [FocusChange]
            /// [Byte #10.]
            /// </summary>
            public sbyte FocusChange { get; set; }

            /// <summary>
            /// [임무조종장치 조이스틱 X]
            /// [JoyStickXChange]
            /// [Byte #11.]
            /// </summary>
            public sbyte JoyStickXChange { get; set; }

            /// <summary>
            /// [임무조종장치 조이스틱 Y]
            /// [JoyStickYChange]
            /// [Byte #12.]
            /// </summary>
            public sbyte JoyStickYChange { get; set; }
        }

    }

    /// <summary>
    /// [FieldChangeExtension]
    /// </summary>
    public static class FieldChangeExtension
    {
        /// <summary>
        /// [0 ~ 255] 범위 => [-127 ~ 127] 범위 변환!
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertAHSToSignedRange(this byte value)
        {
            // 기존 [0 ~ 255] 범위
            int cvtData = Convert.ToInt16(value);
            // 기존 [128 ~ 255] => [-127 ~ 0] 범위 변환!
            if (cvtData > 127)
            {
                cvtData -= 255;
            }
            return cvtData;
        }

    }

}
