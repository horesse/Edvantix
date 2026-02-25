using MimeKit.Text;

namespace Edvantix.Notification.Domain.Builders;

/// <summary>
///     Fluent builder for constructing a <see cref="MimeMessage" /> representing
///     a welcome email sent to a newly registered user.
/// </summary>
/// <example>
/// <code>
/// var message = WelcomeMimeMessageBuilder
///     .Initialize()
///     .WithTo(notification.FullName, notification.Email)
///     .WithSubject("Welcome to Edvantix!")
///     .WithBody(renderedHtml)
///     .Build();
/// </code>
/// </example>
public sealed class WelcomeMimeMessageBuilder
{
    private WelcomeMimeMessageBuilder() { }

    private string? Subject { get; set; } = string.Empty;
    private MimeEntity Body { get; set; } = new TextPart(TextFormat.Html) { Text = string.Empty };
    private MailboxAddress To { get; set; } = new(string.Empty, string.Empty);

    /// <summary>
    ///     Creates a new builder instance.
    /// </summary>
    public static WelcomeMimeMessageBuilder Initialize() => new();

    /// <summary>
    ///     Sets the recipient of the email.
    /// </summary>
    /// <param name="fullName">The display name of the recipient (may be null).</param>
    /// <param name="email">The email address of the recipient. Must not be null or empty.</param>
    public WelcomeMimeMessageBuilder WithTo(string? fullName, string? email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        To = new(fullName, email);
        return this;
    }

    /// <summary>
    ///     Sets the email subject explicitly.
    /// </summary>
    /// <param name="subject">The subject line text.</param>
    public WelcomeMimeMessageBuilder WithSubject(string? subject)
    {
        Subject = subject;
        return this;
    }

    /// <summary>
    ///     Sets the email body from pre-rendered HTML content.
    /// </summary>
    /// <param name="htmlBody">The HTML string produced by the MJML template renderer.</param>
    public WelcomeMimeMessageBuilder WithBody(string? htmlBody)
    {
        var bb = new BodyBuilder { HtmlBody = htmlBody };
        Body = bb.ToMessageBody();
        return this;
    }

    /// <summary>
    ///     Builds the final <see cref="MimeMessage" />.
    /// </summary>
    /// <exception cref="NotificationException">
    ///     Thrown when the recipient email address was not provided before calling Build.
    /// </exception>
    public MimeMessage Build()
    {
        if (string.IsNullOrEmpty(To.Address))
        {
            throw new NotificationException(
                "Recipient email address must be set before building the message."
            );
        }

        var message = new MimeMessage();
        message.To.Add(To);
        message.Subject = Subject ?? "Welcome to Edvantix";
        message.Body = Body;
        message.Date = DateTimeOffset.UtcNow;
        return message;
    }
}
