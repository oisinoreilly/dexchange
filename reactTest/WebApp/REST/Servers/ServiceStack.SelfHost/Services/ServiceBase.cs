using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Contracts;
using Core.Repositories;
using Core.UnitsOfWork;

namespace ServiceStack.SelfHost.Services
{
    [Authenticate]
    public class ServiceBase : Service
    {
        //private IDalUoW _uow; //TODO: Fix this to work, as impl is not matching interface.
      //  protected DocumentUoW Uow;

        // TODO: Make this an interface.
        //protected SQLRepository InfaRepos;

        public ServiceBase()
        {
            string dbName = "DExchange";
            string dbLocation = @"C:\DExchange\DB";
            var infraRepos = new SQLRepository(dbName, dbLocation);

            Uow = new DocumentUoW(infraRepos); /*, databaseRepos, nodeRepos, itemRepos*/
            //To Post or PUt for the create and update operations:
            //Do you name your URL objects you create explicitly, or let the server decide? If you name them then use PUT. If you let the server decide then use POST.
            //PUT is idempotent, so if you PUT an object twice, it has no effect. This is a nice property, so I would use PUT when possible.
            //You can update or create a resource with PUT with the same object URL
            //With POST you can have 2 requests coming in at the same time making modifications to a URL, and they may update different parts of the object.

            //Instantiate our unit of work, which is responsible for handling our requests
            //Note: Returning as a json response, otherwise a requerst of "text/html" will be unable to decode the output.
            //This encoding is unnecessary if you are not making queries from a web browser.
        }
    }
}
