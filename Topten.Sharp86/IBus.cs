﻿// Sharp86 - 8086 Emulator
// Copyright © 2017-2021 Topten Software. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may 
// not use this product except in compliance with the License. You may obtain 
// a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations 
// under the License.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Topten.Sharp86
{
    public interface IMemoryBus
    {
        byte ReadByte(ushort seg, ushort offset);
        void WriteByte(ushort seg, ushort offset, byte value);
        bool IsExecutableSelector(ushort seg);
    }

    public interface IPortBus
    {
        byte ReadPortByte(ushort port);
        void WritePortByte(ushort port, byte value);
    }

    public static class IMemoryBusExtensions
    {
        public static ushort ReadWord(this IMemoryBus This, uint ptr)
        {
            return This.ReadWord((ushort)(ptr >> 16), (ushort)(ptr & 0xFFFF));
        }
        public static ushort ReadWord(this IMemoryBus This, ushort seg, ushort offset)
        {
            return (ushort)(This.ReadByte(seg, offset) | This.ReadByte(seg, (ushort)(offset + 1)) << 8);
        }
        public static void WriteWord(this IMemoryBus This, ushort seg, ushort offset, ushort value)
        {
            This.WriteByte(seg, offset, (byte)(value & 0xFF));
            This.WriteByte(seg, (ushort)(offset + 1), (byte)(value >> 8));
        }
        public static uint ReadDWord(this IMemoryBus This, uint ptr)
        {
            return This.ReadDWord((ushort)(ptr >> 16), (ushort)(ptr & 0xFFFF));
        }

        public static uint ReadDWord(this IMemoryBus This, ushort seg, ushort offset)
        {
            return (uint)(
                    This.ReadByte(seg, offset) |
                    (This.ReadByte(seg, (ushort)(offset + 1)) << 8) |
                    (This.ReadByte(seg, (ushort)(offset + 2)) << 16) |
                    (This.ReadByte(seg, (ushort)(offset + 3)) << 24));

        }
        public static void WriteDWord(this IMemoryBus This, ushort seg, ushort offset, uint value)
        {
            This.WriteByte(seg, offset, (byte)(value & 0xFF));
            This.WriteByte(seg, (ushort)(offset + 1), (byte)((value >> 8) & 0xFF));
            This.WriteByte(seg, (ushort)(offset + 2), (byte)((value >> 16) & 0xFF));
            This.WriteByte(seg, (ushort)(offset + 3), (byte)((value >> 24) & 0xFF));
        }
        public static byte[] ReadBytes(this IMemoryBus This, ushort seg, ushort offset, int count)
        {
            byte[] buf = new byte[count];
            for (int i = 0; i < count; i++)
            {
                buf[i] = This.ReadByte(seg, offset++);
            }
            return buf;
        }

        public static byte[] ReadBytes(this IMemoryBus This, uint ptr, int count)
        {
            return This.ReadBytes((ushort)(ptr >> 16), (ushort)(ptr & 0xFFFF), count);
        }

        public static void WriteBytes(this IMemoryBus This, ushort seg, ushort offset, byte[] bytes, int length = -1)
        {
            if (length < 0)
                length = bytes.Length;
            for (int i = 0; i < length; i++)
            {
                This.WriteByte(seg, offset++, bytes[i]);
            }
        }

        public static void WriteBytes(this IMemoryBus This, uint ptr, byte[] bytes)
        {
            This.WriteBytes((ushort)(ptr >> 16), (ushort)(ptr & 0xFFFF), bytes);
        }

        public static ushort WriteString(this IMemoryBus This, uint ptr, string str, ushort length)
        {
            return This.WriteString((ushort)(ptr >> 16), (ushort)(ptr & 0xFFFF), str, length);
        }

        public static ushort WriteString(this IMemoryBus This, ushort seg, ushort offset, string str, ushort length)
        {
            var bytes = Encoding.GetEncoding(1252).GetBytes(str);
            length = (ushort)Math.Min(bytes.Length, length - 1);

            // Write string
            for (int i = 0; i < bytes.Length; i++)
            {
                This.WriteByte(seg, offset++, bytes[i]);
            }

            // Null terminator
            This.WriteByte(seg, offset, 0);
            return length;
        }

        public static string ReadString(this IMemoryBus This, uint ptr)
        {
            return ReadString(This, (ushort)(ptr >> 16), (ushort)(ptr & 0xFFFF));
        }

        public static string ReadString(this IMemoryBus This, ushort seg, ushort offset, byte terminator = 0)
        {
            if (seg == 0 && offset == 0)
                return null;

            ushort endPos = offset;
            while (This.ReadByte(seg, endPos) != terminator)
            {
                endPos++;
            }

            return Encoding.GetEncoding(1252).GetString(This.ReadBytes(seg, offset, endPos - offset));
        }

        public static string ReadString(this IMemoryBus This, uint ptr, ushort bufSize)
        {
            return ReadString(This, (ushort)(ptr >> 16), (ushort)(ptr & 0xFFFF), bufSize);
        }

        public static string ReadString(this IMemoryBus This, ushort seg, ushort offset, ushort bufSize)
        {
            if (seg == 0 && offset == 0)
                return null;

            ushort endPos = offset;
            while (This.ReadByte(seg, endPos) != 0 && (endPos - offset) < bufSize - 1)
            {
                endPos++;
            }

            return Encoding.GetEncoding(1252).GetString(This.ReadBytes(seg, offset, endPos - offset));
        }


        public static void WriteStruct<T>(this IMemoryBus This, uint ptr, ref T value)
        {
            unsafe
            {
                byte[] temp = new byte[Marshal.SizeOf(typeof(T))];
                fixed (byte* p = temp)
                {
                    Marshal.StructureToPtr(value, (IntPtr)p, false);
                }
                This.WriteBytes(ptr, temp);
            }
        }

        public static void WriteStruct<T>(this IMemoryBus This, ushort seg, ushort offs, ref T value)
        {
            unsafe
            {
                byte[] temp = new byte[Marshal.SizeOf(typeof(T))];
                fixed (byte* p = temp)
                {
                    Marshal.StructureToPtr(value, (IntPtr)p, false);
                }
                This.WriteBytes(seg, offs, temp);
            }
        }

        public static T ReadStruct<T>(this IMemoryBus This, uint ptr)
        {
            unsafe
            {
                var temp = This.ReadBytes(ptr, Marshal.SizeOf(typeof(T)));
                fixed (byte* p = temp)
                {
                    return (T)Marshal.PtrToStructure((IntPtr)p, typeof(T));
                }
            }
        }


    }

    public static class ByteArrayExtensions
    {
        public static void WriteStruct<T>(this byte[] This, uint offset, T value)
        {
            unsafe
            {
                fixed (byte* p = This)
                {
                    Marshal.StructureToPtr(value, (IntPtr)(p + offset), false);
                }
            }
        }

        public static T ReadStruct<T>(this byte[] This, uint ptr)
        {
            unsafe
            {
                fixed (byte* p = This)
                {
                    return (T)Marshal.PtrToStructure((IntPtr)(p + ptr), typeof(T));
                }
            }
        }

        public static ushort ReadWord(this byte[] This, uint ptr)
        {
            return (ushort)(This[ptr] | This[ptr + 1] << 8);
        }

        public static void WriteWord(this byte[] This, uint ptr, ushort value)
        {
            This[ptr] = (byte)(value & 0xFF);
            This[ptr + 1] = (byte)((value >> 8) & 0xFF);
        }

        public static uint ReadDWord(this byte[] This, uint ptr)
        {
            return (uint)(
                    This[ptr] |
                    (This[ptr+1] << 8) |
                    (This[ptr+2] << 16) |
                    (This[ptr+3] << 24));

        }
        public static void WriteDWord(this byte[] This, uint ptr, uint value)
        {
            This[ptr] = (byte)(value & 0xFF);
            This[ptr+1] = (byte)((value >> 8) & 0xFF);
            This[ptr+2] = (byte)((value >> 16) & 0xFF);
            This[ptr+3] = (byte)((value >> 24) & 0xFF);
        }

    }

    public static class IPortBusExtensions
    {

        public static ushort ReadPortWord(this IPortBus This, ushort port)
        {
            return (ushort)(This.ReadPortByte(port) | This.ReadPortByte((ushort)(port + 1)) << 8);
        }

        public static void WritePortWord(this IPortBus This, ushort port, ushort value)
        {
            This.WritePortByte(port, (byte)(value & 0xFF));
            This.WritePortByte((ushort)(port + 1), (byte)((value >> 8) & 0xFF));
        }


    }

    public static class CPUExtensions
    {
        public static void PushWord(this CPU This, ushort value)
        {
            This.sp -= 2;
            This.MemoryBus.WriteWord(This.ss, This.sp, value);
        }

        public static ushort PopWord(this CPU This)
        {
            var val = This.MemoryBus.ReadWord(This.ss, This.sp);
            This.sp += 2;
            return val;
        }

        public static void PushDWord(this CPU This, uint value)
        {
            This.sp -= 4;
            This.MemoryBus.WriteDWord(This.ss, This.sp, value);
        }

        public static uint PopDWord(this CPU This)
        {
            var val = This.MemoryBus.ReadDWord(This.ss, This.sp);
            This.sp += 4;
            return val;
        }

    }
}
