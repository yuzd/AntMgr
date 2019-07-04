using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Newtonsoft.Json;

namespace ant.mgr.core.Filter
{
    public class JsonNetBinder: IModelBinder
    {
        private readonly IHttpContextAccessor HttpContextAccessor;

        public JsonNetBinder(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            
            var bodyText = HttpContextAccessor.HttpContext.Request.Form["__JsonRequest"].FirstOrDefault();
            if (!string.IsNullOrEmpty(bodyText))
            {
                var jsonData = JsonConvert.DeserializeObject(bodyText, bindingContext.ModelType, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                bindingContext.Result = ModelBindingResult.Success(jsonData);
            }
            return Task.CompletedTask;
        }
    }

}
