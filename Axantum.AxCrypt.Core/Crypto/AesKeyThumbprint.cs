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

using Axantum.AxCrypt.Core.Reader;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Axantum.AxCrypt.Core.Crypto
{
    /// <summary>
    /// Represent a salted thumb print for a AES key. Instances of this class are immutable. A thumb print is
    /// typically only valid and comparable on the same computer and log on where it was created.
    /// </summary>
    [DataContract(Namespace = "http://www.axantum.com/Serialization/")]
    public class AesKeyThumbprint : IEquatable<AesKeyThumbprint>
    {
        [DataMember(Name = "Thumbprint")]
        private byte[] _bytes;

        /// <summary>
        /// Instantiate a thumb print
        /// </summary>
        /// <param name="key">The key to thumbprint.</param>
        /// <param name="salt">The salt to use.</param>
        public AesKeyThumbprint(AesKey key, KeyWrapSalt salt, long iterations)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (salt == null)
            {
                throw new ArgumentNullException("salt");
            }

            KeyWrap keyWrap = new KeyWrap(key, salt, iterations, KeyWrapMode.Specification);
            byte[] wrap = keyWrap.Wrap(key);

            Initialize(wrap);
        }

        private void Initialize(byte[] wrap)
        {
            _bytes = ThumbprintFromWrappedKey(wrap);
        }

        private static byte[] ThumbprintFromWrappedKey(byte[] wrap)
        {
            byte[] thumprint = new byte[8];
            for (int i = 0; i < wrap.Length; ++i)
            {
                thumprint[i % thumprint.Length] ^= wrap[i];
            }
            return thumprint;
        }

        #region IEquatable<AesKeyThumbprint> Members

        public bool Equals(AesKeyThumbprint other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return _bytes.IsEquivalentTo(other._bytes);
        }

        #endregion IEquatable<AesKeyThumbprint> Members

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(AesKeyThumbprint) != obj.GetType())
            {
                return false;
            }
            AesKeyThumbprint other = (AesKeyThumbprint)obj;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            int hashcode = 0;
            foreach (byte b in _bytes)
            {
                hashcode += b;
            }
            return hashcode;
        }

        public static bool operator ==(AesKeyThumbprint left, AesKeyThumbprint right)
        {
            if (Object.ReferenceEquals(left, right))
            {
                return true;
            }
            if ((object)left == null)
            {
                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(AesKeyThumbprint left, AesKeyThumbprint right)
        {
            return !(left == right);
        }
    }
}