using DevameetCSharp.Dtos;
using DevameetCSharp.Models;
using DevameetCSharp.Repository;
using DevameetCSharp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevameetCSharp.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;

        public AuthController(ILogger<AuthController> logger, IUserRepository userRepository, TokenService tokenService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _tokenService = tokenService;
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
                    // Mesma resposta para e-mail inexistente e senha errada: não revela quais e-mails têm conta.
                    User? user = _userRepository.GetUserByEmail(loginRequestDto.Login.ToLower());
                    var (valid, needsRehash) = user == null
                        ? (false, false)
                        : PasswordService.Verify(loginRequestDto.Password, user.Password);
                    if (!valid)
                    {
                        user = null;
                    }
                    else if (needsRehash)
                    {
                        user!.Password = PasswordService.Hash(loginRequestDto.Password);
                        _userRepository.UpdateUser(user);
                    }

                    if(user != null)
                    {
                        return Ok(new LoginResponseDto()
                        {
                            Email = user.Email,
                            Name = user.Name,
                            Token = _tokenService.CreateToken(user)
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
                _logger.LogError(ex, "Ocorreu o seguinte erro no login: ");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu um erro interno",
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
                    if (string.IsNullOrWhiteSpace(userDto.Password) || userDto.Password.Length < 8)
                    {
                        errors.Add("A senha precisa ter pelo menos 8 caracteres");
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
                        Password = PasswordService.Hash(userDto.Password),
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
                _logger.LogError(ex, "Ocorreu o seguinte erro: ");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu um erro interno",
                    Status = StatusCodes.Status500InternalServerError,
                });
            }
            return (Ok("Usuário cadastrado com sucesso!"));
        }
    }
}
