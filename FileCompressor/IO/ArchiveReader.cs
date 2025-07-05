namespace FileCompressorApp.IO
{
    public class ArchiveEntry
    {
        public string FileName;
        public string Algorithm;
        public int OriginalSize;
        public int CompressedSize;
        public long DataPosition;
    }

    public static class ArchiveReader
    {
        public static List<ArchiveEntry> ReadArchiveIndex(string archivePath)
        {
            List<ArchiveEntry> entries = new List<ArchiveEntry>();

            using (var stream = File.OpenRead(archivePath))
            using (var reader = new BinaryReader(stream))
            {
                int fileCount = reader.ReadInt32();
                bool isEncrypted = reader.ReadBoolean();

                for (int i = 0; i < fileCount; i++)
                {
                    int fileNameLength = reader.ReadInt32();
                    string fileName = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength));

                    int algoLength = reader.ReadInt32();
                    string algorithm = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(algoLength));

                    int originalSize = reader.ReadInt32();
                    int symbolCount = reader.ReadInt32();

                    for (int s = 0; s < symbolCount; s++)
                    {
                        //reader.ReadChar(); 
                        reader.ReadByte();

                        int codeLength = reader.ReadInt32();
                        int codeByteLength = reader.ReadInt32();
                        reader.ReadBytes(codeByteLength);
                    }

                    int bitCount = reader.ReadInt32();
                    int byteCount = reader.ReadInt32();

                    long dataPos = stream.Position;
                    reader.BaseStream.Seek(byteCount, SeekOrigin.Current);

                    entries.Add(new ArchiveEntry
                    {
                        FileName = fileName,
                        Algorithm = algorithm,
                        OriginalSize = originalSize,
                        CompressedSize = byteCount,
                        DataPosition = dataPos
                    });
                }
            }

            return entries;
        }


    }
}
