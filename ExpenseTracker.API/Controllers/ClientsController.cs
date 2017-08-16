using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using Marvin.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using ExpenseTracker.API.Helpers;
using System.Web.Http.Routing;

namespace ExpenseTracker.API.Controllers
{
    [RoutePrefix("api")]
    public class ClientsController : ApiController
    {

        IExpenseTrackerRepository _repository;
        ClientFactory _clientFactory = new ClientFactory();

        const int maxPageSize = 10;

        public ClientsController()
        {
            _repository = new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext());
        }

        public ClientsController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
        }
         

        [Route("clients", Name = "Clients")]
        public IHttpActionResult Get(string fields = null, string sort = "description"
            , int page = 1, int pageSize = maxPageSize)
        {
            try
            {

                List<string> lstOfFields = new List<string>();

                if (fields != null)
                {
                    lstOfFields = fields.ToLower().Split(',').ToList();
                }

                var clients = _repository.GetClients();

                // ensure the page size isn't larger than the maximum.
                if (pageSize > maxPageSize)
                {
                    pageSize = maxPageSize;
                }

                // calculate data for metadata
                var totalCount = clients.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var urlHelper = new UrlHelper(Request);

                var prevLink = page > 1 ? urlHelper.Link("Clients",
                    new { page = page - 1, pageSize = pageSize, 
                        fields = fields,
                        sort = sort }) : "";
                var nextLink = page < totalPages ? urlHelper.Link("Clients",
                    new { page = page + 1, pageSize = pageSize,
                        fields = fields,
                        sort = sort }) : "";


                var paginationHeader = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    previousPageLink = prevLink,
                    nextPageLink = nextLink
                };

                HttpContext.Current.Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

                var clientsResult = clients
                    .ApplySort(sort)
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize)
                    .ToList()
                    .Select(exp => _clientFactory.CreateDataShapedObject(exp, lstOfFields));
                

                return Ok(clientsResult);

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [VersionedRoute("clients/{id}", 1)]
        public IHttpActionResult Get(int id, string fields = null)
        {
            try
            {
                List<string> lstOfFields = new List<string>();

                if (fields != null)
                {
                    lstOfFields = fields.ToLower().Split(',').ToList();
                }

                Repository.Entities.Client client = null;

                client = _repository.GetClient(id);
                if (client != null)
                {
                    var returnValue = _clientFactory.CreateDataShapedObject(client, lstOfFields);
                    return Ok(returnValue);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        //[VersionedRoute("expensegroups/{expenseGroupId}/expenses/{id}", 2)]
        //[VersionedRoute("expenses/{id}", 2)]
        //public IHttpActionResult GetV2(int id, int? expenseGroupId = null, string fields = null)
        //{
        //    try
        //    {
        //        List<string> lstOfFields = new List<string>();

            //        if (fields != null)
            //        {
            //            lstOfFields = fields.ToLower().Split(',').ToList();
            //        }

            //        Repository.Entities.Expense expense = null;

            //        if (expenseGroupId == null)
            //        {
            //            expense = _repository.GetExpense(id);
            //        }
            //        else
            //        {
            //            var expensesForGroup = _repository.GetExpenses((int)expenseGroupId);

            //            // if the group doesn't exist, we shouldn't try to get the expenses
            //            if (expensesForGroup != null)
            //            {
            //                expense = expensesForGroup.FirstOrDefault(eg => eg.Id == id);
            //            }
            //        }

            //        if (expense != null)
            //        {
            //            var returnValue = _expenseFactory.CreateDataShapedObject(expense, lstOfFields);
            //            return Ok(returnValue);
            //        }
            //        else
            //        {
            //            return NotFound();
            //        }

            //    }
            //    catch (Exception)
            //    {
            //        return InternalServerError();
            //    }
            //}

            //[Route("expenses/{id}")]
            //public IHttpActionResult Delete(int id)
            //{
            //    try
            //    {

            //        var result = _repository.DeleteExpense(id);

            //        if (result.Status == RepositoryActionStatus.Deleted)
            //        {
            //            return StatusCode(HttpStatusCode.NoContent);
            //        }
            //        else if (result.Status == RepositoryActionStatus.NotFound)
            //        {
            //            return NotFound();
            //        }

            //        return BadRequest();
            //    }
            //    catch (Exception)
            //    {
            //        return InternalServerError();
            //    }
            //}

        [Route("clients")]
        public IHttpActionResult Post([FromBody]DTO.Client client)
        {
            try
            {
                if (client == null)
                {
                    return BadRequest();
                }

                // map
                var exp = _clientFactory.CreateClient(client);

                var result = _repository.InsertClient(exp);
                if (result.Status == RepositoryActionStatus.Created)
                {
                    // map to dto
                    var newClient = _clientFactory.CreateClient(result.Entity);
                    return Created<DTO.Client>(Request.RequestUri + "/" + newClient.Id.ToString(), newClient);
                }

                return BadRequest();

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        //[Route("expenses/{id}")]
        //public IHttpActionResult Put(int id, [FromBody]DTO.Expense expense)
        //{
        //    try
        //    {
        //        if (expense == null)
        //        {
        //            return BadRequest();
        //        }

        //        // map
        //        var exp = _expenseFactory.CreateExpense(expense);

        //        var result = _repository.UpdateExpense(exp);
        //        if (result.Status == RepositoryActionStatus.Updated)
        //        {
        //            // map to dto
        //            var updatedExpense = _expenseFactory.CreateExpense(result.Entity);
        //            return Ok(updatedExpense);
        //        }
        //        else if (result.Status == RepositoryActionStatus.NotFound)
        //        {
        //            return NotFound();
        //        }

        //        return BadRequest();
        //    }
        //    catch (Exception)
        //    {
        //        return InternalServerError();
        //    }
        //}


        [Route("clients/{id}")]
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody]JsonPatchDocument<DTO.Client> clientPatchDocument)
        {
            try
            {
                // find 
                if (clientPatchDocument == null)
                {
                    return BadRequest();
                }

                var client = _repository.GetClient(id);
                if (client == null)
                {
                    return NotFound();
                }

                //// map
                var exp = _clientFactory.CreateClient(client);

                // apply changes to the DTO
                clientPatchDocument.ApplyTo(exp);

                // map the DTO with applied changes to the entity, & update
                var result = _repository.UpdateClient(_clientFactory.CreateClient(exp));

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    var updatedClient = _clientFactory.CreateClient(result.Entity);
                    return Ok(updatedClient);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }



    }
}