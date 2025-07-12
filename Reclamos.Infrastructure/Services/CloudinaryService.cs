using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Reclamos.Infrastructure.Interfaces;

namespace Reclamos.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
            Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
            Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> SubirArchivo(Stream archivoStream, string nombreArchivo)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(nombreArchivo, archivoStream),
                Folder = "evidencia_reclamos"
            };

            var resultado = await _cloudinary.UploadAsync(uploadParams);

            if (resultado.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Error al subir la imagen a Cloudinary." + resultado.Error.Message);

            return resultado.SecureUrl.ToString();
        }
    }
}
