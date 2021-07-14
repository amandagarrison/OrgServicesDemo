using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Relationship = Microsoft.Xrm.Sdk.Relationship;
using Entity = Microsoft.Xrm.Sdk.Entity;
using CrmBootcampEntities;

namespace OrgServiceDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://orga05e5bfd.crm.dynamics.com";
            string userName = "admin@CRM271156.onmicrosoft.com";
            string password = "9i1ZtUe71G";

            string conn = $@"
                Url = {url};
                AuthType = OAuth;
                UserName = {userName};
                Password = {password};
                AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
                RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
                LoginPrompt=Auto;
                RequireNewInstance = True";

            using (var svc = new CrmServiceClient(conn))
            {
                WhoAmIRequest request = new WhoAmIRequest();

                WhoAmIResponse response = (WhoAmIResponse)svc.Execute(request);

                Console.WriteLine("Your UserId is {0}", response.UserId);

                string fetchXml = @"
                    <fetch top='50' >
                      <entity name='account' >
                        <attribute name='name' />
                        <filter>
                          <condition
                            attribute='address1_city'
                            operator='eq'
                            value='Redmond' />
                        </filter>
                        <order attribute='name' />
                      </entity>
                    </fetch>";

                var query = new FetchExpression(fetchXml);

                EntityCollection results = svc.RetrieveMultiple(query);

                results.Entities.ToList().ForEach(x =>
                {
                    Console.WriteLine(x.Attributes["name"]);
                });

                
            }
            using (var svc = new CrmServiceClient(conn))
            {
                //var result = CreateEntityLateBound(svc);
                var result = CreateEntityEarlyBound(svc);
                Console.WriteLine($"{result.GetType()}: {result} ");

                UpdateAccout(svc, result);
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();


        }

        // ----- Late Bound -----

        //private static Guid CreateEntityLateBound(IOrganizationService svc)
        //{
        ////Use Entity class with entity logical name
        //var account = new Entity("account");

        ////set attribute values
        ////string primary name
        //account["name"] = "Bootcamp Account";
        ////telephone
        //account["telephone1"] = "350-555-1234";
        ////websiteurl
        //account["websiteurl"] = "www.CrmBootcampl.org";
        ////Boolean (two options)
        //account["creditonhold"] = false;
        ////DateTime
        //account["lastonholdtime"] = new DateTime(2021, 1, 1);
        ////Double
        //account["address1_latitude"] = 47.642311;
        //account["address1_longitude"] = -122.136841;
        ////Int
        //account["numberofemployees"] = 500;
        ////Money
        //account["revenue"] = new Money(new decimal(5000000.00));
        ////Picklist (option set)
        //account["accountcategorycode"] = new OptionSetValue(1);

        ////Create the accout
        //Guid accountid = svc.Create(account);
        //return accountid;
        //}


        // -----Early Bound -----
        private static Guid CreateEntityEarlyBound(IOrganizationService svc)
            {
            var account = new Account();
            // set attribute values 
                //string primary name
                account.Name = "Contoso";
                //Boolean (Two Option)
                account.CreditOnHold = false;
                //DateTime
                 account.LastOnHoldTime = new DateTime(2017, 1, 1);
                //Double
                account.Address1_Latitude = 47.642311;
                account.Address1_Longitude = -122.136841;
                //Int
                account.NumberOfEmployees = 500;
                //Money
                account.Revenue = new Money(new decimal(5000000.00));
                //Picklist (option set)
                account.AccountCategoryCode = new OptionSetValue(1); //Prefered customer

            //Create the account 
            Guid accountid = svc.Create(account);
            return accountid;

            }
            public static void UpdateAccout(IOrganizationService svc, Guid accountGuid)
            {
                // --- Late Bound Version --- 
                //var retrievedAccount = new Entity("account", accountGuid);
                //retrievedAccount["telephone1"] = "216-911-0987";

                //svc.Update(retrievedAccount);

                // --- Early Bound Version --- 

            }


    }
}
