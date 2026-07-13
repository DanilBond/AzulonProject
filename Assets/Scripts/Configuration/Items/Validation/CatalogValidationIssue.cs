namespace Azulon.Configuration.Items.Validation
{
    public enum CatalogValidationSeverity
    {
        Warning = 0,
        Error = 1
    }

    public sealed class CatalogValidationIssue
    {
        public CatalogValidationIssue(CatalogValidationSeverity severity, string message)
        {
            Severity = severity;
            Message = message;
        }

        public CatalogValidationSeverity Severity { get; }

        public string Message { get; }
    }
}
