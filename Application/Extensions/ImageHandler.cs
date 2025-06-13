using Microsoft.AspNetCore.Http;

namespace Application.Extensions
{
    public static class ImageHandler
    {
        public static async Task<string> ImageConverterAsync(IFormFile image)
        {
            if (image == null)
                return null;

            using var dataStream = new MemoryStream();
            await image.CopyToAsync(dataStream); 

            byte[] imageBytes = dataStream.ToArray();  
            return Convert.ToBase64String(imageBytes); 
        }

    }
}


