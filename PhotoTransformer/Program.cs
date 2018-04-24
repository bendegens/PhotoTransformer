using PhotoTransformer.Enums;
using PhotoTransformer.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoTransformer
{
    class Program
    {
        static readonly string DirectoryForPhotoTransforming = Settings.Default.DirectoryForPhotoTransforming;
        static readonly int QualityLevel = Settings.Default.QualityLevel;
        static readonly string PreFixPhotoName = Settings.Default.PreFixPhotoName;

        static FileFormat FileFormat;
        static MirrorType MirrorType;

        static string[] PhotoLocations;
        static string DirectoryForTransformedPhotos;

        static void Main(string[] args)
        {
            try
            {
                InitializeSettings();

                WriteStartLines();

                CreateOutPutDirectoryForMirroredPhotos();

                StartMirroringPhotos();

                OpenOutputLocationFolder();

                WriteEndLines();
            }
            catch (Exception ex)
            {
                WriteErrorLines(ex);
            }
        }

        #region Initialize settings and folders

        private static void InitializeSettings()
        {
            Console.Title = "Photo Transformer";

            FileFormat = EnumHelper.GetFileFormat(Settings.Default.FileFormat);
            MirrorType = EnumHelper.GetMirrorType(Settings.Default.Transformation_MirrorType);

            if (Directory.Exists(DirectoryForPhotoTransforming))
            {
                DirectoryForTransformedPhotos = $"{DirectoryForPhotoTransforming}/Transformed-{DateTime.Now.ToString("yyyyMMdd-HHmm")}";

                PhotoLocations = Directory.GetFiles(DirectoryForPhotoTransforming, $"*.{FileFormat}", SearchOption.TopDirectoryOnly);
            }
            else
            {
                throw new Exception("Photo directory does not exist...");
            }
        }

        private static void CreateOutPutDirectoryForMirroredPhotos()
        {
            if (!Directory.Exists(DirectoryForTransformedPhotos))
            {
                Directory.CreateDirectory(DirectoryForTransformedPhotos);
            }
        }

        private static void OpenOutputLocationFolder()
        {
            Process.Start(DirectoryForTransformedPhotos);
        }

        #endregion

        #region Mirroring photos

        static void StartMirroringPhotos()
        {
            for (int i = 0; i < PhotoLocations.Length; i++)
            {
                Console.Write($"\rTransforming photo nr: { i + 1}   ");

                var photo = LoadAndMirrorPhoto(PhotoLocations[i]);
                SaveMirroredPhoto(photo, i);
            }
        }

        static TransformedBitmap LoadAndMirrorPhoto(string photoLocation)
        {
            BitmapImage originalPhoto = GetOriginalPhoto(photoLocation);

            var transformedPhoto = new TransformedBitmap();

            transformedPhoto.BeginInit();

            transformedPhoto.Source = originalPhoto;

            ApplyMirrorTransformation(transformedPhoto);

            transformedPhoto.EndInit();

            return transformedPhoto;
        }

        static void ApplyMirrorTransformation(TransformedBitmap bitmap)
        {
            switch (MirrorType)
            {
                case MirrorType.Horizontal:
                    bitmap.Transform = new ScaleTransform(-1, 1, 0, 0);
                    break;
                case MirrorType.Vertical:
                    bitmap.Transform = new ScaleTransform(1, -1, 0, 0);
                    break;
            }
        }

        static BitmapImage GetOriginalPhoto(string photoLocation)
        {
            var myBitmapImage = new BitmapImage();

            myBitmapImage.BeginInit();

            myBitmapImage.UriSource = new Uri(photoLocation);

            myBitmapImage.EndInit();

            return myBitmapImage;
        }

        static void SaveMirroredPhoto(TransformedBitmap mirroredPhoto, int photoNumber)
        {
            var encoder = GetEncoder();

            encoder.Frames.Add(BitmapFrame.Create(mirroredPhoto));

            using (FileStream file = File.OpenWrite($"{DirectoryForTransformedPhotos}/{PreFixPhotoName}{photoNumber}.jpg"))
            {
                encoder.Save(file);
            }
        }

        static BitmapEncoder GetEncoder()
        {
            switch (FileFormat)
            {
                case FileFormat.PNG:
                    return new PngBitmapEncoder();

                case FileFormat.JPG:
                default:
                    var jpgEncoder = new JpegBitmapEncoder();
                    jpgEncoder.QualityLevel = QualityLevel;

                    return jpgEncoder;
            }
        }

        #endregion

        #region Write lines

        static void WriteStartLines()
        {
            Console.WriteLine(@"   _   _   _   _   _     _   _   _   _   _   _   _   _   _   _   _  
  / \ / \ / \ / \ / \   / \ / \ / \ / \ / \ / \ / \ / \ / \ / \ / \ 
 ( P | h | o | t | o ) ( t | r | a | n | s | f | o | r | m | e | r )
  \_/ \_/ \_/ \_/ \_/   \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ ");

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Ah, you want to transform some photos.");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("*************************************************************************");
            stringBuilder.AppendLine("You are using the following settings, please make sure they're OK.");
            stringBuilder.AppendLine($"   - Location folder : {DirectoryForPhotoTransforming}");
            stringBuilder.AppendLine($"   - Formats to find : {FileFormat} files");
            stringBuilder.AppendLine($"   - Output folder   : {DirectoryForTransformedPhotos}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("*************************************************************************");
            stringBuilder.AppendLine("Transformation settings");
            stringBuilder.AppendLine($"    - Mirroring : {MirrorType}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("*************************************************************************");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"There are {PhotoLocations.Length} photos found to transform. Press any key to fire up this badboy.");

            Console.WriteLine(stringBuilder.ToString());
            Console.ReadLine();
        }

        static void WriteEndLines()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Our work is done here, please press any key to exit.");
            Console.ReadLine();
        }

        private static void WriteErrorLines(Exception ex)
        {
            var errorMessage = "Something went wrong. Press any key to exit.";
            Console.WriteLine(errorMessage);
            Console.WriteLine();

            Console.WriteLine(ex.Message);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(ex.InnerException);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine(errorMessage);
            Console.ReadLine();
        }

        #endregion
    }
}