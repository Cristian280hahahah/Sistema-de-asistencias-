using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AuthorizePage("/Index", "AdminAccess");
    options.Conventions.AuthorizeFolder("/Admin", "AdminAccess");
    options.Conventions.AuthorizeFolder("/Estudiantes", "AdminAccess");
    options.Conventions.AuthorizeFolder("/Profesores", "AdminAccess");
    options.Conventions.AuthorizeFolder("/Materias", "AdminAccess");
    options.Conventions.AuthorizeFolder("/Secciones", "AdminAccess");
    options.Conventions.AuthorizeFolder("/Aulas", "AdminAccess");
    options.Conventions.AuthorizeFolder("/Asistencias", "TeacherAccess");
    options.Conventions.AuthorizeFolder("/Justificaciones", "TeacherAccess");
    options.Conventions.AuthorizeFolder("/PortalProfesor", "TeacherAccess");
    options.Conventions.AuthorizeFolder("/PortalEstudiante", "StudentAccess");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
    options.Conventions.AllowAnonymousToPage("/Error");
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminAccess", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TeacherAccess", policy => policy.RequireAssertion(context =>
        context.User.IsInRole("Profesor") && !context.User.IsInRole("Admin")));
    options.AddPolicy("StudentAccess", policy => policy.RequireRole("Estudiante"));
});

builder.Services.AddDbContext<AsistenciaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AsistenciaContext>()
.AddDefaultTokenProviders();
builder.Services.AddScoped<IdentityErrorDescriber, SpanishIdentityErrorDescriber>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "SistemaAsistencia.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

await ProvisionInitialAdminAsync(app);

app.Run();

static async Task ProvisionInitialAdminAsync(WebApplication app)
{
    var email = app.Configuration["InitialAdmin:Email"];
    var password = app.Configuration["InitialAdmin:Password"];
    var name = app.Configuration["InitialAdmin:Name"];

    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var roleName in new[] { "Admin", "Profesor", "Estudiante" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleCreation = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!roleCreation.Succeeded)
            {
                var errors = string.Join("; ", roleCreation.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not create the {roleName} role: {errors}");
            }
        }
    }

    if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password))
    {
        if (!app.Environment.IsDevelopment())
        {
            throw new InvalidOperationException("Configure InitialAdmin__Email and InitialAdmin__Password using a secret store before starting in production.");
        }

        app.Logger.LogWarning("Initial administrator is not provisioned. Set InitialAdmin__Email and InitialAdmin__Password to enable sign-in.");
        return;
    }

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        throw new InvalidOperationException("Both InitialAdmin__Email and InitialAdmin__Password must be configured.");
    }

    email = email.Trim().ToLowerInvariant();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
    {
        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            NombreCompleto = string.IsNullOrWhiteSpace(name) ? "Administrador" : name
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Could not create initial administrator: {errors}");
        }

        app.Logger.LogInformation("Initial administrator provisioned for {Email}.", email);
    }
    else if (string.IsNullOrWhiteSpace(user.NombreCompleto) && !string.IsNullOrWhiteSpace(name))
    {
        user.NombreCompleto = name;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw new InvalidOperationException("Could not update the configured administrator profile.");
        }
    }

    if (!await userManager.IsInRoleAsync(user, "Admin"))
    {
        var roleResult = await userManager.AddToRoleAsync(user, "Admin");
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException("Could not assign the administrator role to the configured account.");
        }
    }
}
