using System.Web.Mvc;

namespace Configit.BuildRadiator.Controllers {
  [Authorize]
  public class HomeController: Controller {
    public ActionResult Index() {
      return View();
    }
  }
}