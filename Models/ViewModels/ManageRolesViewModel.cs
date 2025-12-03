namespace SporSalonu.Models.ViewModels
{
    public class ManageRolesViewModel
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public List<RoleSelectionViewModel> Roles { get; set; } = new();
    }

    public class RoleSelectionViewModel
    {
        public required string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}