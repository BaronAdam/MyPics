using System.Collections.Generic;
using System.Threading.Tasks;
using MyPics.Domain.Models;

namespace MyPics.Infrastructure.Interfaces
{
    public interface IPictureRepository
    {
        Task<bool> AddPicturesForPost(List<Picture> pictures);
    }
}