using System;

namespace Microsoft.AspNetCore.Razor.Language;

public abstract class RazorProjectEngineFeatureBase : IRazorProjectEngineFeature
{
    private RazorProjectEngine _projectEngine;

    public virtual RazorProjectEngine ProjectEngine
    {
        get => _projectEngine;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _projectEngine = value;
            OnInitialized();
        }
    }

    protected virtual void OnInitialized() { }
}
