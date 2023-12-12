using DevameetCSharp.Models;

namespace DevameetCSharp.Repository.Impl
{
    public class MeetObjectsRepositoryImpl : IMeetObjectsRepository
    {
        private readonly DevameetContext _context;
        public MeetObjectsRepositoryImpl(DevameetContext context) 
        {
            _context = context;
        }
        public void CreateObjectsByMeet(List<MeetObjects> meetObjectsNew, int meetId)
        {
            List<MeetObjects> meetObjectsExists = _context.MeetObjects.Where(o => o.MeetId == meetId).ToList();

            foreach (MeetObjects meetObj in meetObjectsExists)
            {
                _context.MeetObjects.Remove(meetObj);
                _context.SaveChanges();
            }

            foreach (MeetObjects meetObj in meetObjectsNew)
            {
                _context.MeetObjects.Add(meetObj);
                _context.SaveChanges();
            }
        }

        public List<MeetObjects> GetObjectsByMeet(int meetId)
        {
            return _context.MeetObjects.Where(o => o.MeetId == meetId).ToList();
        }
    }
}
