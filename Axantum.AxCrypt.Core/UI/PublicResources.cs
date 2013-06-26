﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Axantum.AxCrypt.Core.Properties;

namespace Axantum.AxCrypt.Core.UI
{
    public static class PublicResources
    {
#if DEBUG
#pragma warning disable 414
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This is just a dummy here to ensure 100% code coverage in Unit Tests.")]
        private static Axantum.AxCrypt.Core.Properties.Resources _codeCoverageForInternalDesignerGeneratedConstructorDummy = new Axantum.AxCrypt.Core.Properties.Resources();
#pragma warning restore 414
#endif
        public static CultureInfo Culture
        {
            get
            {
                return Resources.Culture;
            }
            set
            {
                Resources.Culture = value;
            }
        }

        public static Stream AxCryptIcon
        {
            get
            {
                return typeof(Resources).Assembly.GetManifestResourceStream("Axantum.AxCrypt.Core.resources.axcrypticon.ico");
            }
        }

        public static string BouncycastleLicense
        {
            get
            {
                return Resources.bouncycastlelicense;
            }
        }

        public static string JsonNetLicense
        {
            get
            {
                return Resources.json_netlicense;
            }
        }
    }
}