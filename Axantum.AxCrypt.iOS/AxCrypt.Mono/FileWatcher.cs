using System;
using Axantum.AxCrypt.Core.IO;

namespace Axantum.AxCrypt.Mono
{
	internal class FileWatcher : IFileWatcher, IDisposable
	{
		public FileWatcher (string path)
		{
		}

		public event EventHandler<FileWatcherEventArgs> FileChanged {
			add {

			}
			remove {

			}
		}

		public void Dispose() {}
	}
}

