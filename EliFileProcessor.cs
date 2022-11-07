using System.Threading.Tasks;

namespace PicDivide
{
    /// <summary>
    /// Обработчик Eli файлов
    /// </summary>
    public class EliFileProcessor
    {
        /// <summary>
        /// Поделить первое изображение на второе
        /// </summary>
        /// <param name="firstFile">Первый файл</param>
        /// <param name="secondFile">Второй файл</param>
        /// <returns></returns>
        public EliFile DivideImage(EliFile firstFile, EliFile secondFile)
        {
            var resultArray = new ushort[firstFile.Header.ImageHeight][];

            Parallel.For(0, firstFile.Header.ImageHeight, i =>
            {
                ushort[] dividedLine = DivideLines(firstFile.Image[i], secondFile.Image[i]);
                resultArray[i] = dividedLine;
            });

            var newHeader = new EliFileHeader()
            {
                Signature       = firstFile.Header.Signature,
                HeaderLength    = firstFile.Header.HeaderLength,
                DataOffset      = firstFile.Header.DataOffset,
                ImageWidth      = firstFile.Header.ImageWidth,
                ImageHeight     = firstFile.Header.ImageHeight,
                BitPerPixel     = firstFile.Header.BitPerPixel,
                ImageLineLength = firstFile.Header.ImageLineLength,

            };
            return new EliFile(newHeader, resultArray);
        }

        private static ushort[] DivideLines(ushort[] line1, ushort[] line2)
        {
            var result = new ushort[line1.Length];

            for (int i = 0; i < line1.Length; i++)
            {
                result[i] = (ushort) (line2[i] == 0 ? 0 : (line1[i] / line2[i])); // так как в условиях не оговорено, что делать с 0 во втором изображении, устанавливаю ноль
            }

            return result;
        }
    }
}