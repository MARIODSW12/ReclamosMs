using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace Reclamos.Tests.Reclamos.Infrastructure.Services
{
    public class CloudinaryWrapper : Cloudinary
    {
        public CloudinaryWrapper(Account account) : base(account) { }
        public CloudinaryWrapper(string cloudinaryUrl) : base(cloudinaryUrl) { }

        // Make the method virtual so Moq can override it
        public virtual Task<ImageUploadResult> UploadAsync(ImageUploadParams uploadParams, CancellationToken cancellationToken = default)
        {
            return base.UploadAsync(uploadParams, cancellationToken);
        }
    }
}
