using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleRestApiQuestions
{
    class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //When No Auth is required
            var noAuthRequired = context.ApiDescription.CustomAttributes().Any(attr => attr.GetType() == typeof(AllowAnonymousAttribute));
            if (noAuthRequired) return;


            //When Auth is required
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                    .Union(context.MethodInfo.GetCustomAttributes(true))
                                    .OfType<AuthorizeAttribute>();

            if (attributes != null && attributes.Count() > 0)
            {
                var attr = attributes.ToList()[0];

                // Add what should be show inside the security section
                IList<string> securityInfos = new List<string>();
                securityInfos.Add($"{nameof(AuthorizeAttribute.Policy)}:{attr.Policy}");
                securityInfos.Add($"{nameof(AuthorizeAttribute.Roles)}:{attr.Roles}");
                securityInfos.Add($"{nameof(AuthorizeAttribute.AuthenticationSchemes)}:{attr.AuthenticationSchemes}");

                operation.Security = new List<OpenApiSecurityRequirement>()
                    {
                        new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "bearer", // Must fit the defined Id of SecurityDefinition in global configuration
                                        Type = ReferenceType.SecurityScheme
                                    }
                                },
                                securityInfos
                            }
                        }
                    };
            }
            else
            {
                operation.Security.Clear();
            }
        }
    }
}
