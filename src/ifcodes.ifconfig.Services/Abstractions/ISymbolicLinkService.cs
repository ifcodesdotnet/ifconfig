
namespace ifcodes.ifconfig.Services.Abstractions
{
    public interface ISymbolicLinkService
    {
        void ApplyConfiguration(string path);

        void ApplyConfiguration(string path, string application); 

        void RemoveConfiguration(string path, string configure); 
    }
}