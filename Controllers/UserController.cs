using DevameetCSharp.Dtos;
using DevameetCSharp.Models;
using DevameetCSharp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DevameetCSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IUserRepository userRepository) : base(userRepository)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetUser()
        {
            try
            {
                User user = GetToken();
                return Ok(new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Avatar = user.Avatar,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro na captura dos dados do usuário: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro na captura dos dados do usuário: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }

        [HttpGet]
        [Route("getuserbyid")]
        public IActionResult GettUserById(int idUser)
        {
            try
            {
                User user = _userRepository.GetUserByLogin(idUser);
                return Ok(new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Avatar = user.Avatar,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro na captura dos dados do usuário: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro na captura dos dados do usuário: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UserRequestDto uerDto)
        {
            try
            {
                User user = GetToken();

                if (user != null)
                {
                    if(!string.IsNullOrEmpty(user.Name) && !string.IsNullOrWhiteSpace(user.Name) &&
                       !string.IsNullOrEmpty(user.Avatar) && !string.IsNullOrWhiteSpace(user.Avatar))
                    {
                        user.Avatar = uerDto.Avatar;
                        user.Name = uerDto.Name;

                        _userRepository.UpdateUser(user);

                        return Ok("Usuário salvo com sucesso!");
                    }
                    else
                    {
                        _logger.LogError("Os dados do usuário devem ser preenchidos corretamente");
                        return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto()
                        {
                            Description = "Os dados do usuário devem ser preenchidos corretamente",
                            Status = StatusCodes.Status400BadRequest,
                        });
                    }
                }
                else
                {
                    _logger.LogError("Este usuário não é válido");
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto()
                    {
                        Description = "Este usuário não é válido",
                        Status = StatusCodes.Status400BadRequest,
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro na atualização nos dados do usuário: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro na atualização nos dados do usuário: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }
    }
}
