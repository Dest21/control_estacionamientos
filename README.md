# Sistema de Control de Estacionamientos

Un sistema completo de gestión de estacionamientos desarrollado con .NET 8, Blazor Server, Entity Framework Core y SQL Server, containerizado con Docker.

## 🚀 Características

- **Backend API REST** con .NET 8 WebAPI
- **Frontend interactivo** con Blazor Server
- **Sistema de autenticación** con login seguro
- **Base de datos** SQL Server con Entity Framework Core
- **Containerización** completa con Docker
- **Auto-inicialización** de base de datos y datos de prueba
- **Interfaz moderna y responsive** con Bootstrap 5 y diseño personalizado
- **Animaciones y efectos visuales** con AOS y CSS avanzado
- **Dashboard interactivo** con estadísticas en tiempo real

## 📋 Funcionalidades

### Gestión de Clientes
- ✅ Registrar nuevos clientes
- ✅ Modificar información de clientes existentes
- ✅ Listar y filtrar clientes
- ✅ Desactivar clientes del sistema

### Gestión de Autos
- ✅ Registrar autos asociados a clientes
- ✅ Modificar información de autos
- ✅ Listar autos con filtros avanzados
- ✅ Gestión por cliente, modelo, placa, etc.

### Gestión de Espacios de Estacionamiento
- ✅ Registrar espacios por zonas
- ✅ Diferentes tipos de espacios (Regular, Compacto, VIP, Discapacitados)
- ✅ Control de disponibilidad en tiempo real
- ✅ Filtros por zona y tipo

### Gestión de Asignaciones
- ✅ Asignar espacios a clientes y autos
- ✅ Control de fechas de inicio y fin
- ✅ Estados de asignación (Activo, Expirado, Cancelado)
- ✅ Revocar asignaciones
- ✅ Historial completo

### Sistema de Autenticación
- ✅ Login seguro con validación
- ✅ Gestión de sesiones
- ✅ Protección de rutas
- ✅ Logout automático

### Dashboard y Reportes
- ✅ Dashboard moderno con estadísticas visuales
- ✅ Gráficos interactivos de ocupación
- ✅ Reportes avanzados con métricas
- ✅ Análisis de tendencias y actividad
- ✅ Exportación de datos

## 🛠️ Tecnologías Utilizadas

- **Backend**: .NET 8, ASP.NET Core WebAPI
- **Frontend**: Blazor Server, Tailwind CSS
- **Base de Datos**: SQL Server 2022, Entity Framework Core 9
- **Containerización**: Docker, Docker Compose
- **Documentación API**: Swagger/OpenAPI
- **UI Framework**: Tailwind CSS con componentes personalizados
- **Iconografía**: Bootstrap Icons
- **Animaciones**: AOS (Animate On Scroll)

## 🏗️ Arquitectura

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Blazor UI     │    │   WebAPI         │    │   SQL Server    │
│   (Frontend)    │◄──►│   (Backend)      │◄──►│   (Database)    │
│   Port: 5000    │    │   Port: 5000     │    │   Port: 1433    │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

## 🚀 Instalación y Ejecución

### Prerrequisitos

- Docker Desktop instalado
- Git (para clonar el repositorio)

### Ejecución con Docker (Recomendado)

1. **Clonar el repositorio**
   ```bash
   git clone <repository-url>
   cd estacionamientos
   ```

2. **Ejecutar con Docker Compose**
   ```bash
   docker-compose up --build
   ```

3. **Acceder a la aplicación**
   - **Frontend/Dashboard**: http://localhost:5000
   - **API Documentation**: http://localhost:5000/swagger
   - **Health Check**: http://localhost:5000/health

4. **Credenciales de acceso**
   - **Usuario**: `admin`
   - **Contraseña**: `admin123`

### Ejecución Local (Desarrollo)

1. **Prerrequisitos adicionales**
   - .NET 8 SDK
   - SQL Server (local o Docker)

2. **Configurar la base de datos**
   ```bash
   cd EstacionamientosApp
   dotnet ef database update
   ```

3. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

## 🗄️ Estructura de la Base de Datos

### Tablas Principales

#### Clients (Clientes)
- `Id` (PK): Identificador único
- `FirstName`: Nombres
- `LastName`: Apellidos  
- `DocumentNumber`: Número de documento (único)
- `DocumentType`: Tipo de documento (DNI, Pasaporte, etc.)
- `Email`: Correo electrónico (único)
- `Phone`: Teléfono
- `Address`: Dirección
- `RegistrationDate`: Fecha de registro
- `IsActive`: Estado activo/inactivo

#### Cars (Autos)
- `Id` (PK): Identificador único
- `LicensePlate`: Placa del vehículo (única)
- `Brand`: Marca
- `Model`: Modelo
- `Color`: Color
- `Year`: Año
- `ClientId` (FK): Referencia al cliente propietario
- `RegistrationDate`: Fecha de registro
- `IsActive`: Estado activo/inactivo

#### ParkingSpaces (Espacios de Estacionamiento)
- `Id` (PK): Identificador único
- `SpaceNumber`: Número del espacio (único)
- `Zone`: Zona del estacionamiento
- `SpaceType`: Tipo (Regular, Compact, VIP, Disabled)
- `IsAvailable`: Disponibilidad
- `Description`: Descripción
- `CreatedDate`: Fecha de creación
- `IsActive`: Estado activo/inactivo

#### ParkingAssignments (Asignaciones)
- `Id` (PK): Identificador único
- `ClientId` (FK): Referencia al cliente
- `CarId` (FK): Referencia al auto
- `ParkingSpaceId` (FK): Referencia al espacio
- `AssignedDate`: Fecha de asignación
- `StartDate`: Fecha de inicio
- `EndDate`: Fecha de fin
- `Status`: Estado (Active, Expired, Cancelled, Suspended)
- `Notes`: Notas adicionales
- `IsActive`: Estado activo/inactivo

## 🔌 API Endpoints

### Clientes
- `GET /api/clients` - Listar clientes
- `GET /api/clients/{id}` - Obtener cliente por ID
- `POST /api/clients` - Crear nuevo cliente
- `PUT /api/clients/{id}` - Actualizar cliente
- `DELETE /api/clients/{id}` - Desactivar cliente

### Autos
- `GET /api/cars` - Listar autos
- `GET /api/cars/{id}` - Obtener auto por ID
- `GET /api/cars/ByClient/{clientId}` - Autos por cliente
- `POST /api/cars` - Crear nuevo auto
- `PUT /api/cars/{id}` - Actualizar auto
- `DELETE /api/cars/{id}` - Desactivar auto

### Espacios de Estacionamiento
- `GET /api/parkingspaces` - Listar espacios
- `GET /api/parkingspaces/{id}` - Obtener espacio por ID
- `GET /api/parkingspaces/Available` - Espacios disponibles
- `GET /api/parkingspaces/Zones` - Listar zonas
- `GET /api/parkingspaces/Types` - Tipos de espacios
- `POST /api/parkingspaces` - Crear nuevo espacio
- `PUT /api/parkingspaces/{id}` - Actualizar espacio
- `DELETE /api/parkingspaces/{id}` - Desactivar espacio

### Asignaciones
- `GET /api/parkingassignments` - Listar asignaciones
- `GET /api/parkingassignments/{id}` - Obtener asignación por ID
- `GET /api/parkingassignments/Active` - Asignaciones activas
- `GET /api/parkingassignments/ByClient/{clientId}` - Por cliente
- `POST /api/parkingassignments` - Crear asignación
- `PUT /api/parkingassignments/{id}` - Actualizar asignación
- `DELETE /api/parkingassignments/{id}` - Cancelar asignación
- `POST /api/parkingassignments/{id}/Revoke` - Revocar asignación

## 🔐 Credenciales por Defecto

### Sistema de Login
- **Usuario**: `admin`
- **Contraseña**: `admin123`
- **Rol**: Administrador

