using SC81.Sandbox.TelligentSDK.Client;
using System.Web.Mvc;

namespace SC81.Sandbox.Controllers
{
    [Route("community")]
    public class GroupsController : Controller
    {

        private UserClient userClient;
        private GroupClient groupClient;

        public GroupsController()
        {
            userClient = new UserClient();
            groupClient = new GroupClient();
        }

        // GET: Groups
        public ActionResult Index()
        {

            var data = userClient.GetUsers();

            return View();
        }

        public JsonResult List()
        {
            var data = userClient.GetUsers();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UserId(string userName)
        {

            var userId = userClient.GetUserId(userName);

            if (userId != null)
            {
                groupClient.GetUserGroups(userId);
                groupClient.GetGroupsAvailable();
            }

            return Json(userId, JsonRequestBehavior.AllowGet);
        }

    }
}