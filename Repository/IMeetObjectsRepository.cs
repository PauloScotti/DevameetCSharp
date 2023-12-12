using DevameetCSharp.Models;

namespace DevameetCSharp.Repository
{
    public interface IMeetObjectsRepository
    {
        void CreateObjectsByMeet(List<MeetObjects> meetObjectsNew, int meetId);
        List<MeetObjects> GetObjectsByMeet(int meetId);
    }
}
