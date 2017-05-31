using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NXT.Helpers
{
    public static class FileStorage
    {
        private static readonly Dictionary<string, SemaphoreSlim> m_Semaphores = new Dictionary<string, SemaphoreSlim>();

        private static SemaphoreSlim GetSemaphore(string filename)
        {
            if (m_Semaphores.ContainsKey(filename))
            {
                return m_Semaphores[filename];
            }

            var semaphore = new SemaphoreSlim(1);
            m_Semaphores[filename] = semaphore;
            return semaphore;
        }

        async public static Task<string> LoadDataAsync(string filename)
        {
            var semaphore = GetSemaphore(filename);
            Task t1 = Task.Factory.StartNew(() =>
            {
                semaphore.Wait();
            });
            await t1;

            try
            {
                return await ReadAllTextAsync(filename);
            }
            finally
            {
                semaphore.Release();
            }
        }

        async public static Task SaveDataAsync(string filename, string data)
        {
            var semaphore = GetSemaphore(filename);

            Task t1 = Task.Factory.StartNew(() =>
            {
                semaphore.Wait();
            });
            await t1;

            try
            {
                await WriteAllTextAsync(filename, data);
            }
            finally
            {
                semaphore.Release();
            }
        }


        public static async Task WriteAllTextAsync(string fileName, string content)
        {

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileName)))
            using (var streamWriter = new StreamWriter(stream))
            {
                await streamWriter.WriteAsync(content);
            }
        }

        //public static async Task WriteAllBytesAsync(string fileName, byte[] content)
        //{
        //    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileName)))
        //    using (var streamWriter = File.Open(fileName, FileMode.Create))
        //    {
        //        await streamWriter.WriteAsync(content, 0, content.Length);
        //    }
        //}


        public static async Task<string> ReadAllTextAsync(string fileName)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileName)))
            using (var streamReader = new StreamReader(stream))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        //public static async Task<byte[]> ReadAllBytesAsync(string fileName)
        //{
        //    using (var fileStream = File.OpenRead(fileName))
        //    {
        //        byte[] data = new byte[fileStream.Length];

        //        await fileStream.ReadAsync(data, 0, data.Length);

        //        return data;
        //    }
        //}

    }
}
