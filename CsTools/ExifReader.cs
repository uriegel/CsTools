using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace CsTools;

public class ExifReader : IDisposable
{
    public enum ExifTag : ushort
    {
        ImageWidth = 0x100,
        ImageLength = 0x101,
        BitsPerSample = 0x102,
        Compression = 0x103,
        PhotometricInterpretation = 0x106,
        ImageDescription = 0x10E,
        Make = 0x10F,
        Model = 0x110,
        StripOffsets = 0x111,
        Orientation = 0x112,
        SamplesPerPixel = 0x115,
        RowsPerStrip = 0x116,
        StripByteCounts = 0x117,
        XResolution = 0x11A,
        YResolution = 0x11B,
        PlanarConfiguration = 0x11C,
        ResolutionUnit = 0x128,
        TransferFunction = 0x12D,
        Software = 0x131,
        DateTime = 0x132,
        Artist = 0x13B,
        WhitePoint = 0x13E,
        PrimaryChromaticities = 0x13F,
        JPEGInterchangeFormat = 0x201,
        JPEGInterchangeFormatLength = 0x202,
        YCbCrCoefficients = 0x211,
        YCbCrSubSampling = 0x212,
        YCbCrPositioning = 0x213,
        ReferenceBlackWhite = 0x214,
        Copyright = 0x8298,

        // SubIFD items
        ExposureTime = 0x829A,
        FNumber = 0x829D,
        ExposureProgram = 0x8822,
        SpectralSensitivity = 0x8824,
        ISOSpeedRatings = 0x8827,
        OECF = 0x8828,
        ExifVersion = 0x9000,
        DateTimeOriginal = 0x9003,
        DateTimeDigitized = 0x9004,
        ComponentsConfiguration = 0x9101,
        CompressedBitsPerPixel = 0x9102,
        ShutterSpeedValue = 0x9201,
        ApertureValue = 0x9202,
        BrightnessValue = 0x9203,
        ExposureBiasValue = 0x9204,
        MaxApertureValue = 0x9205,
        SubjectDistance = 0x9206,
        MeteringMode = 0x9207,
        LightSource = 0x9208,
        Flash = 0x9209,
        FocalLength = 0x920A,
        SubjectArea = 0x9214,
        MakerNote = 0x927C,
        UserComment = 0x9286,
        SubsecTime = 0x9290,
        SubsecTimeOriginal = 0x9291,
        SubsecTimeDigitized = 0x9292,
        FlashpixVersion = 0xA000,
        ColorSpace = 0xA001,
        PixelXDimension = 0xA002,
        PixelYDimension = 0xA003,
        RelatedSoundFile = 0xA004,
        FlashEnergy = 0xA20B,
        SpatialFrequencyResponse = 0xA20C,
        FocalPlaneXResolution = 0xA20E,
        FocalPlaneYResolution = 0xA20F,
        FocalPlaneResolutionUnit = 0xA210,
        SubjectLocation = 0xA214,
        ExposureIndex = 0xA215,
        SensingMethod = 0xA217,
        FileSource = 0xA300,
        SceneType = 0xA301,
        CFAPattern = 0xA302,
        CustomRendered = 0xA401,
        ExposureMode = 0xA402,
        WhiteBalance = 0xA403,
        DigitalZoomRatio = 0xA404,
        FocalLengthIn35mmFilm = 0xA405,
        SceneCaptureType = 0xA406,
        GainControl = 0xA407,
        Contrast = 0xA408,
        Saturation = 0xA409,
        Sharpness = 0xA40A,
        DeviceSettingDescription = 0xA40B,
        SubjectDistanceRange = 0xA40C,
        ImageUniqueID = 0xA420,

        // GPS subifd items
        GPSVersionID = 0x0,
        GPSLatitudeRef = 0x1,
        GPSLatitude = 0x2,
        GPSLongitudeRef = 0x3,
        GPSLongitude = 0x4,
        GPSAltitudeRef = 0x5,
        GPSAltitude = 0x6,
        GPSTimeStamp = 0x7,
        GPSSatellites = 0x8,
        GPSStatus = 0x9,
        GPSMeasureMode = 0xA,
        GPSDOP = 0xB,
        GPSSpeedRef = 0xC,
        GPSSpeed = 0xD,
        GPSTrackRef = 0xE,
        GPSTrack = 0xF,
        GPSImgDirectionRef = 0x10,
        GPSImgDirection = 0x11,
        GPSMapDatum = 0x12,
        GPSDestLatitudeRef = 0x13,
        GPSDestLatitude = 0x14,
        GPSDestLongitudeRef = 0x15,
        GPSDestLongitude = 0x16,
        GPSDestBearingRef = 0x17,
        GPSDestBearing = 0x18,
        GPSDestDistanceRef = 0x19,
        GPSDestDistance = 0x1A,
        GPSProcessingMethod = 0x1B,
        GPSAreaInformation = 0x1C,
        GPSDateStamp = 0x1D,
        GPSDifferential = 0x1E
    }

