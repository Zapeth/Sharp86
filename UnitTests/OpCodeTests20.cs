﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Topten.Sharp86;

namespace UnitTests
{
    public class OpCodeTests20 : CPUUnitTests
    {
        [Fact]
        public void And_Eb_Gb()
        {
            MMU.WriteByte(0, 100, 0x60);
            al = 0x20;
            emit("and byte [100], al");
            run();
            Assert.Equal(0x20, MMU.ReadByte(0, 100));
        }

        [Fact]
        public void And_Ev_Gv()
        {
            WriteWord(0, 100, 0x6000);
            ax = 0x2000;
            emit("and word [100], ax");
            run();
            Assert.Equal(0x2000, ReadWord(0, 100));
        }

        [Fact]
        public void And_Gb_Eb()
        {
            MMU.WriteByte(0, 100, 0x60);
            al = 0x20;
            emit("and al, byte [100]");
            run();
            Assert.Equal(0x20, al);
        }

        [Fact]
        public void And_Gv_Ev()
        {
            WriteWord(0, 100, 0x6000);
            ax = 0x2000;
            emit("and ax, word [100]");
            run();
            Assert.Equal(0x2000, ax);
        }

        [Fact]
        public void And_AL_Ib()
        {
            al = 0x60;
            emit("and al, 0x20");
            run();
            Assert.Equal(0x20, al);
        }

        [Fact]
        public void And_AX_Iv()
        {
            ax = 0x6000;
            emit("and ax, 0x2000");
            run();
            Assert.Equal(0x2000, ax);
        }

        [Fact]
        public void DAA()
        {
            al = 0xCC;
            emit("daa");
            run();
            Assert.Equal(0x32, al);
        }

    }
}
