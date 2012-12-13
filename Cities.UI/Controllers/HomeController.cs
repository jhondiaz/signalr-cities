using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Cities.Data;
using Newtonsoft.Json;
using SignalR;

namespace Cities.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.States = Database.Context.States;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCities(string term, int? stateid, int start, int count)
        {
            var query = from c in Database.Context.Cities.OrderBy(c => c.CityId)
                        where (string.IsNullOrEmpty(term) || c.Description.Contains(term)) && (stateid == null || c.StateId == stateid.Value)
                        select new
                        {
                            id = c.CityId,
                            name = c.Description,
                            stateid = c.StateId,
                            state = c.State.Description,
                        };

            var cities = query.Skip(start).Take(count).ToList();

            var result = new 
            {
                cities = cities,
                count = cities.Count(),
                total = query.Count(),
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public int SaveCity(City city)
        {
            string action;

            if (city.CityId == 0)
            {
                action = "INSERT";

                Database.Context.Cities.AddObject(city);
            }
            else
            {
                var entity = Database.Context.Cities.First(c => c.CityId == city.CityId);

                action = "UPDATE";
                entity.Description = city.Description;
                entity.StateId = city.StateId;
            }

            Database.Context.SaveChanges();
            Call(city, action);

            return city.CityId;
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        public void DeleteCity(int id)
        {
            var city = Database.Context.Cities.First(c => c.CityId == id);
            
            Database.Context.DeleteObject(city);
            Database.Context.SaveChanges();

            Call(city, "DELETE");
        }

        private void Call(City city, string action)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;

                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("cityid");
                jsonWriter.WriteValue(city.CityId);
                jsonWriter.WritePropertyName("name");
                jsonWriter.WriteValue(city.Description);
                jsonWriter.WritePropertyName("stateid");
                jsonWriter.WriteValue(city.StateId);
                jsonWriter.WritePropertyName("action");
                jsonWriter.WriteValue(action);
                jsonWriter.WriteEndObject();
            }

            var context = GlobalHost.ConnectionManager.GetConnectionContext<SignalConnection>();
            context.Connection.Broadcast(sb.ToString());
        }
    }
}
