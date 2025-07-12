using CloudinaryDotNet.Actions;

namespace Reclamos.Infrastructure.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> SubirArchivo(Stream archivoStream, string nombreArchivo);
    }
}
