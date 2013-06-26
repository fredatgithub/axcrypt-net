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

using Axantum.AxCrypt.Core;
using Axantum.AxCrypt.Core.IO;
using Axantum.AxCrypt.Core.Runtime;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Axantum.AxCrypt.Mono.Test
{
    [TestFixture]
    public static class TestRuntimeEnvironment
    {
        private static IRuntimeEnvironment _previousEnvironment;

        private static readonly TimeSpan _workFolderStateMinimumIdle = new TimeSpan(0, 0, 0, 0, 100);

        [SetUp]
        public static void Setup()
        {
            _previousEnvironment = OS.Current;
            OS.Current = new RuntimeEnvironment(_workFolderStateMinimumIdle);
        }

        [TearDown]
        public static void Teardown()
        {
            OS.Current = _previousEnvironment;
        }

        [Test]
        public static void TestAxCryptExtension()
        {
            Assert.That(OS.Current.AxCryptExtension, Is.EqualTo(".axx"), "Checking the standard AxCrypt extension.");
        }

        [Test]
        public static void TestIfIsLittleEndian()
        {
            Assert.That(OS.Current.IsLittleEndian, Is.EqualTo(BitConverter.IsLittleEndian), "Checking endianess.");
        }

        [Test]
        public static void TestRandomBytes()
        {
            byte[] randomBytes = OS.Current.GetRandomBytes(100);
            Assert.That(randomBytes.Length, Is.EqualTo(100), "Ensuring we really got the right number of bytes.");
            Assert.That(randomBytes, Is.Not.EquivalentTo(new byte[100]), "It is not in practice possible that all zero bytes are returned by GetRandomBytes().");

            randomBytes = OS.Current.GetRandomBytes(1000);
            double average = randomBytes.Average(b => b);
            Assert.That(average >= 120 && average <= 135, "Unscientific, but the sample sequence should not vary much from a mean of 127.5, but was {0}".InvariantFormat(average));
        }

        [Test]
        public static void TestRuntimeFileInfo()
        {
            IRuntimeFileInfo runtimeFileInfo = OS.Current.FileInfo(Path.Combine(Path.GetTempPath(), "A File.txt"));
            Assert.That(runtimeFileInfo is RuntimeFileInfo, "The instance returned should be of type RuntimeFileInfo");
            Assert.That(runtimeFileInfo.Name, Is.EqualTo("A File.txt"));
            runtimeFileInfo = OS.Current.FileInfo(Path.Combine(Path.GetTempPath(), "A File.txt"));
            Assert.That(runtimeFileInfo.Name, Is.EqualTo("A File.txt"));
        }

        [Test]
        public static void TestTemporaryDirectoryInfo()
        {
            IRuntimeFileInfo tempInfo = OS.Current.WorkFolder;
            Assert.That(tempInfo is RuntimeFileInfo, "The instance returned should be of type RuntimeFileInfo");
            IRuntimeFileInfo tempFileInfo = OS.Current.FileInfo(Path.Combine(tempInfo.FullName, "AxCryptTestTemp.tmp"));
            Assert.DoesNotThrow(() =>
            {
                try
                {
                    using (Stream stream = tempFileInfo.OpenWrite())
                    {
                    }
                }
                finally
                {
                    tempFileInfo.Delete();
                }
            }, "Write permissions should always be present in the temp directory.");
        }

        [Test]
        public static void TestUtcNow()
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime utcNowAgain = OS.Current.UtcNow;
            Assert.That(utcNowAgain - utcNow < new TimeSpan(0, 0, 1), "The difference should not be greater than one second, that's not reasonable.");
        }

        [Test]
        public static void TestFileWatcher()
        {
            bool wasHere = false;
            using (IFileWatcher fileWatcher = OS.Current.CreateFileWatcher(OS.Current.WorkFolder.FullName))
            {
                fileWatcher.FileChanged += (object sender, FileWatcherEventArgs e) =>
                {
                    wasHere = true;
                };
                IRuntimeFileInfo tempFileInfo = OS.Current.FileInfo(Path.Combine(OS.Current.WorkFolder.FullName, "AxCryptTestTemp.tmp"));
                try
                {
                    using (Stream stream = tempFileInfo.OpenWrite())
                    {
                    }
                    for (int i = 0; !wasHere && i < 20; ++i)
                    {
                        Thread.Sleep(100);
                    }
                    Assert.That(wasHere, "The FileWatcher should have noticed the creation of a file.");
                }
                finally
                {
                    wasHere = false;
                    tempFileInfo.Delete();
                }
                for (int i = 0; !wasHere && i < 20; ++i)
                {
                    Thread.Sleep(100);
                }
                Assert.That(wasHere, "The FileWatcher should have noticed the deletion of a file.");
            }
        }

        [Test]
        public static void TestChangedEvent()
        {
            bool wasHere = false;
            OS.Current.WorkFolderStateChanged += (object sender, EventArgs e) => { wasHere = true; };
            OS.Current.NotifyWorkFolderStateChanged();

            Assert.That(wasHere, Is.False, "The RaiseChanged() method should not raise the event immediately.");

            Thread.Sleep(_workFolderStateMinimumIdle.Add(new TimeSpan(0, 0, 0, 0, 100)));

            Assert.That(wasHere, Is.True, "The RaiseChanged() method should raise the event.");
        }
    }
}