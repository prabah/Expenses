using ExpenseTracker.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Factories
{
    public class ClientFactory
    {

        public ClientFactory()
        {

        }

        public DTO.Client CreateClient(Client client)
        {
            return new DTO.Client()
            {
                Address1 = client.Address1,
                Address2 = client.Address2,
                Address3 = client.Address3,
                Postcode = client.Postcode,
                Description = client.Description,
                IsNonOrganisationlClient = client.IsNonOrganisationlClient,
                Email = client.Email,
                AdminEmail = client.AdminEmail,
                Id = client.Id,
                IsActive = client.IsActive
            };
        }



        public Client CreateClient(DTO.Client client)
        {
            return new Client()
            {
                Address1 = client.Address1,
                Address2 = client.Address2,
                Address3 = client.Address3,
                Postcode = client.Postcode,
                Description = client.Description,
                IsNonOrganisationlClient = client.IsNonOrganisationlClient,
                Email = client.Email,
                AdminEmail = client.AdminEmail,
                Id = client.Id,
                IsActive = client.IsActive
            };
        }


        public object CreateDataShapedObject(Client client, List<string> lstOfFields)
        {
 
            return CreateDataShapedObject(CreateClient(client), lstOfFields);
        }


        public object CreateDataShapedObject(DTO.Client client, List<string> lstOfFields)
        {

            if (!lstOfFields.Any())
            {
                return client;
            }
            else
            { 

                // create a new ExpandoObject & dynamically create the properties for this object

                ExpandoObject objectToReturn = new ExpandoObject();
                foreach (var field in lstOfFields)
                {
                    // need to include public and instance, b/c specifying a binding flag overwrites the
                    // already-existing binding flags.

                    var fieldValue = client.GetType()
                        .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                        .GetValue(client, null);

                    // add the field to the ExpandoObject
                    ((IDictionary<String, Object>)objectToReturn).Add(field, fieldValue);
                }

                return objectToReturn;
            }
        }
    }
}
