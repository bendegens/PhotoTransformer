using PhotoTransformer.Enums;
using System;

namespace PhotoTransformer.Enums
{
    public static class EnumHelper
    {
        internal static FileFormat GetFileFormat(string fileFormat)
        {
            if (Enum.TryParse(fileFormat, true, out FileFormat newFileFormat))
            {
                return newFileFormat;
            }

            throw new Exception($"File format not recognized (allowed values: {Constants.AllowedFileTypes}");
        }

        internal static MirrorType GetMirrorType(string transformationMirrorType)
        {
            if (Enum.TryParse(transformationMirrorType, true, out MirrorType mirrorType))
            {
                return mirrorType;
            }

            return MirrorType.None;
        }
    }
}