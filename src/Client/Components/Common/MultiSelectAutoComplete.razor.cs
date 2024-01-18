//using Microsoft.AspNetCore.Components;
//using MudBlazor;
//using TItem = System.Type;
//using System.Net.Http.Json;

//namespace FSH.BlazorWebAssembly.Client.Components.Common;

//public partial class MultiSelectAutoComplete
//{
//    [Inject] HttpClient httpClient { get; set; }
//    [Parameter] public TItem T { get; set; }
//    private Element value;
//    private List<Element> elements = new List<Element>() {
//        new Element{ID="1", Name="Ahmed"},
//        new Element{ID="2", Name="Said"},
//        new Element{ID="3", Name="Mohammed"},
//    };
//    private async Task<IEnumerable<Element>> Search(string value)
//    {
//        await Task.Delay(5);

//        // ?return await httpClient.GetFromJsonAsync<List<Element>>($"webapi/periodictable/{value}");
//        return elements.Where(e => e.Name.Contains(value));
//    }
    
//    [Parameter]
//    public IEnumerable<TItem>? Values { get; set; }

//    [Parameter]
//    public EventCallback<IEnumerable<TItem>> ValuesChanged { get; set; }

//    [Parameter]
//    public string Label { get; set; }

//    [Parameter, EditorRequired]
//    public Func<string, Task<IEnumerable<TItem>>> SearchFunc { get; set; }

//    [Parameter, EditorRequired]
//    public Func<TItem, string> GetNameFunc { get; set; }

//    [Parameter, EditorRequired]
//    public Func<TItem, int> GetIDFunc { get; set; }

//    private HashSet<TItem> selectedValues = new HashSet<TItem>();

//    private async Task<IEnumerable<TItem>> FilteredSearchFunc(string searchText)
//    {
//        if (!string.IsNullOrEmpty(searchText))
//        {
//            var results = await SearchFunc(searchText);
//            return results.Except(selectedValues);
//        }

//        return Enumerable.Empty<TItem>();
//    }

//    protected override void OnParametersSet()
//    {
//        base.OnParametersSet();

//        if (Values == null)
//        {
//            selectedValues.Clear();
//        }
//        else if (!Values.SequenceEqual(selectedValues))
//        {
//            selectedValues = Values.ToHashSet();
//            ValuesChanged.InvokeAsync(Values);
//        }
//    }

//    private void RefreshBinding()
//    {
//        Values = selectedValues.ToList();
//        ValuesChanged.InvokeAsync(Values);
//        StateHasChanged();
//    }

//    private void RemoveValue(MudChip chip)
//    {
//        if (selectedValues.RemoveWhere(x => GetNameFunc(x) == chip.Text) > 0)
//            RefreshBinding();
//    }

//    private void AddValue(TItem newValue)
//    {
//        if (newValue != null)
//        {
//            if (selectedValues.Add(newValue))
//                RefreshBinding();
//        }
//    }

//    /// <summary>
//    /// Note that this is required to a) clear the control after you add
//    /// an item to the list, and b) to trigger the addvalue method.
//    /// If MudAutoComplete's bind-Value:after worked, we could get rid
//    /// of this and just clear the value after it was added.
//    /// </summary>
//    private TItem theValue
//    {
//        get => default(TItem);
//        set { AddValue(value); }
//    }

//}

//internal class Element
//{
//    public string ID { get; set; }
//    public string Name { get; set; }
//}

//namespace FSH.BlazorWebAssembly.Client;
