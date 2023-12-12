using DevameetCSharp.Models;

namespace DevameetCSharp.Repository
{
    public interface IMeetRepository
    {
        void CreateMeet(Meet meet);
        void DeleteMeet(int meetId);
        Meet GetMeetById(int meetId);
        List<Meet> GetMeetsByUser(int idUser);
        void UpdateMeet(Meet meet);
    }
}
