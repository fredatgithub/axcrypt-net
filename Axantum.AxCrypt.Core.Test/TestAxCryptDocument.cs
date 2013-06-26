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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Axantum.AxCrypt.Core.Crypto;
using Axantum.AxCrypt.Core.Reader;
using Axantum.AxCrypt.Core.Runtime;
using Axantum.AxCrypt.Core.Test.Properties;
using Axantum.AxCrypt.Core.UI;
using NUnit.Framework;

namespace Axantum.AxCrypt.Core.Test
{
    [TestFixture]
    public static class TestAxCryptDocument
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
        public static void TestAnsiFileNameFromSimpleFile()
        {
            using (Stream testStream = FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt))
            {
                using (AxCryptDocument document = new AxCryptDocument())
                {
                    Passphrase passphrase = new Passphrase("a");
                    bool keyIsOk = document.Load(testStream, passphrase.DerivedPassphrase);
                    Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                    string fileName = document.DocumentHeaders.FileName;
                    Assert.That(fileName, Is.EqualTo("HelloWorld-Key-a.txt"), "Wrong file name");
                }
            }
        }

        [Test]
        public static void TestUnicodeFileNameFromSimpleFile()
        {
            using (Stream testStream = FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt))
            {
                using (AxCryptDocument document = new AxCryptDocument())
                {
                    Passphrase passphrase = new Passphrase("a");
                    bool keyIsOk = document.Load(testStream, passphrase.DerivedPassphrase);
                    Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                    string fileName = document.DocumentHeaders.FileName;
                    Assert.That(fileName, Is.EqualTo("HelloWorld-Key-a.txt"), "Wrong file name");
                }
            }
        }

        [Test]
        public static void TestFileNameFromSimpleFileWithUnicode()
        {
            using (Stream testStream = FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt))
            {
                using (AxCryptDocument document = new AxCryptDocument())
                {
                    Passphrase passphrase = new Passphrase("a");
                    bool keyIsOk = document.Load(testStream, passphrase.DerivedPassphrase);
                    Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                    string fileName = document.DocumentHeaders.FileName;
                    Assert.That(fileName, Is.EqualTo("HelloWorld-Key-a.txt"), "Wrong file name");
                }
            }
        }

        [Test]
        public static void TestHmacFromSimpleFile()
        {
            DataHmac expectedHmac = new DataHmac(new byte[] { 0xF9, 0xAF, 0x2E, 0x67, 0x7D, 0xCF, 0xC9, 0xFE, 0x06, 0x4B, 0x39, 0x08, 0xE7, 0x5A, 0x87, 0x81 });
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                DataHmac hmac = document.DocumentHeaders.Hmac;
                Assert.That(hmac.GetBytes(), Is.EqualTo(expectedHmac.GetBytes()), "Wrong HMAC");
            }
        }

        [Test]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is a test, and they should start with 'Test'.")]
        public static void TestIsCompressedFromSimpleFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                bool isCompressed = document.DocumentHeaders.IsCompressed;
                Assert.That(isCompressed, Is.False, "This file should not be compressed.");
            }
        }

        [Test]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is a test, and they should start with 'Test'.")]
        public static void TestInvalidPassphraseWithSimpleFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("b");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.False, "The passphrase provided is wrong!");
            }
        }

        [Test]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "This is a test, and they should start with 'Test'.")]
        public static void TestIsCompressedFromLargerFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("Å ä Ö");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.david_copperfield_key__aa_ae_oe__ulu_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                bool isCompressed = document.DocumentHeaders.IsCompressed;
                Assert.That(isCompressed, Is.True, "This file should be compressed.");
            }
        }

        [Test]
        public static void TestDecryptUncompressedFromSimpleFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                using (MemoryStream plaintextStream = new MemoryStream())
                {
                    document.DecryptTo(plaintextStream, new ProgressContext());
                    Assert.That(Encoding.ASCII.GetString(plaintextStream.GetBuffer(), 0, (int)plaintextStream.Length), Is.EqualTo("HelloWorld"), "Unexpected result of decryption.");
                    Assert.That(document.DocumentHeaders.PlaintextLength, Is.EqualTo(10), "'HelloWorld' should be 10 bytes uncompressed plaintext.");
                }
            }
        }

        [Test]
        public static void TestDecryptUncompressedWithCancel()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                using (MemoryStream plaintextStream = new MemoryStream())
                {
                    ProgressContext progress = new ProgressContext(TimeSpan.Zero);
                    progress.Progressing += (object sender, ProgressEventArgs e) =>
                    {
                        throw new OperationCanceledException();
                    };

                    Assert.Throws<OperationCanceledException>(() => { document.DecryptTo(plaintextStream, progress); });
                }
            }
        }

        [Test]
        public static void TestDecryptUncompressedWithPaddingError()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                using (MemoryStream encryptedFile = FakeRuntimeFileInfo.ExpandableMemoryStream((byte[])Resources.helloworld_key_a_txt.Clone()))
                {
                    encryptedFile.Seek(-1, SeekOrigin.End);
                    byte lastByte = (byte)encryptedFile.ReadByte();
                    ++lastByte;
                    encryptedFile.Seek(-1, SeekOrigin.End);
                    encryptedFile.WriteByte(lastByte);
                    encryptedFile.Position = 0;
                    bool keyIsOk = document.Load(encryptedFile, passphrase.DerivedPassphrase);
                    Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                    using (MemoryStream plaintextStream = new MemoryStream())
                    {
                        Assert.Throws<CryptographicException>(() => { document.DecryptTo(plaintextStream, new ProgressContext()); });
                    }
                }
            }
        }

        [Test]
        public static void TestDecryptCompressedWithTruncatedFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("Å ä Ö");
                using (MemoryStream encryptedFile = FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.david_copperfield_key__aa_ae_oe__ulu_txt))
                {
                    encryptedFile.SetLength(encryptedFile.Length / 2);
                    encryptedFile.Position = 0;
                    bool keyIsOk = document.Load(encryptedFile, passphrase.DerivedPassphrase);
                    Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                    using (MemoryStream plaintextStream = new MemoryStream())
                    {
                        Assert.Throws<CryptographicException>(() => { document.DecryptTo(plaintextStream, new ProgressContext()); });
                    }
                }
            }
        }

        [Test]
        public static void TestDecryptCompressedWithCancel()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("Å ä Ö");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.david_copperfield_key__aa_ae_oe__ulu_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                using (MemoryStream plaintextStream = new MemoryStream())
                {
                    ProgressContext progress = new ProgressContext(TimeSpan.Zero);
                    progress.Progressing += (object sender, ProgressEventArgs e) =>
                    {
                        throw new OperationCanceledException();
                    };

                    Assert.Throws<OperationCanceledException>(() => { document.DecryptTo(plaintextStream, progress); });
                }
            }
        }

        [Test]
        public static void TestDecryptCompressedFromLegacy0B6()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("åäö");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.tst_0_0b6_key__aaaeoe__medium_html), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "A correct passphrase was provided, but it was not accepted.");
                Assert.That(document.DocumentHeaders.IsCompressed, Is.True, "The file is compressed.");
                Assert.That(document.DocumentHeaders.FileName, Is.EqualTo("readme.html"), "The file name should be 'readme.html'.");
                using (MemoryStream plaintextStream = new MemoryStream())
                {
                    document.DecryptTo(plaintextStream, new ProgressContext());
                    Assert.That(document.DocumentHeaders.PlaintextLength, Is.EqualTo(3736), "The compressed content should be recorded as 3736 bytes in the headers.");
                    Assert.That(plaintextStream.Length, Is.EqualTo(9528), "The file should be 9528 bytes uncompressed plaintext in actual fact.");
                }
            }
        }

        [Test]
        public static void TestDecryptWithoutLoadFirstFromEmptyFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                using (MemoryStream plaintextStream = new MemoryStream())
                {
                    Assert.Throws<InternalErrorException>(() => { document.DecryptTo(plaintextStream, new ProgressContext()); });
                }
            }
        }

        [Test]
        public static void TestDecryptAfterFailedLoad()
        {
            using (Stream testStream = new MemoryStream())
            {
                AxCrypt1Guid.Write(testStream);
                testStream.Position = 0;
                using (AxCryptDocument document = new AxCryptDocument())
                {
                    Passphrase passphrase = new Passphrase("Å ä Ö");
                    Assert.Throws<FileFormatException>(() => { document.Load(testStream, passphrase.DerivedPassphrase); });
                    using (MemoryStream plaintextStream = new MemoryStream())
                    {
                        Assert.Throws<InternalErrorException>(() => { document.DecryptTo(plaintextStream, new ProgressContext()); });
                    }
                }
            }
        }

        [Test]
        public static void TestDecryptCompressedFromLargerFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("Å ä Ö");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.david_copperfield_key__aa_ae_oe__ulu_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                using (MemoryStream plaintextStream = new MemoryStream())
                {
                    document.DecryptTo(plaintextStream, new ProgressContext());
                    string text = Encoding.UTF8.GetString(plaintextStream.GetBuffer(), 0, (int)plaintextStream.Length);
                    Assert.That(text, Is.StringStarting("The Project Gutenberg EBook of David Copperfield, by Charles Dickens"), "Unexpected start of David Copperfield.");
                    Assert.That(text, Is.StringEnding("subscribe to our email newsletter to hear about new eBooks." + (Char)13 + (Char)10), "Unexpected end of David Copperfield.");
                    Assert.That(text.Length, Is.EqualTo(1992490), "Wrong length of full text of David Copperfield.");
                    Assert.That(document.DocumentHeaders.PlaintextLength, Is.EqualTo(795855), "Wrong expected length of compressed text of David Copperfield.");
                }
            }
        }

        [Test]
        public static void TestHmacCalculationFromSimpleFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                document.DecryptTo(Stream.Null, new ProgressContext());
            }
        }

        [Test]
        public static void TestFailedHmacCalculationFromSimpleFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                document.DocumentHeaders.Hmac = new DataHmac(new byte[document.DocumentHeaders.Hmac.Length]);
                Assert.Throws<Axantum.AxCrypt.Core.Runtime.InvalidDataException>(() =>
                {
                    document.DecryptTo(Stream.Null, new ProgressContext());
                });
            }
        }

        [Test]
        public static void TestNoMagicGuidFound()
        {
            byte[] dummy = Encoding.ASCII.GetBytes("This is a string that generates some bytes, none of which will match the magic GUID");
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                Assert.Throws<FileFormatException>(() => { document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(dummy), passphrase.DerivedPassphrase); }, "Calling with dummy data that does not contain a GUID.");
            }
        }

        [Test]
        public static void TestInputStreamTooShort()
        {
            using (MemoryStream testStream = new MemoryStream())
            {
                byte[] guid = AxCrypt1Guid.GetBytes();
                testStream.Write(guid, 0, guid.Length);
                testStream.Position = 0;
                using (AxCryptDocument document = new AxCryptDocument())
                {
                    Assert.Throws<FileFormatException>(() => { document.Load(testStream, new Passphrase(String.Empty).DerivedPassphrase); }, "Calling with too short a stream, only containing a GUID.");
                }
            }
        }

        [Test]
        public static void TestFileTimesFromSimpleFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");

                string creationTime = document.DocumentHeaders.CreationTimeUtc.ToString(CultureInfo.InvariantCulture);
                Assert.That(creationTime, Is.EqualTo("01/13/2012 17:17:18"), "Checking creation time.");
                string lastAccessTime = document.DocumentHeaders.LastAccessTimeUtc.ToString(CultureInfo.InvariantCulture);
                Assert.That(lastAccessTime, Is.EqualTo("01/13/2012 17:17:18"), "Checking last access time.");
                string lastWriteTime = document.DocumentHeaders.LastWriteTimeUtc.ToString(CultureInfo.InvariantCulture);
                Assert.That(lastWriteTime, Is.EqualTo("01/13/2012 17:17:45"), "Checking last modify time.");
            }
        }

        [Test]
        public static void TestChangePassphraseForSimpleFile()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct and should work!");

                Passphrase newPassphrase = new Passphrase("b");
                using (Stream changedStream = new MemoryStream())
                {
                    DocumentHeaders outputDocumentHeaders = new DocumentHeaders(document.DocumentHeaders);
                    outputDocumentHeaders.SetCurrentVersion();
                    outputDocumentHeaders.RewrapMasterKey(newPassphrase.DerivedPassphrase);

                    document.CopyEncryptedTo(outputDocumentHeaders, changedStream, new ProgressContext());
                    changedStream.Position = 0;
                    using (AxCryptDocument changedDocument = new AxCryptDocument())
                    {
                        bool changedKeyIsOk = changedDocument.Load(changedStream, newPassphrase.DerivedPassphrase);
                        Assert.That(changedKeyIsOk, Is.True, "The changed passphrase provided is correct and should work!");

                        using (MemoryStream plaintextStream = new MemoryStream())
                        {
                            changedDocument.DecryptTo(plaintextStream, new ProgressContext());
                            Assert.That(Encoding.ASCII.GetString(plaintextStream.GetBuffer(), 0, (int)plaintextStream.Length), Is.EqualTo("HelloWorld"), "Unexpected result of decryption.");
                            Assert.That(changedDocument.DocumentHeaders.PlaintextLength, Is.EqualTo(10), "'HelloWorld' should be 10 bytes uncompressed plaintext.");
                        }
                    }
                }
            }
        }

        [Test]
        public static void TestSimpleEncryptToWithCompression()
        {
            DateTime creationTimeUtc = new DateTime(2012, 1, 1, 1, 2, 3, DateTimeKind.Utc);
            DateTime lastAccessTimeUtc = creationTimeUtc + new TimeSpan(1, 0, 0);
            DateTime lastWriteTimeUtc = creationTimeUtc + new TimeSpan(2, 0, 0); ;
            using (Stream inputStream = FakeRuntimeFileInfo.ExpandableMemoryStream(Encoding.UTF8.GetBytes("AxCrypt is Great!")))
            {
                using (Stream outputStream = new MemoryStream())
                {
                    using (AxCryptDocument document = new AxCryptDocument())
                    {
                        Passphrase passphrase = new Passphrase("a");
                        DocumentHeaders headers = new DocumentHeaders(passphrase.DerivedPassphrase);
                        headers.FileName = "MyFile.txt";
                        headers.CreationTimeUtc = creationTimeUtc;
                        headers.LastAccessTimeUtc = lastAccessTimeUtc;
                        headers.LastWriteTimeUtc = lastWriteTimeUtc;
                        document.DocumentHeaders = headers;
                        document.EncryptTo(headers, inputStream, outputStream, AxCryptOptions.EncryptWithCompression, new ProgressContext());
                    }
                    outputStream.Position = 0;
                    using (AxCryptDocument document = new AxCryptDocument())
                    {
                        Passphrase passphrase = new Passphrase("a");
                        bool keyIsOk = document.Load(outputStream, passphrase.DerivedPassphrase);
                        Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                        Assert.That(document.DocumentHeaders.FileName, Is.EqualTo("MyFile.txt"));
                        Assert.That(document.DocumentHeaders.CreationTimeUtc, Is.EqualTo(creationTimeUtc));
                        Assert.That(document.DocumentHeaders.LastAccessTimeUtc, Is.EqualTo(lastAccessTimeUtc));
                        Assert.That(document.DocumentHeaders.LastWriteTimeUtc, Is.EqualTo(lastWriteTimeUtc));
                        using (MemoryStream plaintextStream = new MemoryStream())
                        {
                            document.DecryptTo(plaintextStream, new ProgressContext());
                            Assert.That(document.DocumentHeaders.UncompressedLength, Is.EqualTo(17), "'AxCrypt is Great!' should be 17 bytes uncompressed plaintext.");
                            Assert.That(Encoding.ASCII.GetString(plaintextStream.GetBuffer(), 0, (int)plaintextStream.Length), Is.EqualTo("AxCrypt is Great!"), "Unexpected result of decryption.");
                        }
                    }
                }
            }
        }

        [Test]
        public static void TestSimpleEncryptToWithoutCompression()
        {
            DateTime creationTimeUtc = new DateTime(2012, 1, 1, 1, 2, 3, DateTimeKind.Utc);
            DateTime lastAccessTimeUtc = creationTimeUtc + new TimeSpan(1, 0, 0);
            DateTime lastWriteTimeUtc = creationTimeUtc + new TimeSpan(2, 0, 0); ;
            using (Stream inputStream = FakeRuntimeFileInfo.ExpandableMemoryStream(Encoding.UTF8.GetBytes("AxCrypt is Great!")))
            {
                using (Stream outputStream = new MemoryStream())
                {
                    using (AxCryptDocument document = new AxCryptDocument())
                    {
                        Passphrase passphrase = new Passphrase("a");
                        DocumentHeaders headers = new DocumentHeaders(passphrase.DerivedPassphrase);
                        headers.FileName = "MyFile.txt";
                        headers.CreationTimeUtc = creationTimeUtc;
                        headers.LastAccessTimeUtc = lastAccessTimeUtc;
                        headers.LastWriteTimeUtc = lastWriteTimeUtc;
                        document.DocumentHeaders = headers;
                        document.EncryptTo(headers, inputStream, outputStream, AxCryptOptions.EncryptWithoutCompression, new ProgressContext());
                    }
                    outputStream.Position = 0;
                    using (AxCryptDocument document = new AxCryptDocument())
                    {
                        Passphrase passphrase = new Passphrase("a");
                        bool keyIsOk = document.Load(outputStream, passphrase.DerivedPassphrase);
                        Assert.That(keyIsOk, Is.True, "The passphrase provided is correct!");
                        Assert.That(document.DocumentHeaders.FileName, Is.EqualTo("MyFile.txt"));
                        Assert.That(document.DocumentHeaders.CreationTimeUtc, Is.EqualTo(creationTimeUtc));
                        Assert.That(document.DocumentHeaders.LastAccessTimeUtc, Is.EqualTo(lastAccessTimeUtc));
                        Assert.That(document.DocumentHeaders.LastWriteTimeUtc, Is.EqualTo(lastWriteTimeUtc));
                        using (MemoryStream plaintextStream = new MemoryStream())
                        {
                            document.DecryptTo(plaintextStream, new ProgressContext());
                            Assert.That(document.DocumentHeaders.UncompressedLength, Is.EqualTo(-1), "'AxCrypt is Great!' should not return a value at all for uncompressed, since it was not compressed.");
                            Assert.That(document.DocumentHeaders.PlaintextLength, Is.EqualTo(17), "'AxCrypt is Great!' is 17 bytes plaintext length.");
                            Assert.That(Encoding.ASCII.GetString(plaintextStream.GetBuffer(), 0, (int)plaintextStream.Length), Is.EqualTo("AxCrypt is Great!"), "Unexpected result of decryption.");
                        }
                    }
                }
            }
        }

        private class NonSeekableStream : Stream
        {
            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
            }

            public override long Length
            {
                get { return 0; }
            }

            public override long Position
            {
                get
                {
                    return 0;
                }
                set
                {
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
            }
        }

        [Test]
        public static void TestInvalidArguments()
        {
            using (Stream inputStream = FakeRuntimeFileInfo.ExpandableMemoryStream(Encoding.UTF8.GetBytes("AxCrypt is Great!")))
            {
                using (Stream outputStream = new MemoryStream())
                {
                    using (AxCryptDocument document = new AxCryptDocument())
                    {
                        Passphrase passphrase = new Passphrase("a");
                        DocumentHeaders headers = new DocumentHeaders(passphrase.DerivedPassphrase);

                        Assert.Throws<ArgumentNullException>(() => { document.EncryptTo(null, inputStream, outputStream, AxCryptOptions.EncryptWithCompression, new ProgressContext()); });
                        Assert.Throws<ArgumentNullException>(() => { document.EncryptTo(headers, null, outputStream, AxCryptOptions.EncryptWithCompression, new ProgressContext()); });
                        Assert.Throws<ArgumentNullException>(() => { document.EncryptTo(headers, inputStream, null, AxCryptOptions.EncryptWithCompression, new ProgressContext()); });
                        Assert.Throws<ArgumentNullException>(() => { document.EncryptTo(headers, inputStream, outputStream, AxCryptOptions.EncryptWithCompression, null); });
                        Assert.Throws<ArgumentException>(() => { document.EncryptTo(headers, inputStream, new NonSeekableStream(), AxCryptOptions.EncryptWithCompression, new ProgressContext()); });
                        Assert.Throws<ArgumentException>(() => { document.EncryptTo(headers, inputStream, outputStream, AxCryptOptions.EncryptWithCompression | AxCryptOptions.EncryptWithoutCompression, new ProgressContext()); });
                        Assert.Throws<ArgumentException>(() => { document.EncryptTo(headers, inputStream, outputStream, AxCryptOptions.None, new ProgressContext()); });

                        Assert.Throws<ArgumentNullException>(() => { document.CopyEncryptedTo(null, outputStream, new ProgressContext()); });
                        Assert.Throws<ArgumentNullException>(() => { document.CopyEncryptedTo(headers, null, new ProgressContext()); });
                        Assert.Throws<ArgumentException>(() => { document.CopyEncryptedTo(headers, new NonSeekableStream(), new ProgressContext()); });
                        Assert.Throws<InternalErrorException>(() => { document.CopyEncryptedTo(headers, outputStream, new ProgressContext()); });
                    }
                }
            }
        }

        [Test]
        public static void TestDoubleDispose()
        {
            AxCryptDocument document = new AxCryptDocument();
            document.Dispose();
            document.Dispose();
        }

        [Test]
        public static void TestInvalidHmacInCopyEncryptedTo()
        {
            using (AxCryptDocument document = new AxCryptDocument())
            {
                Passphrase passphrase = new Passphrase("a");
                bool keyIsOk = document.Load(FakeRuntimeFileInfo.ExpandableMemoryStream(Resources.helloworld_key_a_txt), passphrase.DerivedPassphrase);
                Assert.That(keyIsOk, Is.True, "The passphrase provided is correct and should work!");

                Passphrase newPassphrase = new Passphrase("b");
                using (Stream changedStream = new MemoryStream())
                {
                    DocumentHeaders outputDocumentHeaders = new DocumentHeaders(document.DocumentHeaders);
                    outputDocumentHeaders.SetCurrentVersion();
                    outputDocumentHeaders.RewrapMasterKey(newPassphrase.DerivedPassphrase);

                    byte[] modifiedHmacBytes = document.DocumentHeaders.Hmac.GetBytes();
                    modifiedHmacBytes[0] += 1;
                    document.DocumentHeaders.Hmac = new DataHmac(modifiedHmacBytes);
                    Assert.Throws<Axantum.AxCrypt.Core.Runtime.InvalidDataException>(() =>
                    {
                        document.CopyEncryptedTo(outputDocumentHeaders, changedStream, new ProgressContext());
                    });
                }
            }
        }
    }
}