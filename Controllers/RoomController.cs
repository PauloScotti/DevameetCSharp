using DevameetCSharp.Dtos;
using DevameetCSharp.Models;
using DevameetCSharp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DevameetCSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IRoomRepository _roomRepository;

        public RoomController(ILogger<RoomController> logger, IRoomRepository roomRepository, IUserRepository userRepository) : base(userRepository)
        {
            _logger = logger;
            _roomRepository = roomRepository;
        }

        [HttpGet]
        public ActionResult GetRoom(int meetId) 
        {
            try
            {
                return Ok(_roomRepository.GetRoomById(meetId));
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro na busca dos dados da sala de reunião: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro na busca dos dados da sala de reunião: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }
    }
}
