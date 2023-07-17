using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WCTTestbed2
{
	public static class StreamHelperEx
	{
		/// <summary>
		/// Return a stream to a specified file from the installation folder.
		/// </summary>
		/// <param name="assemblyType">The owner type for the embedded file</param>
		/// <param name="fileName">Relative name of the file to open. Can contains subfolders.</param>
		/// <returns>File stream</returns>
		public static async Task<Stream> GetEmbeddedFileStreamAsync(Type assemblyType, string fileName)
		{
			await Task.Yield();

			var manifestName = assemblyType.GetTypeInfo().Assembly
				.GetManifestResourceNames()
				.FirstOrDefault(n => n.EndsWith(fileName.Replace(" ", "_"), StringComparison.OrdinalIgnoreCase));

			if (manifestName == null)
			{
				throw new InvalidOperationException($"Failed to find resource [{fileName}]");
			}

			return assemblyType.GetTypeInfo().Assembly.GetManifestResourceStream(manifestName);
		}
	}
}
