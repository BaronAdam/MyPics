using System.IO;
using System.Threading.Tasks;
using MyPics.Infrastructure.Persistence;

namespace MyPics.Infrastructure.Interfaces
{
    public interface ICloudinaryService
    {
        Task<CustomUploadResult> UploadFile(Stream stream, string filename);
    }
}