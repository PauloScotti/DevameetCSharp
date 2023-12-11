using DevameetCSharp.Dtos;
using DevameetCSharp.Models;
using DevameetCSharp.Repository;
using DevameetCSharp.Service;
using DevameetCSharp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevameetCSharp.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(ILogger<AuthController> logger, IUserRepository userRepository, IConfiguration configuration)
        {
            _logger = logger;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/[controller]/login")]
        public IActionResult ExecuteLogin([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(loginRequestDto.Login) && !string.IsNullOrEmpty(loginRequestDto.Password) 
                    && !string.IsNullOrWhiteSpace(loginRequestDto.Login) && !string.IsNullOrWhiteSpace(loginRequestDto.Password))
                {
                    User user = _userRepository.GetUserByLoginPassword(loginRequestDto.Login.ToLower(), MD5Utils.GenerateMD5(loginRequestDto.Password));

                    if(user != null)
                    {
                        return Ok(new LoginResponseDto()
                        {
                            Email = user.Email,
                            Name = user.Name,
                            Token = TokenService.CreateToken(user, _configuration["JWT:SecretKey"])
                        });
                    }
                    else
                    {
                        return BadRequest(new ErrorResponseDto()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Description = "Usuário e/ou senha são inválidos, por favor tente novamente.",
                        });
                    }
                }
                else
                {
                    return BadRequest(new ErrorResponseDto()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Description = "Campos de login e senha estão preenchidos errados",
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro no login: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro no login: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/[controller]/register")]
        public IActionResult SavedUser([FromBody] UserRegisterDto userDto)
        {
            try
            {
                if(userDto != null)
                {
                    var errors = new List<string>();

                    if(string.IsNullOrEmpty(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Name))
                    {
                        errors.Add("Nome inválido");
                    }
                    if (string.IsNullOrEmpty(userDto.Email) || string.IsNullOrWhiteSpace(userDto.Email) || !userDto.Email.Contains("@"))
                    {
                        errors.Add("E-mail inválido");
                    }
                    if(errors.Count > 0)
                    {

                        foreach (var error in errors)
                        {
                            _logger.LogError(error);
                        }

                        return BadRequest(new ErrorResponseDto()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Description = "Erros encontrados na requisição",
                            Errors = errors
                        });
                    }

                    User user = new User()
                    {
                        Email = userDto.Email.ToLower(),
                        Password = MD5Utils.GenerateMD5(userDto.Password),
                        Name = userDto.Name,
                        Avatar = userDto.Avatar
                    };

                    if (!_userRepository.VerifyEmail(user.Email))
                    {
                        _userRepository.Save(user);
                    }
                    else
                    {
                        _logger.LogError("Usuário já cadastrado");
                        return BadRequest("Usuário já cadastrado");
                    }

                }
                else
                {
                    _logger.LogError("O usuário para ser cadastrado está vazio");
                    return BadRequest("O usuário para ser cadastrado está vazio");
                }
            } catch (Exception ex)
            {
                _logger.LogError("Ocorreu o seguinte erro: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu o seguinte erro: " + ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
            return (Ok("Usuário cadastrado com sucesso!"));
        }
    }
}
