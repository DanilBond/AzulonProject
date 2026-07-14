using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Azulon.Configuration.Validation;

namespace Azulon.Configuration.Game.Validation
{
    public sealed class GameSessionConfigValidationResult
    {
        private readonly ReadOnlyCollection<CatalogValidationIssue> _issues;

        public GameSessionConfigValidationResult(IList<CatalogValidationIssue> issues)
        {
            _issues = new List<CatalogValidationIssue>(issues).AsReadOnly();
        }

        public IReadOnlyList<CatalogValidationIssue> Issues => _issues;

        public bool IsValid
        {
            get
            {
                foreach (var issue in _issues)
                {
                    if (issue.Severity == CatalogValidationSeverity.Error)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public string FormatErrors()
        {
            var builder = new StringBuilder();
            foreach (var issue in _issues)
            {
                if (issue.Severity != CatalogValidationSeverity.Error)
                {
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                builder.Append("- ");
                builder.Append(issue.Message);
            }

            return builder.ToString();
        }
    }
}
