#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace Microsoft.Maui
{
	/// <inheritdoc/>
	public partial class FontRegistrar : IFontRegistrar
	{
		static string? LoadNativeAppFont(string font, string filename, string? alias)
		{
			if (FileSystemUtils.AppPackageFileExists(filename))
				return $"ms-appx:///{filename}";

			var packagePath = Path.Combine("Assets", filename);
			if (FileSystemUtils.AppPackageFileExists(packagePath))
				return $"ms-appx:///Assets/{filename}";

			packagePath = Path.Combine("Fonts", filename);
			if (FileSystemUtils.AppPackageFileExists(packagePath))
				return $"ms-appx:///Fonts/{filename}";

			packagePath = Path.Combine("Assets", "Fonts", filename);
			if (FileSystemUtils.AppPackageFileExists(packagePath))
				return $"ms-appx:///Assets/Fonts/{filename}";

			// TODO: check other folders as well

			var emb = LoadFont(new EmbeddedFont { FontName = filename, ResourceStream = null });

			if (emb.success)
			{
				 
			}



			throw new FileNotFoundException($"Native font with the name {filename} was not found.");
		}

        static (bool success, string? filePath) LoadFont(EmbeddedFont font)
        {
            if (font.FontName is null || font.ResourceStream is null)
                return (false, null);

            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var assemblyName = entryAssembly?.GetName().Name ?? "App";
            var tmpdir = Path.Combine(Path.GetTempPath(), assemblyName, "Fonts");
            Directory.CreateDirectory(tmpdir);
            var filePath = Path.Combine(tmpdir, font.FontName);
            if (File.Exists(filePath))
                return (true, filePath);
            try {
                using (var fileStream = File.Create(filePath)) {
                    font.ResourceStream.CopyTo(fileStream);
                }
                return (true, filePath);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
                File.Delete(filePath);
            }
            return (false, null);
        }
	}
}