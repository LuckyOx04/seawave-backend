namespace Services;

public static class HtmlFormattingService
{
    private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string WebPagesFolderPath = Path.Combine(BaseDirectory, "WebPages");
    private static readonly string EmailTemplatesFolderPath = Path.Combine(BaseDirectory, "EmailTemplates");

    private static async Task<string> GetFormattedStaticInfoHtml(string title, string message, string color)
    {
        var html = await File.ReadAllTextAsync(Path.Combine(WebPagesFolderPath, "StaticInfo",
            "static-info.html"));
        var css = await File.ReadAllTextAsync(Path.Combine(WebPagesFolderPath, "StaticInfo",
            "static-info.css"));
        
        html = html.Replace("{CustomCSS}", css).Replace("{color}", color).Replace("{title}", title)
            .Replace("{message}", message);
        
        return html;
    }

    private static async Task<string> GetEmailTemplate(string fileName, string token)
    {
        var template = await File.ReadAllTextAsync(Path.Combine(EmailTemplatesFolderPath, fileName));
        template = template.Replace("{token}", token);
        
        return template;
    }
    
    public static async Task<string> GetConfirmEmailResultPage(bool success)
    {
        var title = success ? "Email confirmed" : "Link Expired";
        var message = success ? "Your email has been confirmed. You can now log in to the desktop app." 
            : "This confirmation link is invalid or has already been used.";
        var color = success ? "#28a745" : "#F44336";
        
        var html = await GetFormattedStaticInfoHtml(title, message, color);
        
        return html;
    }

    public static async Task<string> GetPasswordChangeResultPage(bool success)
    {
        var title = success ? "Password changed." : "Password reset failed.";
        var message = success
            ? "Your password has been updated. You can now log in to the Seawave app with your new credentials."
            : "The link has expired or the token is invalid.";
        var color = success ? "#28a745" : "#dc3545";

        var html = await GetFormattedStaticInfoHtml(title, message, color);
        
        return html;
    }

    public static async Task<string> GetPasswordChangeFormPage(string token)
    {
        var html = await File.ReadAllTextAsync(Path.Combine(WebPagesFolderPath, "FormPage", "form-page.html"));
        var css = await File.ReadAllTextAsync(Path.Combine(WebPagesFolderPath, "FormPage", "form-page.css"));
        var js = await File.ReadAllTextAsync(Path.Combine(WebPagesFolderPath, "FormPage", "form-page.js"));

        html = html.Replace("{CustomCSS}", css).Replace("{token}", token).Replace("{CustomJS}", js);
        
        return html;
    }

    public static async Task<string> GetPasswordResetEmailBody(string token)
    {
        var body = await GetEmailTemplate("password-reset.html", token);
        return body;
    }

    public static async Task<string> GetVerifyEmailBody(string token)
    {
        var body = await GetEmailTemplate("verify-email.html", token);
        return body;
    }
}