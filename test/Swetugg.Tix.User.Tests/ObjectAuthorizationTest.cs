using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using Xunit;

namespace Swetugg.Tix.User.Tests
{
    public class ObjectAuthorizationTest
    {
        [Fact]
        public void one_attribute_with_matching_property_value()
        {
            var activityId = Guid.NewGuid();
            var target = new ObjectAuthorization(new[] {
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value = activityId.ToString()},
                } },
            });

            var actual = target.HasPermissionForObject("ReadActivityBasic", new
            {
                ActivityId = activityId,
                OrganizationId = Guid.NewGuid().ToString()
            });
            Assert.True(actual);
        }


        [Fact]
        public void two_attributes_with_matching_property_value()
        {
            var activityId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();

            var target = new ObjectAuthorization(new[] {
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value = activityId.ToString()},
                    new PermissionClaimAttrib() { Name = "OrganizationId", Value = organizationId.ToString()},
                } },
            });

            var actual = target.HasPermissionForObject("ReadActivityBasic", new
            {
                ActivityId = activityId,
                OrganizationId = organizationId
            });
            Assert.True(actual);
        }

        [Fact]
        public void two_attributes_with_one_wildcard_and_one_matching_property_value()
        {
            var activityId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();

            var target = new ObjectAuthorization(new[] {
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value = "*"},
                    new PermissionClaimAttrib() { Name = "OrganizationId", Value = organizationId.ToString()},
                } },
            });

            var actual = target.HasPermissionForObject("ReadActivityBasic", new
            {
                ActivityId = activityId,
                OrganizationId = organizationId
            });
            Assert.True(actual);
        }


        [Fact]
        public void two_attributes_with_one_wildcard_and_one_non_matching_property_value()
        {
            var activityId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();

            var target = new ObjectAuthorization(new[] {
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value = "*"},
                    new PermissionClaimAttrib() { Name = "OrganizationId", Value = organizationId.ToString()},
                } },
            });

            var actual = target.HasPermissionForObject("ReadActivityBasic", new
            {
                ActivityId = activityId,
                OrganizationId = Guid.NewGuid()
            });
            Assert.False(actual);
        }

        [Fact]
        public void no_attributes()
        {
            var organizationId = Guid.NewGuid();

            var target = new ObjectAuthorization(new[] {
                new PermissionClaim { PermissionCode = "CreateOrganization", Attributes = new List<PermissionClaimAttrib> {
                } },
            });

            var actual = target.HasPermissionForObject("CreateOrganization", new
            {
                OrganizationId = organizationId
            });
            Assert.True(actual);
        }

        [Fact]
        public void multiple_claims_with_second_matching()
        {
            var activityId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();

            var target = new ObjectAuthorization(new[] {
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value = Guid.NewGuid().ToString()},
                    new PermissionClaimAttrib() { Name = "OrganizationId", Value = organizationId.ToString()},
                } },
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value = activityId.ToString()},
                    new PermissionClaimAttrib() { Name = "OrganizationId", Value = organizationId.ToString()},
                } },
            });

            var actual = target.HasPermissionForObject("ReadActivityBasic", new
            {
                ActivityId = activityId,
                OrganizationId = organizationId
            });
            Assert.True(actual);
        }


        [Fact]
        public void multiple_claims_with_no_matching()
        {
            var activityId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();

            var target = new ObjectAuthorization(new[] {
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value = Guid.NewGuid().ToString()},
                    new PermissionClaimAttrib() { Name = "OrganizationId", Value = organizationId.ToString()},
                } },
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value =  Guid.NewGuid().ToString()},
                    new PermissionClaimAttrib() { Name = "OrganizationId", Value = organizationId.ToString()},
                } },
            });

            var actual = target.HasPermissionForObject("ReadActivityBasic", new
            {
                ActivityId = activityId,
                OrganizationId = organizationId
            });
            Assert.False(actual);
        }

        [Fact]
        public void two_attributes_with_matching_property_value_one_wildcard()
        {
            var activityId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();

            var target = new ObjectAuthorization(new[] {
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value = "*"},
                    new PermissionClaimAttrib() { Name = "OrganizationId", Value = organizationId.ToString()},
                } },
            });

            var actual = target.HasPermissionForObject("ReadActivityBasic", new
            {
                ActivityId = "*",
                OrganizationId = organizationId
            });
            Assert.True(actual);
        }

        [Fact]
        public void two_attributes_with_matching_property_value_wrong_wildcard()
        {
            var activityId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();

            var target = new ObjectAuthorization(new[] {
                new PermissionClaim { PermissionCode = "ReadActivityBasic", Attributes = new List<PermissionClaimAttrib> {
                    new PermissionClaimAttrib() { Name = "ActivityId", Value = "*"},
                    new PermissionClaimAttrib() { Name = "OrganizationId", Value = organizationId.ToString()},
                } },
            });

            var actual = target.HasPermissionForObject("ReadActivityBasic", new
            {
                ActivityId = "*",
                OrganizationId = "*"
            });
            Assert.False(actual);
        }
    }
}
