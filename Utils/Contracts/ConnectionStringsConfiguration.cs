namespace Utils.Contracts
{
    public sealed class ConnectionStringsConfiguration
    {
        public string Web { get; set; }
        public ConnectionStringsConfiguration()
        {
            Web = string.Empty;
        }
    }
}
