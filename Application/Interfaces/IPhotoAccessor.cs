using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
  public interface IPhotoAccessor
  {
    PhotoUploadResult AddPhoto(IFormFile file);
    string DeletePhoto(string publicId);
  }

  public class PhotoUploadResult
  {
    public string PublicId { get; set; }
    public string Url { get; set; }
  }
}