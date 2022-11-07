namespace PicDivide
{
    /// <summary>
    /// Заголовок файла ELI
    /// </summary>
    public class EliFileHeader
    {
        /// <summary>
        /// Сигнатура, "ELI\0"
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Длина заголовка, байты
        /// </summary>
        public int HeaderLength { get; set; }

        /// <summary>
        /// Смещение до изображения, байты
        /// </summary>
        public int DataOffset { get; set; }

        /// <summary>
        /// Ширина изображения, пиксель
        /// </summary>
        public int ImageWidth { get; set; }

        /// <summary>
        /// Высота изображения, пиксель
        /// </summary>
        public int ImageHeight { get; set; }

        /// <summary>
        /// Количество бит на пиксель
        /// </summary>
        public int BitPerPixel { get; set; }

        /// <summary>
        /// Размер строки изображения в байтах
        /// </summary>
        public int ImageLineLength { get; set; }
    }
}