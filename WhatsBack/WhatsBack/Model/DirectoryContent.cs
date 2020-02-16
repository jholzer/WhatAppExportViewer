namespace WhatsBack.Model
{
    public abstract class DirectoryContentBase
    {
        protected DirectoryContentBase(string name, string fullPath)
        {
            Name = name;
            FullPath = fullPath;
        }

        public string Name { get; }
        public string FullPath { get; }

        public override string ToString()
        {
            return $"{GetType()}: {FullPath}";
        }
    }

    public class DirectoryContent : DirectoryContentBase
    {
        public DirectoryContent(string name, string fullPath) : base(name, fullPath)
        {
        }
    }

    public class FileContent : DirectoryContentBase
    {
        public string Extension { get; }

        public FileContent(string name, string fullPath, string extension) : base(name, fullPath)
        {
            Extension = extension;
        }
    }
}
