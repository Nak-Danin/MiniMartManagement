namespace MiniMartManagement.Utilities
{
    /// <summary>
    /// Product photos are copied next to the executable (under ProductImages/) rather
    /// than stored in the database as binary data - keeps the DB schema simple and lets
    /// Explorer/backup tools treat them like ordinary files. Product.ImagePath only ever
    /// stores the relative path this class hands back.
    /// </summary>
    public static class ProductImageStore
    {
        private const string FolderName = "ProductImages";

        /// <summary>Copies the given file into the app's ProductImages folder under a new unique name and returns the relative path to store.</summary>
        public static string SaveCopy(string sourceFilePath)
        {
            string folder = Path.Combine(AppContext.BaseDirectory, FolderName);
            Directory.CreateDirectory(folder);

            string extension = Path.GetExtension(sourceFilePath);
            string fileName = $"{Guid.NewGuid():N}{extension}";
            string destinationPath = Path.Combine(folder, fileName);

            File.Copy(sourceFilePath, destinationPath, overwrite: true);

            return Path.Combine(FolderName, fileName).Replace('\\', '/');
        }
    }
}
