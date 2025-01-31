using Blazor.Diagrams.Core.Models;
using DotNetSolutionTools.Core.Models;
using Microsoft.AspNetCore.Components;

namespace DotNetSolutionTools.Photino.Models;

public class CustomNodeModel : NodeModel
{
    public required Project Project { get; set; }
    public bool Highlighted { get; set; }
    public event EventHandler RefreshWidget = null!;

    public void InvokeRefreshWidget()
    {
        RefreshWidget?.Invoke(this, EventArgs.Empty);
    }
}
