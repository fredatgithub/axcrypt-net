﻿#region Coypright and License

/*
 * AxCrypt - Copyright 2012, Svante Seleborg, All Rights Reserved
 *
 * This file is part of AxCrypt.
 *
 * AxCrypt is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * AxCrypt is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with AxCrypt.  If not, see <http://www.gnu.org/licenses/>.
 *
 * The source is maintained at http://bitbucket.org/axantum/axcrypt-net please visit for
 * updates, contributions and contact with the author. You may also visit
 * http://www.axantum.com for more information about the author.
*/

#endregion Coypright and License

using System;
using Axantum.AxCrypt.Core.Crypto;
using Axantum.AxCrypt.Core.Reader;
using NUnit.Framework;

namespace Axantum.AxCrypt.Core.Test
{
    [TestFixture]
    public static class TestKeyWrap1HeaderBlock
    {
        private class KeyWrap1HeaderBlockForTest : KeyWrap1HeaderBlock
        {
            public KeyWrap1HeaderBlockForTest(AesKey key)
                : base(key)
            {
            }

            public void SetValuesDirect(byte[] wrapped, KeyWrapSalt salt, long iterations)
            {
                Set(wrapped, salt, iterations);
            }
        }

        [SetUp]
        public static void Setup()
        {
            SetupAssembly.AssemblySetup();
        }

        [TearDown]
        public static void Teardown()
        {
            SetupAssembly.AssemblyTeardown();
        }

        [Test]
        public static void TestSetBadArguments()
        {
            KeyWrap1HeaderBlockForTest keyWrap1HeaderBlock = new KeyWrap1HeaderBlockForTest(new AesKey());

            KeyWrapSalt okSalt = new KeyWrapSalt(16);
            KeyWrapSalt badSalt = new KeyWrapSalt(32);

            Assert.Throws<ArgumentNullException>(() =>
            {
                keyWrap1HeaderBlock.SetValuesDirect(null, okSalt, 100);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                keyWrap1HeaderBlock.SetValuesDirect(new byte[0], okSalt, 100);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                keyWrap1HeaderBlock.SetValuesDirect(new byte[16], okSalt, 100);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                keyWrap1HeaderBlock.SetValuesDirect(new byte[32], okSalt, 100);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                keyWrap1HeaderBlock.SetValuesDirect(new byte[24], null, 100);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                keyWrap1HeaderBlock.SetValuesDirect(new byte[24], badSalt, 100);
            });
        }
    }
}