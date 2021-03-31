using Swetugg.Tix.Organization.Contract;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Organization
{
    public interface IOrganizationQueries
    {
        Task<OrganizationInfo> GetOrganization(Guid organizationId);
        Task<OrganizationInfo[]> ListOrganizations(Guid userId);
    }
}