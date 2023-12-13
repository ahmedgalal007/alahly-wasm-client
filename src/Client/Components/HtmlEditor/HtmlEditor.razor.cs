using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.RichTextEditor;

namespace FSH.BlazorWebAssembly.Client.Components.HtmlEditor;

public partial class HtmlEditor : IDisposable
{
    SfRichTextEditor rteObj;
    private bool dialogVisible { get; set; }
    [Parameter] public string HtmlValue { get; set; }

    [Parameter] public string? Title { get; set; }
    public List<ToolbarItemModel> Tools = new List<ToolbarItemModel>()
    {
        new ToolbarItemModel() { Command = ToolbarCommand.Bold },
        new ToolbarItemModel() { Command = ToolbarCommand.Italic },
        new ToolbarItemModel() { Command = ToolbarCommand.Underline },
        new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        new ToolbarItemModel() { Command = ToolbarCommand.Formats },
        new ToolbarItemModel() { Command = ToolbarCommand.Alignments },
        new ToolbarItemModel() { Command = ToolbarCommand.OrderedList },
        new ToolbarItemModel() { Command = ToolbarCommand.UnorderedList },
        new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        new ToolbarItemModel() { Command = ToolbarCommand.CreateLink },
        new ToolbarItemModel() { Command = ToolbarCommand.Image },
        new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        new ToolbarItemModel() { Name = "Symbol", TooltipText = "Select Image From Server" },
        new ToolbarItemModel() { Command = ToolbarCommand.SourceCode },
        new ToolbarItemModel() { Command = ToolbarCommand.Undo },
        new ToolbarItemModel() { Command = ToolbarCommand.Redo }
    };

    public HtmlEditor()
    {
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    public void Dispose()
    {
        // throw new NotImplementedException();
    }

    //  protected override async Task OnInitializedAsync()
    //  {
    //      this.HtmlValue = ""
    // }
    private async Task ClickHandler()
    {
        this.dialogVisible = true;
        await this.rteObj.SaveSelectionAsync();
    }
}

