namespace Credio.Core.Application.Dtos.CoreConfiguration
{
    public class SystemSettingDTO
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
        public string? Description { get; set; }
        public string? GroupName { get; set; }
        public bool IsEditable { get; set; }
    }
}
