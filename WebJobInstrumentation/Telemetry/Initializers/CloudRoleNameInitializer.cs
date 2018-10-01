using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace WebJobInstrumentation.Telemetry.Initializers
{
    public class CloudRoleNameInitializer : ITelemetryInitializer
    {
        private readonly string roleName;

        public CloudRoleNameInitializer(string roleName)
        {
            this.roleName = roleName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = this.roleName;
            telemetry.Context.Cloud.RoleInstance = this.roleName;
        }
    }
}
