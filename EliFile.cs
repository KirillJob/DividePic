using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PicDivide
{
    /// <summary>
    /// Файл формата ELI
    /// </summary>
    public class EliFile
    {
        /// <summary>
        /// Заголовок файла
        /// </summary>
        public EliFileHeader Header { get; private set; }

        /// <summary>
        /// Изображение (массив строчек)
        /// </summary>
        public ushort[][] Image { get; private set; }


        /// <summary>
        /// Файл формата ELI
        /// </summary>
        /// <param name="header">Заголовок файла</param>
        /// <param name="image">Изображение</param>
        public EliFile(EliFileHeader header, ushort[][] image)
        {
            Header = header;
            Image = image;
        }

        /// <summary>
        /// Файл формата ELI
        /// </summary>
        /// <param name="path">Путь к ELI файлу</param>
        public EliFile(string path)
        {
            var readTask = ReadDataAsync(path);

            if (readTask.IsFaulted)
            {
                throw readTask.Exception;
            }

            ParseHeader(readTask.Result);
            ParseImage(readTask.Result);
        }

        private async Task<byte[]> ReadDataAsync(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);

                return buffer;
            }
        }

        private void ParseHeader(byte[] rawData)
        {
            int paramLength = 4;

            Header = new EliFileHeader
            {
                Signature       = Encoding.ASCII.GetString(rawData, HeaderAddresses.Signature, paramLength),
                HeaderLength    = BitConverter.ToInt32(rawData, HeaderAddresses.HeaderLength),
                DataOffset      = BitConverter.ToInt32(rawData, HeaderAddresses.DataOffset),
                ImageWidth      = BitConverter.ToInt32(rawData, HeaderAddresses.ImageWidth),
                ImageHeight     = BitConverter.ToInt32(rawData, HeaderAddresses.ImageHeight),
                BitPerPixel     = BitConverter.ToInt32(rawData, HeaderAddresses.BitPerPixel),
                ImageLineLength = BitConverter.ToInt32(rawData, HeaderAddresses.ImageLineLength)
            };
        }

        private void ParseImage(byte[] rawData)
        {
            Image = new ushort[Header.ImageHeight][];

            for (int i = 0; i < Header.ImageHeight; i++)
            {
                var rawLineData = new byte[Header.ImageLineLength];
                int nextIndex = Header.DataOffset + i * Header.ImageLineLength;
                Array.Copy(rawData, nextIndex, rawLineData, 0, Header.ImageLineLength);

                int columnCounter = 0;
                Image[i] = new ushort[Header.ImageWidth];
                for (int j = 0; j < rawLineData.Length; j += 2)
                {
                    Image[i][columnCounter] = BitConverter.ToUInt16(rawLineData, j);
                    columnCounter++;
                }
            }
        }

        public async Task MakeFileAsync(string path)
        {
            var headerBytes = new byte[Header.DataOffset];
            var headerBytesLength = headerBytes.LongLength;
            Encoding.ASCII.GetBytes(Header.Signature).CopyTo(headerBytes, HeaderAddresses.Signature);
            BitConverter.GetBytes(Header.HeaderLength).CopyTo(headerBytes, HeaderAddresses.HeaderLength);
            BitConverter.GetBytes(Header.DataOffset).CopyTo(headerBytes, HeaderAddresses.DataOffset);
            BitConverter.GetBytes(Header.ImageWidth).CopyTo(headerBytes, HeaderAddresses.ImageWidth);
            BitConverter.GetBytes(Header.ImageHeight).CopyTo(headerBytes, HeaderAddresses.ImageHeight);
            BitConverter.GetBytes(Header.BitPerPixel).CopyTo(headerBytes, HeaderAddresses.BitPerPixel);
            BitConverter.GetBytes(Header.ImageLineLength).CopyTo(headerBytes, HeaderAddresses.ImageLineLength);

            var imageBytes = new List<byte>(capacity: Image.Length * sizeof(ushort));
            foreach (var line in Image)
            {
                foreach (var value in line)
                {
                    imageBytes.AddRange(BitConverter.GetBytes(value));
                }
            }
            var imageBytesLength = imageBytes.Count;

            var outputBytes = new byte[headerBytesLength + imageBytesLength];
            Array.Copy(headerBytes, outputBytes, headerBytesLength);
            Array.Copy(imageBytes.ToArray(), 0,outputBytes, headerBytesLength, imageBytesLength);

            using (FileStream stream = File.OpenWrite(path))
            {
                await stream.WriteAsync(outputBytes);
            }
        }
    }
}