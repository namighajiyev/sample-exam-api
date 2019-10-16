using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SampleExam.Infrastructure.Conventions
{
    public class AppControllerModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.Attributes.OfType<RouteAttribute>().FirstOrDefault();
            var apiVersion = controllerNamespace?.Template?.Split('/')?.First()?.ToLower() ?? "default";

            var methods = controller.Actions.Select(a => a.Attributes.OfType<HttpMethodAttribute>()
            .FirstOrDefault() ?? new HttpGetAttribute()).Select(a => a.HttpMethods.First());

            var groups = methods.GroupBy(e => e,
             (k, e) => $"{k.ToLower()}:{e.Count()}"
            );
            var suffix = string.Join(' ', groups.ToArray());

            apiVersion = $"{apiVersion} ({suffix})";
            controller.ApiExplorer.GroupName = apiVersion;
        }
    }
}