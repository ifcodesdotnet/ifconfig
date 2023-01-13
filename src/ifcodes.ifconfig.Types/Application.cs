
namespace ifcodes.ifconfig.Types
{
    public class Application
    {
        public string Name { get; set; }

        public string TargetDirectory { get; set; }

        public bool IsSameNameAs(string application)
        {
            if (!string.IsNullOrEmpty(application))
            {
                if (this.Name.ToLower() == application.ToLower())
                {
                    return true;
                }
            }

            return false;
        }
    }
}