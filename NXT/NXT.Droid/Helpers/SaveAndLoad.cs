using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXT.Services;
using NXT.Droid.Helpers;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.IO;

[assembly: Dependency(typeof(SaveAndLoad))]
namespace NXT.Droid.Helpers
{
    public class SaveAndLoad : ISaveAndLoad
    {
        public async Task<string> ReadAllTextAsync(string filename)
        {
            var file = AppDataFolder(filename);
            if (System.IO.File.Exists(file))
            {
                using (var streamReader = new StreamReader(file))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
            return "";
        }
        public async Task WriteAllTextAsync(string filename, string content)
        {
            //using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileName)))
            try
            {
                var path = AppDataFolder(filename);
                using (var streamWriter = new StreamWriter(path))
                {
                    await streamWriter.WriteAsync(content);
                }
            }
            catch (Exception e)
            {

            }
        }
        
        private static string AppDataFolder(string filename)
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename);
            
            return path;
        }
    }
}