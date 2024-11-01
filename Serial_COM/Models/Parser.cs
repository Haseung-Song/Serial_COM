using Soletop.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Serial_COM.Models
{
    public class Parser
    {
        private const byte STX = 0x02;
        private const byte DLE = 0x10;
        private const byte ETX = 0x03;

        public CCUtoCPCField Parse(byte[] data)
        {
            try
            {
                using (ByteStream stream = new ByteStream(data, 0, data.Length))
                {
                    CheckDataCondition(data);
                    CCUtoCPCField field = new CCUtoCPCField
                    {
                        EngineStart = (byte)stream.GetBits(5, 1, 7, 1)
                    };
                    Console.WriteLine(field.EngineStart);
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

        public byte[] CheckDataCondition(byte[] check)
        {
            List<byte> filteredData = new List<byte>();
            // DLE(0x10)를 제외한 데이터를 필터링
            for (int i = 0; i < check.Length; i++)
            {
                if (check[i] == 0x10 && 
                    (check[i + 1] == 0x02 || check[i + 1] == 0x03 || check[i + 1] == 0x10))
                {
                    i++;
                }
                filteredData.Add(check[i]);
            }
            return filteredData.ToArray();
        }

        public class CCUtoCPCField
        {
            public byte EngineStart { get; set; }
        }

    }

}
