# Autenticación del portal

El portal utiliza ASP.NET Core Identity con almacenamiento SQL Server. Identity guarda hashes de contraseña (no contraseñas legibles), aplica una política mínima de 8 caracteres y bloqueo temporal tras cinco intentos fallidos. El inicio de sesión, acceso denegado y la página de error son públicos; los módulos del personal requieren el rol `Admin` u `Operador`. No existe registro público de usuarios. El administrador inicial recibe el rol `Admin` y puede gestionar cuentas desde **Administración**. Las cuentas ordinarias creadas por el administrador reciben el rol `Operador`.

## Primera configuración local

1. Asegúrate de que SQL Server/LocalDB esté disponible y aplica la migración antes de iniciar la aplicación:

   ```powershell
   dotnet ef database update
   ```

2. Inicializa User Secrets y configura el correo/nombre del administrador:

  
   ```

   Sustituye la contraseña de ejemplo por una contraseña única generada por un gestor de contraseñas. User Secrets es solo para desarrollo y no se debe incluir en el repositorio.

3. Inicia el proyecto con `dotnet run` e inicia sesión con el correo y contraseña configurados. Cambia la contraseña desde el menú del usuario después de entrar.

Al iniciar, el sistema asegura los roles `Admin`, `Operador` y `Estudiante`, y crea/provisiona la cuenta administradora usando `UserManager` y la política de Identity. Las credenciales no se escriben en `appsettings.json`. Una vez creada la cuenta, cambiar el secreto de aprovisionamiento no restablece su contraseña: usa la opción **Cambiar contraseña**.

## Crear cuentas de estudiantes

1. Registra primero al estudiante desde **Comunidad → Estudiantes** con su correo.
2. Entra en **Administración → Cuentas de estudiantes** y pulsa **Crear acceso** en su fila.
3. Define una contraseña temporal fuerte y entrégasela al estudiante por un canal seguro. La cuenta usará el correo del expediente y quedará vinculada a ese estudiante.

El estudiante inicia sesión con ese correo y contraseña y llega a **Mi portal**, donde solo ve sus secciones y su historial. No tiene acceso a las pantallas de administración ni a los datos de otros estudiantes. Puede cambiar su contraseña desde el menú de su cuenta.

## Despliegue

Guarda `InitialAdmin__Email`, `InitialAdmin__Name` y `InitialAdmin__Password` en el gestor de secretos del entorno (por ejemplo, Azure Key Vault o variables protegidas del servicio). Configura también `ConnectionStrings__DefaultConnection`. Aplica las migraciones como un paso controlado del despliegue antes de arrancar la aplicación. En producción el arranque falla si falta el correo o la contraseña iniciales; no se crea una cuenta con clave por defecto.

La cookie de autenticación es HttpOnly, Secure, SameSite=Lax y expira tras ocho horas de inactividad (con renovación deslizante). La aplicación redirige HTTP a HTTPS.
