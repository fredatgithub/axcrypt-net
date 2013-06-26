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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axantum.AxCrypt.Core.Crypto;
using Axantum.AxCrypt.Core.UI;
using NUnit.Framework;

namespace Axantum.AxCrypt.Core.Test
{
    [TestFixture]
    public static class TestKnownKeys
    {
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
        public static void TestAddNewKnownKey()
        {
            KnownKeys knownKeys = new KnownKeys();
            AesKey key = new AesKey();
            knownKeys.Add(key);
            Assert.That(knownKeys.Keys.First(), Is.EqualTo(key), "The first and only key should be the one just added.");
        }

        [Test]
        public static void TestAddTwoNewKnownKeys()
        {
            KnownKeys knownKeys = new KnownKeys();
            AesKey key1 = new AesKey();
            knownKeys.Add(key1);
            AesKey key2 = new AesKey();
            knownKeys.Add(key2);
            Assert.That(knownKeys.Keys.First(), Is.EqualTo(key2), "The first key should be the last one added.");
            Assert.That(knownKeys.Keys.Last(), Is.EqualTo(key1), "The last key should be the first one added.");
        }

        [Test]
        public static void TestAddSameKeyTwice()
        {
            KnownKeys knownKeys = new KnownKeys();
            AesKey key = new AesKey();
            knownKeys.Add(key);
            knownKeys.Add(key);
            Assert.That(knownKeys.Keys.Count(), Is.EqualTo(1), "Only one key should be in the collection even if added twice.");
            Assert.That(knownKeys.Keys.First(), Is.EqualTo(key), "The first and only key should be the one just added.");
        }

        [Test]
        public static void TestDefaultEncryptionKey()
        {
            KnownKeys knownKeys = new KnownKeys();
            AesKey key = new AesKey();
            knownKeys.DefaultEncryptionKey = key;
            Assert.That(knownKeys.DefaultEncryptionKey, Is.EqualTo(key), "The DefaultEncryptionKey should be the one just set as it.");
            Assert.That(knownKeys.Keys.Count(), Is.EqualTo(1), "Only one key should be in the collection.");
            Assert.That(knownKeys.Keys.First(), Is.EqualTo(key), "The first and only key should be the one just set as DefaultEncryptionKey.");
        }

        [Test]
        public static void TestClear()
        {
            KnownKeys knownKeys = new KnownKeys();
            AesKey key1 = new AesKey();
            knownKeys.Add(key1);
            AesKey key2 = new AesKey();
            knownKeys.Add(key2);
            Assert.That(knownKeys.Keys.Count(), Is.EqualTo(2), "There should be two keys in the collection.");

            knownKeys.Clear();
            Assert.That(knownKeys.Keys.Count(), Is.EqualTo(0), "There should be zero keys in the collection after Clear().");
        }

        [Test]
        public static void TestSettingNullDefaultEncryptionKey()
        {
            KnownKeys knownKeys = new KnownKeys();
            AesKey key1 = new AesKey();
            knownKeys.Add(key1);
            AesKey key2 = new AesKey();
            knownKeys.DefaultEncryptionKey = key2;

            Assert.That(knownKeys.Keys.Count(), Is.EqualTo(2), "Setting the DefaultEncryptionKey should also add it as a known key.");

            knownKeys.DefaultEncryptionKey = null;
            Assert.That(knownKeys.Keys.Count(), Is.EqualTo(2), "Setting the DefaultEncryptionKey to null should not affect the known keys.");
        }

        [Test]
        public static void TestChangedEvent()
        {
            bool wasChanged = false;
            KnownKeys knownKeys = new KnownKeys();
            knownKeys.Changed += (object sender, EventArgs e) =>
            {
                wasChanged = true;
            };
            AesKey key1 = new AesKey();
            knownKeys.Add(key1);
            Assert.That(wasChanged, Is.True, "A new key should trigger the Changed event.");
            wasChanged = false;
            knownKeys.Add(key1);
            Assert.That(wasChanged, Is.False, "Re-adding an existing key should not trigger the Changed event.");
        }
    }
}