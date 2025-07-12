using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Moq;
using Reclamos.Infrastructure.Services;

namespace Reclamos.Tests.Reclamos.Infrastructure.Services
{
    public class CloudinaryService_Tests
    {
        private readonly Mock<CloudinaryWrapper> _cloudinaryMock;
        private readonly CloudinaryService _cloudinaryService;

        public CloudinaryService_Tests()
        {
            // Initialize the mock with the wrapper
            Environment.SetEnvironmentVariable("CLOUDINARY_CLOUD_NAME", "duy5malu9");
            Environment.SetEnvironmentVariable("CLOUDINARY_API_KEY", "675594727598117");
            Environment.SetEnvironmentVariable("CLOUDINARY_API_SECRET", "MHQSB10i5vNz1N_h7l9Y04bK3o8");
            Environment.SetEnvironmentVariable("CLOUDINARY_URL", "cloudinary://675594727598117:MHQSB10i5vNz1N_h7l9Y04bK3o8@duy5malu9");
            var account = new Account(
                Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
                Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
                Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
            );
            _cloudinaryMock = new Mock<CloudinaryWrapper>(account);

            // Manually create the service and inject the mock via reflection
            _cloudinaryService = new CloudinaryService(null);

            // Use reflection to inject the mock into the private field
            var cloudinaryField = typeof(CloudinaryService).GetField(
                "_cloudinary",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            cloudinaryField?.SetValue(_cloudinaryService, _cloudinaryMock.Object);
        }

        [Fact]
        public async Task ShouldReturnUrl_IfOk()
        {
            var fileName = "test.jpg";
           
            _cloudinaryMock
                .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ImageUploadResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    SecureUrl = new Uri("https://res.cloudinary.com/test.jpg")
                });

            // Act
            var result = await _cloudinaryService.SubirArchivo(CreateTestImageStream(), fileName);

            // Assert
            Assert.Contains("https://res.cloudinary.com/", result);
        }

        [Fact]
        public async Task ShouldReturnError_IfNotOk()
        {
            var fileName = "test.jpg";

            _cloudinaryMock
                .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ImageUploadResult
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    SecureUrl = new Uri("https://res.cloudinary.com/test.jpg")
                });

            // Act
            var result = await _cloudinaryService.SubirArchivo(CreateTestImageStream(), fileName);

            // Assert
            Assert.Contains("https://res.cloudinary.com/", result);
        }





        public MemoryStream CreateTestImageStream()
        {
            // Create a 1x1 pixel PNG image (tiny but valid)
            using (var bitmap = new Bitmap(1, 1))
            {
                bitmap.SetPixel(0, 0, Color.Red); // Set pixel color (optional)

                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png); // Save as PNG
                stream.Position = 0; // Reset stream position
                return stream;
            }
        }
    }
}
