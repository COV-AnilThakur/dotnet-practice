using System.Diagnostics;
using Ecommerce_DBFirst.Models;
using Ecommerce_DBFirst.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_DBFirst.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISingletonOperation _singletonOperation;
        private readonly IScopedOperation _scopedOperation;
        private readonly ITransientOperation _transientOperation;

        public HomeController(
            ILogger<HomeController> logger,
            ISingletonOperation singletonOperation,
            IScopedOperation scopedOperation,
            ITransientOperation transientOperation)
        {
            _logger = logger;
            _singletonOperation = singletonOperation;
            _scopedOperation = scopedOperation;
            _transientOperation = transientOperation;
        }

        public IActionResult Index()
        {
            _logger.LogInformation(
                "Lifetime demo values - Singleton: {SingletonId}, Scoped: {ScopedId}, Transient: {TransientId}",
                _singletonOperation.OperationId,
                _scopedOperation.OperationId,
                _transientOperation.OperationId);

            ViewData["SingletonOperationId"] = _singletonOperation.OperationId;
            ViewData["ScopedOperationId"] = _scopedOperation.OperationId;
            ViewData["TransientOperationId"] = _transientOperation.OperationId;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("Error action executed. RequestId: {RequestId}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
