using DevameetCSharp.Dtos;
using DevameetCSharp.Models;
using DevameetCSharp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DevameetCSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeetController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IMeetRepository _meetRepository;
        private readonly IMeetObjectsRepository _meetObjectsRepository;

        public MeetController(ILogger<MeetController> logger, IUserRepository userRepository,
            IMeetRepository meetRepository, IMeetObjectsRepository meetObjectsRepository) : base(userRepository)
        {
            _logger = logger;
            _meetRepository = meetRepository;
            _meetObjectsRepository = meetObjectsRepository;
        }

        [HttpGet]
        public IActionResult GetMeet()
        {
            try
            {
                User user = GetToken();

                List<Meet> meets = _meetRepository.GetMeetsByUser(user.Id);

                return Ok(meets);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro na busca das salas de reunião: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro na busca das salas de reunião: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }

        [HttpGet]
        [Route("objects")]
        public IActionResult GetMeetObjects(int meetId)
        {
            try
            {
                return Ok(_meetObjectsRepository.GetObjectsByMeet(meetId));
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro na busca dos objetos da sala de reunião: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro na busca dos objetos da sala de reunião: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }

        [HttpPost]
        public IActionResult CreateMeet([FromBody] MeetRequestDto meetRequestDto)
        {
            try
            {
                if(string.IsNullOrEmpty(meetRequestDto.Name) || string.IsNullOrWhiteSpace(meetRequestDto.Name))
                {
                    _logger.LogError("O nome da sala precisa ser preenchido");
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto()
                    {
                        Description = "O nome da sala precisa ser preenchido",
                        Status = StatusCodes.Status400BadRequest,
                    });
                }
                else
                {
                    Meet meet = new Meet();
                    meet.Name = meetRequestDto.Name;
                    meet.Color = meetRequestDto.Color;
                    meet.Link = Guid.NewGuid().ToString();
                    meet.UserId = GetToken().Id;

                    _meetRepository.CreateMeet(meet);

                    return Ok("Sala de reunião salva com sucesso");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro ao salvar a sala de reunião: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro ao salvar a sala de reunião: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }

        [HttpPut]
        public IActionResult UpdateMeet([FromBody] MeetUpdateRequestDto meetUpdateRequestDto, int meetId)
        {
            try
            {
                Meet meet = _meetRepository.GetMeetById(meetId);
                meet.Name = meetUpdateRequestDto.Name;
                meet.Color = meetUpdateRequestDto.Color;
                _meetRepository.UpdateMeet(meet);
                List<MeetObjects> meetObjects = new List<MeetObjects>();
                foreach (MeetObjectsDto objectsDto in meetUpdateRequestDto.Objects)
                {
                    MeetObjects meetObj = new MeetObjects();
                    meetObj.Name = objectsDto.Name;
                    meetObj.Orientation = objectsDto.Orientation;
                    meetObj.X = objectsDto.X;
                    meetObj.Y = objectsDto.Y;
                    meetObj.MeetId = meet.Id;
                    meetObj.ZIndex = objectsDto.ZIndex;
                    meetObj.Walkable = objectsDto.Walkable == null ? true : false;
                    meetObjects.Add(meetObj);
                }

                _meetObjectsRepository.CreateObjectsByMeet(meetObjects, meetId);

                return Ok("Sala de reunião salva com sucesso!");

            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro ao atualizar a sala de reunião: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro ao atualizar a sala de reunião: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }

        [HttpDelete]
        public IActionResult DeleteMeet(int meetId)
        {
            try
            {
                _meetRepository.DeleteMeet(meetId);
                return Ok("Sala de reunião excluída com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro ao excluir a sala de reunião: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro ao excluir a sala de reunião: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }
    }
}
