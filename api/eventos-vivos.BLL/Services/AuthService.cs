using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using eventos_vivos.BDO.DTOs.Auth;
using eventos_vivos.BDO.Enums;
using eventos_vivos.BLL.Interfaces;
using eventosvivos.DAL.Entities;
using eventosvivos.DAL.Persistence;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace eventos_vivos.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly EventosVivosDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(EventosVivosDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginGoogleAsync(GoogleLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
                throw new ArgumentException("El token de Google es obligatorio.");

            var googleClientId = _configuration["GoogleAuth:ClientId"];
            if (string.IsNullOrWhiteSpace(googleClientId))
                throw new InvalidOperationException("No está configurado GoogleAuth:ClientId.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId }
                });
            }
            catch
            {
                throw new UnauthorizedAccessException("El token de Google no es válido.");
            }

            var googleId = payload.Subject;
            var email = payload.Email;
            var nombre = payload.Name;
            var fotoUrl = payload.Picture;

            if (string.IsNullOrWhiteSpace(googleId))
                throw new UnauthorizedAccessException("El token de Google no contiene identificador de usuario.");
            if (string.IsNullOrWhiteSpace(email))
                throw new UnauthorizedAccessException("El token de Google no contiene email.");

            var adminEmails = _configuration.GetSection("AdminEmails").Get<string[]>() ?? Array.Empty<string>();
            var esAdmin = adminEmails.Any(x => x.Equals(email, StringComparison.OrdinalIgnoreCase));
            var rolId = esAdmin ? (long)RolEnum.Administrador : (long)RolEnum.Cliente;

            var user = await _context.Usuarios.FirstOrDefaultAsync(x => x.GoogleId == googleId);
            if (user is null)
            {
                user = new Usuario
                {
                    GoogleId = googleId,
                    Email = email,
                    Nombre = string.IsNullOrWhiteSpace(nombre) ? email : nombre,
                    FotoUrl = fotoUrl,
                    RolId = rolId,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimoAcceso = DateTime.UtcNow
                };
                _context.Usuarios.Add(user);
            }
            else
            {
                user.Email = email;
                user.Nombre = string.IsNullOrWhiteSpace(nombre) ? user.Nombre : nombre;
                user.FotoUrl = fotoUrl;
                user.RolId = rolId;
                user.FechaUltimoAcceso = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            var role = await _context.Roles.FindAsync(user.RolId);
            if (role == null)
                throw new InvalidOperationException("No se encontró el rol configurado para el usuario.");

            var nombreRol = role.Nombre.Trim().ToLower();

            // Generar JWT para el usuario del front con autorizaciones
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no está configurado.");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "EventosVivos";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "EventosVivosClients";
            var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var m) ? m : 60;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UsuarioId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Role, nombreRol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                Token = tokenString,
                UsuarioId = user.UsuarioId,
                Nombre = user.Nombre,
                Email = user.Email,
                FotoUrl = user.FotoUrl,
                Rol = nombreRol
            };
        }
    }
}
