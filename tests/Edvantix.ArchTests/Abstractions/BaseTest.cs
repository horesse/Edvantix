using System.Reflection;
using Edvantix.Blog;
using Edvantix.Catalog;
using Edvantix.Chassis;
using Edvantix.Constants;
using Edvantix.Notification;
using Edvantix.Organizational;
using Edvantix.Persona;
using Edvantix.Scheduler;
using Edvantix.SharedKernel;
using Edvantix.Subscriptions;

namespace Edvantix.ArchTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Assembly BlogAssembly = typeof(IBlogApiMarker).Assembly;
    protected static readonly Assembly CatalogAssembly = typeof(ICatalogApiMarker).Assembly;
    protected static readonly Assembly OrganizationalAssembly = typeof(IOrganizationalApiMarker).Assembly;
    protected static readonly Assembly PersonaAssembly = typeof(IPersonaApiMarker).Assembly;
    protected static readonly Assembly NotificationAssembly = typeof(INotificationApiMarker).Assembly;
    protected static readonly Assembly SchedulerAssembly = typeof(ISchedulerApiMarker).Assembly;
    protected static readonly Assembly SubscriptionsAssembly = typeof(ISubscriptionsApiMarker).Assembly;
    protected static readonly Assembly ChassisAssembly = typeof(IChassisMarker).Assembly;
    protected static readonly Assembly ConstantsAssembly = typeof(IConstantsMarker).Assembly;
    protected static readonly Assembly SharedKernelAssembly = typeof(ISharedKernelMarker).Assembly;
}

