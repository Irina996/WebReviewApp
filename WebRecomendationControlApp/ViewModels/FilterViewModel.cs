using Microsoft.AspNetCore.Mvc.Rendering;
using WebRecomendationControlApp.Models;

namespace WebRecomendationControlApp.ViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(List<ReviewGroup> groups, int? group, string name)
        {
            groups.Insert(0, new ReviewGroup { Name = "All", Id = 0 });
            ReviewGroups = new SelectList(groups, "Id", "Name", group);
            SelectedGroup = group;
            SelectedName = name;
        }
        public SelectList ReviewGroups { get; private set; }
        public int? SelectedGroup { get; private set; }
        public string SelectedName { get; private set; }
    }
}
