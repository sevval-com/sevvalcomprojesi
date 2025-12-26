using Sevval.Domain.Entities;

internal class UserDetailsViewModel
{
    public ApplicationUser User { get; set; }
    public List<IlanModel> Ilanlar { get; set; }
}