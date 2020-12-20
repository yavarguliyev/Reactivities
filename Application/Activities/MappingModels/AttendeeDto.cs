namespace Application.Activities.MappingModels
{
  public class AttendeeDto
  {
    public string UserName { get; set; }
    public string DisplayName { get; set; }
    public string Image { get; set; }
    public bool IsHost { get; set; }
    public bool Following { get; set; }
  }
}