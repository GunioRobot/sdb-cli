using System;
using System.Collections.Generic;

namespace Mono.Debugger.Cli
{
    public sealed class CommandArguments
    {
        public CommandArguments(IEnumerable<string> args)
        {
            _enum = args.GetEnumerator();
        }

        private readonly IEnumerator<string> _enum;

        public bool NextBoolean()
        {
            if (!_enum.MoveNext())
                throw MissingArgument();

            bool value;
            if (bool.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public char NextChar(char? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (char)def;

                throw MissingArgument();
            }

            char value;
            if (char.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public byte NextByte(byte? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (byte)def;

                throw MissingArgument();
            }

            byte value;
            if (byte.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public sbyte NextSByte(sbyte? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (sbyte)def;

                throw MissingArgument();
            }

            if (!_enum.MoveNext())
                throw MissingArgument();

            sbyte value;
            if (sbyte.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public ushort NextUInt16(ushort? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (ushort)def;

                throw MissingArgument();
            }

            ushort value;
            if (ushort.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public short NextInt16(short? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (short)def;

                throw MissingArgument();
            }

            if (!_enum.MoveNext())
                throw MissingArgument();

            short value;
            if (short.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public uint NextUInt32(uint? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (uint)def;

                throw MissingArgument();
            }

            uint value;
            if (uint.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public int NextInt32(int? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (int)def;

                throw MissingArgument();
            }

            int value;
            if (int.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public ulong NextUInt64(ulong? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (ulong)def;

                throw MissingArgument();
            }

            ulong value;
            if (ulong.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public long NextInt64(long? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (long)def;

                throw MissingArgument();
            }

            long value;
            if (long.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public float NextSingle(float? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (float)def;

                throw MissingArgument();
            }

            float value;
            if (float.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public double NextDouble(double? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (double)def;

                throw MissingArgument();
            }

            double value;
            if (double.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public decimal NextDecimal(decimal? def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return (decimal)def;

                throw MissingArgument();
            }

            decimal value;
            if (decimal.TryParse(_enum.Current, out value))
                return value;

            throw InvalidFormat();
        }

        public string NextString(string def = null)
        {
            if (!_enum.MoveNext())
            {
                if (def != null)
                    return def;

                throw MissingArgument();
            }

            var current = _enum.Current;

            // Special case for quoted strings...
            if (current.StartsWith("\""))
            {
                while (!current.EndsWith("\""))
                    current += NextString();

                return current.Substring(1, current.Length - 2);
            }

            return current;
        }

        public T NextEnum<T>(T? def = null)
            where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Type T is not an enum.");

            T value;
            if (Enum.TryParse(NextString(def != null ? def.ToString() : null), true, out value))
                return value;

            throw InvalidFormat();
        }

        private static Exception MissingArgument()
        {
            return new Exception("Missing command argument.");
        }

        private static Exception InvalidFormat()
        {
            return new Exception("Incorrectly formatted argument.");
        }
    }
}
