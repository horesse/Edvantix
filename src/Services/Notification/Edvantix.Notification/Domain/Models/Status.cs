using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Edvantix.Notification.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter<Status>))]
internal enum Status : byte
{
    [Description("New")]
    New = 0,

    [Description("Completed")]
    Completed = 1,

    [Description("Canceled")]
    Canceled = 2,
}
