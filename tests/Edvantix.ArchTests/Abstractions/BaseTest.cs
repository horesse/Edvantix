using System.Reflection;
using Edvantix.Chassis;
using Edvantix.Constants;
using Edvantix.Notification;
using Edvantix.Organizational;
using Edvantix.Persona;
using Edvantix.Scheduler;
using Edvantix.SharedKernel;

namespace Edvantix.ArchTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Assembly PersonaAssembly = typeof(IPersonaApiMarker).Assembly;
    protected static readonly Assembly NotificationAssembly =
        typeof(INotificationApiMarker).Assembly;
    protected static readonly Assembly SchedulerAssembly = typeof(ISchedulerApiMarker).Assembly;
    protected static readonly Assembly OrganizationalAssembly =
        typeof(IOrganizationalApiMarker).Assembly;
    protected static readonly Assembly ChassisAssembly = typeof(IChassisMarker).Assembly;
    protected static readonly Assembly ConstantsAssembly = typeof(IConstantsMarker).Assembly;
    protected static readonly Assembly SharedKernelAssembly = typeof(ISharedKernelMarker).Assembly;
}
