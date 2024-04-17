using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using Mapster;
using MediatR;
using static FSH.BlazorWebAssembly.Client.Pages.Keyword.Manage;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public static class AddEditModelLocalizer<TRowDTO, TLocalizedDto, TID, TRequest>
    // where TID : struct
    where TLocalizedDto : LocalizedDto
    where TRequest : LocalizedRequest
{
    public static void SyncWithContextLocals(EntityTableContext<TRowDTO, TID, TRequest> Context, TLocalizedDto rowDTO)
    {
        if (Context.AddEditModal.RequestModel.Locals == null)
            Context.AddEditModal.RequestModel.Locals = new List<TLocalizedDto>() as List<LocalizedDto>;
        var loc = Context.AddEditModal.RequestModel.Locals.Where(e => e.CulturCode == rowDTO.CulturCode).FirstOrDefault();
        if (loc == null)
        {
            // item.KeywordId = Context.AddEditModal.RequestModel.Id;
            Context.AddEditModal.RequestModel.Locals.Add(rowDTO);
            loc = Context.AddEditModal.RequestModel.Locals.Where(e => e.CulturCode == rowDTO.CulturCode).FirstOrDefault();
            // Context.AddEditModal.BuildAdapter();
        }
        else
        {
            // loc.Title = item.Title;
            // loc.Description = item.Description;
            loc.Enabled = rowDTO.Enabled;
            loc.IsDefault = rowDTO.IsDefault;
            loc.Adapt(rowDTO);
        }

        // If new and disabled remove the item
        if (loc != null && !loc.Enabled && (loc.Id == default || loc.Id == Guid.Empty))
        {
            Context.AddEditModal.RequestModel.Locals
                .Remove(Context.AddEditModal.RequestModel.Locals.FirstOrDefault(e => e.CulturCode == loc.CulturCode)!);
        }
    }
}
