﻿using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021;

internal class Day_16 : BaseDay
{
    private readonly BitStream input;

    public Day_16(string inputFile)
    {
        input = new BitStream(File.ReadAllText(inputFile).Trim());
    }

    private class BitStream
    {
        private readonly List<byte> bytes = new();

        public BitStream(string hexBytes)
        {
            bytes = Base.HexToBytes(hexBytes).ToList();
        }

        public IEnumerable<byte> Get(int index, int count)
        {
            var bits = new List<byte>(count);

            for (var i = 0; i < count; i++)
            {
                var bitIndex = index + i;

                var byteIndex = bitIndex / 8;

                var shift = 7 - (bitIndex % 8);

                var bit = (byte)((bytes[byteIndex] >> shift) & 0x01);

                bits.Add(bit);
            }

            return bits;
        }

        public static ulong Value(IEnumerable<byte> bits)
        {
            ulong result = 0;

            foreach (var bit in bits)
            {
                result <<= 1;
                result |= bit;
            }

            return result;
        }
    }

    private abstract class Packet
    {
        public readonly byte Version;
        public readonly byte Type;

        private Packet(byte version, byte type)
        {
            Version = version;
            Type = type;
        }

        private static IEnumerable<byte> GetBits(BitStream stream, ref int index, int count)
        {
            var bits = stream.Get(index, count);
            index += count;

            return bits;
        }

        private static byte GetByte(BitStream stream, ref int index, int count) =>
            (byte)BitStream.Value(GetBits(stream, ref index, count));

        private static ushort GetUShort(BitStream stream, ref int index, int count) =>
            (ushort)BitStream.Value(GetBits(stream, ref index, count));

        private static byte GetHeaderField(BitStream stream, ref int index) => GetByte(stream, ref index, 3);

        public override string ToString() => $"Version: {Version} Type: {Type}";

        public class Literal : Packet
        {
            public readonly ulong Value;

            private Literal(byte version, byte type, ulong value) : base(version, type)
            {
                Value = value;
            }

            internal static Packet Parse(BitStream stream, ref int index, byte version, byte type)
            {
                var valueBytes = new List<byte>();

                while (true)
                {
                    var b = GetByte(stream, ref index, 5);

                    valueBytes.Add(b);

                    if (b >> 4 == 0) break;
                }

                ulong value = 0;
                foreach (var b in valueBytes)
                {
                    value = (value << 4) | (byte)(b & 0xf);
                }

                return new Literal(version, type, value);
            }

            public override string ToString() => $"{base.ToString()} Value: {Value}";
        }

        public class Operator : Packet
        {
            public readonly List<Packet> Packets = new();

            private Operator(byte version, byte type, IEnumerable<Packet> packets) : base(version, type)
            {
                Packets.AddRange(packets);
            }

            internal static Packet Parse(BitStream stream, ref int index, byte version, byte type)
            {
                var lengthType = GetByte(stream, ref index, 1);

                var packets = lengthType == 0 ? ParseToLength(stream, ref index, 15) : ParseToCount(stream, ref index, 11);

                return new Operator(version, type, packets);
            }

            private static IEnumerable<Packet> ParseToLength(BitStream stream, ref int index, int lengthBitsCount)
            {
                var length = GetUShort(stream, ref index, lengthBitsCount);

                var packets = new List<Packet>();

                var indexStart = index;

                while (index - indexStart < length)
                {
                    packets.Add(Parse(stream, ref index));
                }

                return packets;
            }

            private static IEnumerable<Packet> ParseToCount(BitStream stream, ref int index, int lengthBitsCount)
            {
                var count = GetUShort(stream, ref index, lengthBitsCount);

                var packets = new List<Packet>();

                for (var i = 0; i < count; i++)
                {
                    packets.Add(Parse(stream, ref index));
                }

                return packets;
            }

            public override string ToString()
            {
                var result = new StringBuilder();

                result.Append($"{base.ToString()} ");

                result.Append("[ ");

                result.Append(string.Join(", ", Packets.Select(p => $"( {p} )")));

                result.Append(" ]");

                return result.ToString();
            }
        }

        private static Packet Parse(BitStream stream, ref int index)
        {
            var version = GetHeaderField(stream, ref index);

            var type = GetHeaderField(stream, ref index);

            return type switch
            {
                4 => Literal.Parse(stream, ref index, version, type),
                _ => Operator.Parse(stream, ref index, version, type),
            };
        }

        public static Packet Parse(BitStream stream)
        {
            var index = 0;

            return Parse(stream, ref index);
        }
    }

    private static IEnumerable<Packet> Iterate(Packet packet)
    {
        if (packet is Packet.Literal litPacket)
        {
            yield return litPacket;
        }
        else if (packet is Packet.Operator opPacket)
        {
            yield return opPacket;

            foreach(var p in opPacket.Packets)
            {
                foreach(var subP in Iterate(p))
                {
                    yield return subP;
                }
            }
        }
    }

    private static int TotalPacketCount(Packet packet) => Iterate(packet).Count();

    private static int VersionSum(Packet packet) => Iterate(packet).Select(p => (int)p.Version).Sum();

    [Test]
    public static TestResult Test1()
    {
        var input = "D2FE28";

        var stream = new BitStream(input);

        var packet = (Packet.Literal)Packet.Parse(stream);

        //Console.WriteLine(packet.ToString());

        return ExecuteTest(1, () => TotalPacketCount(packet));
    }

    [Test]
    public static TestResult Test2()
    {
        var input = "38006F45291200";

        var stream = new BitStream(input);

        var packet = (Packet.Operator)Packet.Parse(stream);

        //Console.WriteLine(packet.ToString());

        return ExecuteTest(3, () => TotalPacketCount(packet));
    }

    [Test]
    public static TestResult Test3()
    {
        var input = "EE00D40C823060";

        var stream = new BitStream(input);

        var packet = (Packet.Operator)Packet.Parse(stream);

        //Console.WriteLine(packet.ToString());

        return ExecuteTest(4, () => TotalPacketCount(packet));
    }

    [Test]
    public static TestResult Test4()
    {
        var testValues = new List<(string input, (int packetCount, int versionSum) expected)>()
        {
            ("8A004A801A8002F478", (4, 16)),
            ("620080001611562C8802118E34", (7, 12)),
            ("C0015000016115A2E0802F182340", (7, 23)),
            ("A0016C880162017C3686B18A3D4780", (8, 31)),
        };

        return ExecuteTests(testValues, (input) =>
        {
            var stream = new BitStream(input);

            var packet = Packet.Parse(stream);

            return (TotalPacketCount(packet), VersionSum(packet));
        });
    }

    [Part]
    public string Solve1() => $"{VersionSum(Packet.Parse(input))}";
}