    public ExifReader(string fileName)
    {
        try
        {
            fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            reader = new (fileStream);

            if (ReadUShort() != 0xFFD8
                ||!ReadToExifStart()
                ||!CreateTagIndex())
                Dispose();
        }
        catch
        {
            Dispose();
        }
    }

    public DateTime? GetDateTime(ExifTag tagId)
        => DateTime.TryParseExact(GetTagValue<string>(tagId)?.TrimEnd('\0') ?? "", "yyyy:MM:dd HH:mm:ss", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
            ? dt
            : null;

    public T? GetTagValue<T>(ExifTag tagId)
        where T: class
    {
        byte[] tagData = GetTagBytes((ushort)tagId, out var tiffDataType, out var numberOfComponents);

        if (tagData == null)
            return null;

        byte fieldLength = GetTIFFFieldLength(tiffDataType);
        switch (tiffDataType)
        {
            case 1:
                return numberOfComponents == 1
                    ? (T)(object)tagData[0]
                    : (T)(object)tagData;
            case 2:
                string str = Encoding.ASCII.GetString(tagData);
                int nullCharIndex = str.IndexOf('\0');
                if (nullCharIndex != -1)
                    str = str.Substring(0, nullCharIndex);
                return (typeof(T) == typeof(DateTime))
                    ? (T)(object)DateTime.ParseExact(str, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture)
                    : (T)(object)str;
            default:
                return null;
        }
    }

    public T? GetTagStructValue<T>(ExifTag tagId)
        where T: struct
    {
        byte[] tagData = GetTagBytes((ushort)tagId, out var tiffDataType, out var numberOfComponents);

        if (tagData.Length == 0)
            return null;

        byte fieldLength = GetTIFFFieldLength(tiffDataType);
        switch (tiffDataType)
        {
            case 3:
                return numberOfComponents == 1
                    ? (T)(object)ToUShort(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToUShort);
            case 4:
                return numberOfComponents == 1
                    ? (T)(object)ToUint(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToUint);
            case 5:
                return numberOfComponents == 1
                    ? (T)(object)ToURational(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToURational);
            case 6:
                return numberOfComponents == 1
                    ? (T)(object)ToSByte(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToSByte);
            case 7:
                return numberOfComponents == 1
                    ? (T)(object)ToUint(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToUint);
            case 8:
                return numberOfComponents == 1
                    ? (T)(object)ToShort(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToShort);
            case 9:
                return numberOfComponents == 1
                    ? (T)(object)ToInt(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToInt);
            case 10:
                return numberOfComponents == 1
                    ? (T)(object)ToRational(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToRational);
            case 11:
                return numberOfComponents == 1
                    ? (T)(object)ToSingle(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToSingle);
            case 12:
                return numberOfComponents == 1
                    ? (T)(object)ToDouble(tagData)
                    : (T)(object)GetArray(tagData, fieldLength, ToDouble);
            default:
                return null;
        }
    }

    byte[] GetTagBytes(ushort tagID, out ushort tiffDataType, out uint numberOfComponents)
    {
        if (reader == null || !catalogue.ContainsKey(tagID))
        {
            tiffDataType = 0;
            numberOfComponents = 0;
            return Array.Empty<byte>();
        }

        long tagOffset = catalogue[tagID];
        if (fileStream?.Position != null)
            fileStream.Position = tagOffset;
        ushort currentTagID = ReadUShort();
        if (currentTagID != tagID)
            throw new Exception("Tag number not at expected offset");
        tiffDataType = ReadUShort();
        numberOfComponents = ReadUint();
        byte[] tagData = ReadBytes(4);
        var dataSize = (int)(numberOfComponents * GetTIFFFieldLength(tiffDataType));
        if (dataSize > 4)
        {
            ushort offsetAddress = ToUShort(tagData);
            return ReadBytes(offsetAddress, dataSize);
        }
        Array.Resize(ref tagData, dataSize);
        return tagData;
    }

    void CatalogueIFD()
    {
        ushort entryCount = ReadUShort();
        for (ushort currentEntry = 0; currentEntry < entryCount; currentEntry++)
        {
            ushort currentTagNumber = ReadUShort();
            catalogue = catalogue.SetItem(currentTagNumber, fileStream!.Position - 2);
            reader?.BaseStream.Seek(10, SeekOrigin.Current);
        }
    }

    byte GetTIFFFieldLength(ushort tiffDataType)
        => tiffDataType switch
        {
            1 => 1,
            2 => 1,
            6 => 1,
            3 => 2,
            8 => 2,
            4 => 4,
            7 => 4,
            9 => 4,
            11 => 4,
            5 => 8,
            10 => 8,
            12 => 8,
            _ => throw new Exception("Unknown TIFF datatype: {tiffDataType}")
        };

    ushort ReadUShort() => ToUShort(ReadBytes(2));
    uint ReadUint() => ToUint(ReadBytes(4));
    string ReadString(int chars) =>Encoding.ASCII.GetString(ReadBytes(chars));
    byte[] ReadBytes(int byteCount) => reader!.ReadBytes(byteCount);
    byte[] ReadBytes(ushort tiffOffset, int byteCount)
    {
        long originalOffset = fileStream!.Position;
        fileStream.Seek(tiffOffset + tiffHeaderStart, SeekOrigin.Begin);
        byte[] data = reader!.ReadBytes(byteCount);
        fileStream.Position = originalOffset;
        return data;
    }

    ushort ToUShort(byte[] data)
    {
        if (isLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(data);

        return data.Count() != 0
            ? BitConverter.ToUInt16(data, 0)
            : (ushort)0;
    }

    double ToURational(byte[] data)
    {
        var numeratorData = new byte[4];
        var denominatorData = new byte[4];
        Array.Copy(data, numeratorData, 4);
        Array.Copy(data, 4, denominatorData, 0, 4);
        uint numerator = ToUint(numeratorData);
        uint denominator = ToUint(denominatorData);
        return numerator / (double)denominator;
    }

    double ToRational(byte[] data)
    {
        var numeratorData = new byte[4];
        var denominatorData = new byte[4];
        Array.Copy(data, numeratorData, 4);
        Array.Copy(data, 4, denominatorData, 0, 4);
        int numerator = ToInt(numeratorData);
        int denominator = ToInt(denominatorData);
        return numerator / (double)denominator;
    }

    uint ToUint(byte[] data)
    {
        if (isLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(data);
        return BitConverter.ToUInt32(data, 0);
    }

    int ToInt(byte[] data)
    {
        if (isLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(data);
        return BitConverter.ToInt32(data, 0);
    }

    double ToDouble(byte[] data)
    {
        if (isLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(data);
        return BitConverter.ToDouble(data, 0);
    }

    float ToSingle(byte[] data)
    {
        if (isLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(data);
        return BitConverter.ToSingle(data, 0);
    }

    short ToShort(byte[] data)
    {
        if (isLittleEndian != BitConverter.IsLittleEndian)
            Array.Reverse(data);
        return BitConverter.ToInt16(data, 0);
    }

    sbyte ToSByte(byte[] data)
        => (sbyte)(data[0] - byte.MaxValue);

    Array GetArray<T>(byte[] data, int elementLengthBytes, ConverterMethod<T> converter)
    {
        Array convertedData = Array.CreateInstance(typeof(T), data.Length / elementLengthBytes);
        var buffer = new byte[elementLengthBytes];
        for (int elementCount = 0; elementCount < data.Length / elementLengthBytes; elementCount++)
        {
            Array.Copy(data, elementCount * elementLengthBytes, buffer, 0, elementLengthBytes);
            convertedData.SetValue(converter(buffer), elementCount);
        }
        return convertedData;
    }

    delegate T ConverterMethod<out T>(byte[] data);

    bool ReadToExifStart()
    {
        byte markerStart;
        byte markerNumber = 0;
        while (((markerStart = reader!.ReadByte()) == 0xFF) && (markerNumber = reader.ReadByte()) != 0xE1)
        {
            ushort dataLength = ReadUShort();
            reader.BaseStream.Seek(dataLength - 2, SeekOrigin.Current);
        }
        return markerStart == 0xFF && markerNumber == 0xE1;
    }

    bool CreateTagIndex()
    {
        ReadUShort();
        if (ReadString(4) != "Exif")
            return false;
        if (ReadUShort() != 0)
            return false;
        tiffHeaderStart = reader!.BaseStream.Position;
        isLittleEndian = ReadString(2) == "II";
        if (ReadUShort() != 0x002A)
            return false;
        uint ifdOffset = ReadUint();
        fileStream!.Position = ifdOffset + tiffHeaderStart;
        CatalogueIFD();
        var offset = GetTagStructValue<uint>((ExifTag)0x8769);
        if (!offset.HasValue)
            return false;
        fileStream.Position = offset.Value + tiffHeaderStart;
        CatalogueIFD();
        offset = GetTagStructValue<uint>((ExifTag)0x8825);
        if (offset.HasValue)
        {
            fileStream.Position = offset.Value + tiffHeaderStart;
            CatalogueIFD();
        }
        return true;
    }

    readonly BinaryReader? reader;
    readonly FileStream? fileStream;

    /// <summary>
    /// The catalogue of tag ids and their absolute offsets within the
    /// file
    /// </summary>
    ImmutableDictionary<ushort, long> catalogue = ImmutableDictionary<ushort, long>.Empty;

    /// <summary>
    /// Indicates whether to read data using big or little endian byte aligns
    /// </summary>
    bool isLittleEndian;

    /// <summary>
    /// The position in the filestream at which the TIFF header starts
    /// </summary>
    long tiffHeaderStart;

    #region IDisposable
    
    bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Verwalteten Zustand (verwaltete Objekte) bereinigen
                reader?.Close();
                fileStream?.Close();
            }

            // Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            // Große Felder auf NULL setzen
            disposedValue = true;
        }
    }

    // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
    // ~ExifReader()
    // {
    //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