### Base de Datos SQL Server
- **Usuario**: `sa`
- **Contraseña**: `YourStrong@Passw0rd`
- **Base de Datos**: `EstacionamientosDB`
- **Puerto**: `1433`

## 📊 Conexión con SQL Server Management Studio (SSMS)

### Configuración de Conexión

Una vez que el sistema esté ejecutándose con `docker-compose up`, puedes conectarte a la base de datos usando SSMS:

**Datos de Conexión:**
- **Tipo de servidor**: Motor de base de datos
- **Nombre del servidor**: `localhost,1433` o `127.0.0.1,1433`
- **Autenticación**: Autenticación de SQL Server
- **Inicio de sesión**: `sa`
- **Contraseña**: `YourStrong@Passw0rd`

### Base de Datos y Tablas

Una vez conectado, encontrarás la base de datos `EstacionamientosDB` con las siguientes tablas:

- **Users** - Usuarios del sistema (admin/admin123)
- **Clients** - Clientes registrados (4 ejemplos)
- **Cars** - Vehículos registrados (5 ejemplos)
- **ParkingSpaces** - Espacios de estacionamiento (43 espacios)
- **ParkingAssignments** - Asignaciones activas (3 ejemplos)

### Consultas de Ejemplo

```sql
-- Ver todos los usuarios del sistema
SELECT * FROM Users;

-- Ver clientes activos
SELECT Id, FirstName, LastName, Email, Phone
FROM Clients
WHERE IsActive = 1;

-- Ver espacios disponibles por zona
SELECT Zone, COUNT(*) as Total,
       SUM(CASE WHEN IsAvailable = 1 THEN 1 ELSE 0 END) as Disponibles,
       SUM(CASE WHEN IsAvailable = 0 THEN 1 ELSE 0 END) as Ocupados
FROM ParkingSpaces
WHERE IsActive = 1
GROUP BY Zone;

-- Ver asignaciones activas con detalles completos
SELECT
    pa.Id,
    c.FirstName + ' ' + c.LastName AS Cliente,
    car.LicensePlate AS Placa,
    car.Brand + ' ' + car.Model AS Vehiculo,
    ps.SpaceNumber AS Espacio,
    ps.Zone AS Zona,
    pa.Status,
    pa.AssignedDate,
    pa.StartDate,
    pa.EndDate
FROM ParkingAssignments pa
JOIN Clients c ON pa.ClientId = c.Id
JOIN Cars car ON pa.CarId = car.Id
JOIN ParkingSpaces ps ON pa.ParkingSpaceId = ps.Id
WHERE pa.Status = 'Active' AND pa.IsActive = 1;

-- Estadísticas generales del sistema
SELECT
    (SELECT COUNT(*) FROM Clients WHERE IsActive = 1) as TotalClientes,
    (SELECT COUNT(*) FROM Cars WHERE IsActive = 1) as TotalVehiculos,
    (SELECT COUNT(*) FROM ParkingSpaces WHERE IsActive = 1) as TotalEspacios,
    (SELECT COUNT(*) FROM ParkingSpaces WHERE IsAvailable = 0 AND IsActive = 1) as EspaciosOcupados,
    (SELECT COUNT(*) FROM ParkingAssignments WHERE Status = 'Active' AND IsActive = 1) as AsignacionesActivas;
```

### Notas Importantes

⚠️ **Requisitos para la conexión:**
- El contenedor de Docker debe estar ejecutándose
- El puerto 1433 debe estar disponible en tu sistema
- Usar las credenciales exactas proporcionadas

⚠️ **Persistencia de datos:**
- Los datos se mantienen mientras el volumen Docker exista
- Si ejecutas `docker-compose down -v`, se eliminarán todos los datos
- Para mantener los datos, usa solo `docker-compose down` (sin -v)

💡 **Tip:** Puedes usar estas consultas para verificar que los datos se están creando correctamente desde la aplicación web.

## 📊 Datos de Prueba

El sistema incluye datos de prueba que se cargan automáticamente:

