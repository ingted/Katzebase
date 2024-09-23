namespace NTDLS.Katzebase.Management
{
    internal class ManagementSettings
    {
        public int MaximumRows { get; set; } = 1000;
        public int UIQueryTimeOut { get; set; } = 10;
        public int QueryTimeOut { get; set; } = -1;
    }
}
