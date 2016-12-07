using ExpenseTracker.DTO;
using ExpenseTracker.WebClient.Helpers;
using Marvin.JsonPatch;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseTracker.WebClient.Models;
using PagedList;
using Thinktecture.IdentityModel.Mvc;


namespace ExpenseTracker.WebClient.Controllers
{
    public class ClientsController : Controller
    {
        [ResourceAuthorize("Read", "Clients")]
        public async Task<ActionResult> Index(int? page = 1)
        {

            var client = ExpenseTrackerHttpClient.GetClient();
            var model = new ExpenseGroupsViewModel();

            HttpResponseMessage egsResponse = await client.GetAsync("api/clients");

            if (egsResponse.IsSuccessStatusCode)
            {
                string egsContent = await egsResponse.Content.ReadAsStringAsync();
                var clients = JsonConvert.DeserializeObject<IEnumerable<Client>>(egsContent);
                model.Clients = clients;
            }
            else
            {
                return Content(ExpenseTrackerConstants.GeneralExceptionMessage);
            }


            HttpResponseMessage response = await client.GetAsync("api/clients?sort=description&page=" + page + "&pagesize=5");


            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                // get the paging info from the header
                var pagingInfo = HeaderParser.FindAndParsePagingInfo(response.Headers);

                var clients = JsonConvert.DeserializeObject<IEnumerable<Client>>(content);

                var clientsList = new StaticPagedList<Client>(clients, pagingInfo.CurrentPage,
                    pagingInfo.PageSize, pagingInfo.TotalCount);

                model.Clients = clientsList;
                model.PagingInfo = pagingInfo;
            }
            else
            {
                return Content(ExpenseTrackerConstants.GeneralExceptionMessage);
            }


            return View(model);

        }

        // GET: Expenses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Client client)
        {
            try
            {
                var httpClient = ExpenseTrackerHttpClient.GetClient();

                // serialize & POST
                var serializedItemToCreate = JsonConvert.SerializeObject(client);

                var response = await httpClient.PostAsync("api/clients",
                    new StringContent(serializedItemToCreate,
                        System.Text.Encoding.Unicode, "application/json"));

                if (!response.IsSuccessStatusCode) return Content(ExpenseTrackerConstants.GeneralExceptionMessage);

                return RedirectToAction("Index", "Clients");

            }
            catch
            {
                return Content(ExpenseTrackerConstants.GeneralExceptionMessage);
            }
        }

        // GET: Clients/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var client = ExpenseTrackerHttpClient.GetClient();

            HttpResponseMessage response = await client.GetAsync("api/clients/" + id);
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<Client>(content);
                return View(model);
            }

            return Content(ExpenseTrackerConstants.GeneralExceptionMessage);
        }




        // GET: Expenses/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            // get version 2
            var client = ExpenseTrackerHttpClient.GetClient();

            HttpResponseMessage response = await client.GetAsync("api/clients/" + id
                + "?fields=id,description,address1,address2,address3,postcode,adminemail,email,isactive,IsNonOrganisationlClient");

            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<Client>(content);
                return View(model);
            }

            return Content(ExpenseTrackerConstants.GeneralExceptionMessage + content);

        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Client client)
        {
            try
            {
                var httpClient = ExpenseTrackerHttpClient.GetClient();


                // create a JSON Patch Document
                JsonPatchDocument<DTO.Client> patchDoc = new JsonPatchDocument<DTO.Client>();
                patchDoc.Replace(e => e.Address1, client.Address1);
                patchDoc.Replace(e => e.Address2, client.Address2);
                patchDoc.Replace(e => e.Address3, client.Address3);
                patchDoc.Replace(e => e.Postcode, client.Postcode);
                patchDoc.Replace(e => e.Description, client.Description);
                patchDoc.Replace(e => e.Email, client.Email);
                patchDoc.Replace(e => e.AdminEmail, client.AdminEmail);
                patchDoc.Replace(e => e.IsActive, client.IsActive);

                // serialize and PATCH
                var serializedItemToUpdate = JsonConvert.SerializeObject(patchDoc);

                var response = await httpClient.PatchAsync("api/clients/" + id,
                    new StringContent(serializedItemToUpdate,
                    System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "Clients", new { id = client.Id });
                }
                else
                {
                    return Content(ExpenseTrackerConstants.GeneralExceptionMessage);
                }

            }
            catch
            {
                return Content(ExpenseTrackerConstants.GeneralExceptionMessage);
            }
        }

        public ActionResult ManageImages()
        {
            return View();
        }
        //// GET: Expenses/Delete/5
        //public async Task<ActionResult> Delete(int expenseGroupId, int id)
        //{
        //    try
        //    {
        //        var client = ExpenseTrackerHttpClient.GetClient();

        //        var response = await client.DeleteAsync("api/expenses/" + id);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return RedirectToAction("Details", "ExpenseGroups", new { id = expenseGroupId });
        //        }
        //        else
        //        {
        //            return Content("An error occurred");
        //        }

        //    }
        //    catch
        //    {
        //        return Content("An error occurred");
        //    }
        //}

    }
}
