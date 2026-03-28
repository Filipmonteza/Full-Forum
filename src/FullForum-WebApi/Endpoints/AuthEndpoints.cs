using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FullForum_Infrastructure.Identity;
using FullForum_WebApi.Contracts.Auth;
using FullForum_WebApi.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FullForum_WebApi.Endpoints;

/// Provides authentication endpoints for user registration, login and retrieving the current user.
public static class AuthEndpoints
{
    /// <summary>
    /// Registers authentication endpoints under /api/auth.
    /// </summary>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        // Register User
        group.MapPost("/register", async (
            RegisterRequest registerRequest,
            UserManager<ApplicationIdentityUser> userManager) =>
        {
            // Validate required input
            if (string.IsNullOrWhiteSpace(registerRequest.Email) ||
                string.IsNullOrWhiteSpace(registerRequest.Password) ||
                string.IsNullOrWhiteSpace(registerRequest.Username))
                return Results.BadRequest("Email, password and username are required.");

            // Normalize username
            var displayName = registerRequest.Username.Trim();

            // Validate username length
            if (displayName.Length < 3 || displayName.Length > 50)
                return Results.BadRequest("Username must be between 3 and 50 characters.");

            // Ensure email is unique
            if (await userManager.FindByEmailAsync(registerRequest.Email) != null)
                return Results.BadRequest(new { errors = new[] { "Email is already taken." } });

            // Ensure display name is unique
            var displayTaken = await userManager.Users
                .AnyAsync(u => u.DisplayName == displayName);

            if (displayTaken)
                return Results.BadRequest(new { errors = new[] { "Username is already taken." } });

            // Create new user
            var user = new ApplicationIdentityUser
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                DisplayName = displayName,
                EmailConfirmed = true
            };

            // Save user with hashed password
            var result = await userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
                return Results.BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            // Assign default role
            var roleResult = await userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
                return Results.BadRequest(new { errors = roleResult.Errors.Select(e => e.Description) });

            // Return success response
            return Results.Ok(new RegisterResponse(
                "User registered successfully",
                user.Id
            ));
        }).AllowAnonymous();


        // Login User
        group.MapPost("/login", async (
            LoginRequest loginRequest,
            UserManager<ApplicationIdentityUser> userManager,
            IOptions<JwtSettings> jwtOptions) =>
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(loginRequest.Email) ||
                string.IsNullOrWhiteSpace(loginRequest.Password))
                return Results.BadRequest("Email and password are required.");

            // Find user by email
            var user = await userManager.FindByEmailAsync(loginRequest.Email);
            if (user is null)
                return Results.Unauthorized();

            // Verify password
            var valid = await userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!valid)
                return Results.Unauthorized();

            var jwt = jwtOptions.Value;

            // Create signing key and credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Get user roles
            var roles = await userManager.GetRolesAsync(user);

            // Build claims for JWT
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("displayName", user.DisplayName, string.Empty),
                new Claim(ClaimTypes.Name, user.DisplayName, string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add role claims (used for authorization)
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // Create token
            var token = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwt.ExpiresInMinutes),
                signingCredentials: creds);

            // Serialize token
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Return token and user info
            return Results.Ok(new AuthResponse(
                tokenString,
                user.Id,
                user.Email,
                user.DisplayName,
                roles
            ));
        }).AllowAnonymous();


        // Get Current User
        group.MapGet("/me", [Authorize] async (
            ClaimsPrincipal user,
            UserManager<ApplicationIdentityUser> userManager) =>
        {
            // Extract user id from token
            var userIdStr =
                user.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                user.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate id format
            if (!Guid.TryParse(userIdStr, out var userId))
                return Results.Unauthorized();

            // Load user from database
            var appUser = await userManager.FindByIdAsync(userId.ToString());
            if (appUser is null)
                return Results.NotFound();

            // Get roles
            var roles = await userManager.GetRolesAsync(appUser);

            // Return current user info
            return Results.Ok(new CurrentUserResponse(
                appUser.Id,
                appUser.Email,
                appUser.DisplayName,
                roles
            ));
        });

        return app;
    }
}