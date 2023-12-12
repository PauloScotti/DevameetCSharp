using System.Collections.ObjectModel;
using DevameetCSharp.Dtos;
using DevameetCSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace DevameetCSharp.Repository.Impl
{
    public class RoomRepositoryImpl : IRoomRepository
    {
        private readonly DevameetContext _context;
        public RoomRepositoryImpl(DevameetContext context)
        {
            _context = context;
        }

        public async Task DeleteUserPosition(string clientId)
        {
            var room = await _context.Rooms.Where(r => r.ClientId == clientId).FirstOrDefaultAsync();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

        }

        public Meet GetRoomById(int meetId)
        {
            Meet meet = _context.Meets.Where(m => m.Id == meetId).FirstOrDefault();

            meet.MeetObjects = _context.MeetObjects.Where(o => o.MeetId == meetId).ToList();

            return meet;
        }

        public async Task<ICollection<PositionDto>> ListUsersPosition(string link)
        {
            var meet = await _context.Meets.Where(m => m.Link == link).FirstOrDefaultAsync();
            var rooms = await _context.Rooms.Where(r => r.MeetId == meet.Id).ToListAsync();

            return rooms.Select(r => new PositionDto
            {
                X = r.X,
                Y = r.Y,
                Orientation = r.Orientation,
                Id = r.Id,
                Name = r.UserName,
                Avatar = r.Avatar,
                Muted = r.Muted,
                Meet = r.meet.Link,
                User = r.UserId.ToString(),
                ClientId = r.ClientId,
            }).ToList();

        }

        public async Task UpdateUserMute(MuteDto muteDto)
        {
            var meet = await _context.Meets.Where(m => m.Link == muteDto.Link).FirstOrDefaultAsync();
            var room = await _context.Rooms.Where(r => r.MeetId == meet.Id && r.UserId == Int32.Parse(muteDto.UserId)).FirstOrDefaultAsync();

            room.Muted = muteDto.Muted;

            await _context.SaveChangesAsync();

        }

        public async Task UpdateUserPosition(int userId, string link, string clientId, UpdatePositionDto updatePositionDto)
        {
            var meet = await _context.Meets.Where(m => m.Link == link).FirstOrDefaultAsync();
            var user = await _context.Users.Where(o =>  o.Id == userId).FirstOrDefaultAsync();

            var usersinRoom = await _context.Rooms.Where(r => r.MeetId == meet.Id).ToListAsync();

            if(usersinRoom.Count >= 15)
            {
                throw new Exception("A sala já está cheia");
            }

            if(usersinRoom.Any(r => r.UserId == userId || r.ClientId == clientId))
            {
                var position = await _context.Rooms.Where(r => r.UserId == userId || r.ClientId == clientId).FirstOrDefaultAsync();
                position.Y = updatePositionDto.Y;
                position.X = updatePositionDto.X;
                position.Orientation = updatePositionDto.Orientation;
            }
            else
            {
                var room = new Room();
                room.Id = userId;
                room.ClientId = clientId;
                room.X = updatePositionDto.X;
                room.Y = updatePositionDto.Y;
                room.Orientation = updatePositionDto.Orientation;
                room.MeetId = meet.Id;
                room.UserName = user.Name;
                room.Avatar = user.Avatar;

                await _context.Rooms.AddAsync(room);
            }
            await _context.SaveChangesAsync();
        }
    }
}