- **1 usuario administrador** (admin/admin123)
- **4 clientes** de ejemplo
- **5 autos** registrados
- **43 espacios** de estacionamiento distribuidos en zonas:
  - Zona A: 20 espacios regulares
  - Zona B: 15 espacios compactos
  - Zona C: 5 espacios VIP
  - Zona Especial: 3 espacios para discapacitados
- **3 asignaciones** activas de ejemplo

## 🐳 Configuración Docker

### Servicios

1. **estacionamientos-app**
   - Puerto: 5000
   - Imagen: Construida desde Dockerfile
   - Dependencias: SQL Server

2. **sqlserver**
   - Puerto: 1433
   - Imagen: mcr.microsoft.com/mssql/server:2022-latest
   - Volumen persistente para datos

### Volúmenes
- `sqlserver_data`: Persistencia de datos de SQL Server

### Redes
- `estacionamientos-network`: Red interna para comunicación entre servicios

## 🔧 Configuración de Desarrollo

### Variables de Entorno

```bash
# Desarrollo local
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection="Server=localhost;Database=EstacionamientosDB;Trusted_Connection=true;"

# Producción (Docker)
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="Server=sqlserver;Database=EstacionamientosDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;"
```

## 🚨 Solución de Problemas

### Error de Conexión a Base de Datos
```bash
# Verificar que SQL Server esté ejecutándose
docker-compose ps

# Ver logs del contenedor de base de datos
docker-compose logs sqlserver

# Reiniciar servicios
docker-compose restart
```

### Puerto en Uso
```bash
# Cambiar puerto en docker-compose.yml
ports:
  - "5001:5000"  # Cambiar 5000 por otro puerto disponible
```

### Limpiar y Reconstruir
```bash
# Limpiar contenedores y volúmenes
docker-compose down -v

# Reconstruir desde cero
docker-compose up --build --force-recreate
```

## 📝 Notas de Desarrollo

- La aplicación usa **EnsureCreated()** en lugar de migraciones para simplicidad en Docker
- Los datos se inicializan automáticamente en el primer arranque
- **Sistema de autenticación** basado en sesiones con JavaScript sessionStorage
- **Interfaz moderna** con gradientes, animaciones y efectos visuales
- **Diseño responsive** optimizado para dispositivos móviles y desktop
- **Componentes interactivos** con Blazor Server y renderizado en tiempo real
- Todas las operaciones incluyen validaciones tanto en frontend como backend

## 🎨 Características de Diseño

- **Tailwind CSS** como framework principal de diseño
- **Tema moderno** con gradientes y efectos glassmorphism
- **Animaciones suaves** con AOS (Animate On Scroll) y CSS personalizado
- **Iconografía consistente** con Bootstrap Icons
- **Tipografía profesional** con Google Fonts (Inter y Poppins)
- **Sistema de colores personalizado** con paleta extendida
- **Cards interactivas** con efectos hover y transformaciones
- **Dashboard visual** con gráficos y métricas en tiempo real
- **Navegación intuitiva** con sidebar colapsible y menús contextuales
- **Componentes responsivos** optimizados para todos los dispositivos
- **Efectos de carga** y transiciones suaves

## 🤝 Contribución

1. Fork el proyecto
2. Crear una rama para la funcionalidad (`git checkout -b feature/AmazingFeature`)
3. Commit los cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## 👥 Autor

Desarrollado como sistema de demostración para gestión de estacionamientos.

---

## 🔄 Instrucciones de Ejecución Completa

### Desde Cero (Recomendado)

1. **Limpiar Docker completamente**
   ```bash
   docker-compose down -v --remove-orphans
   docker system prune -a -f
   ```

2. **Construir y ejecutar**
   ```bash
   docker-compose build --no-cache
   docker-compose up
   ```

3. **Acceder al sistema**
   - Abrir navegador en: http://localhost:5000
   - Usar credenciales: `admin` / `admin123`

---

**¡Listo para usar!** 🚀

Simplemente ejecuta los comandos anteriores y tendrás un sistema completo de gestión de estacionamientos con interfaz moderna y sistema de login funcionando en minutos.