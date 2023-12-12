using DevameetCSharp.Dtos;
using DevameetCSharp.Models;

namespace DevameetCSharp.Repository
{
    public interface IRoomRepository
    {
        Task DeleteUserPosition(string clientId);
        Meet GetRoomById(int meetId);
        Task<ICollection<PositionDto>> ListUsersPosition(string link);
        Task UpdateUserMute(MuteDto muteDto);
        Task UpdateUserPosition(int userId, string link, string clientId, UpdatePositionDto updatePositionDto);
    }
}
