using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PB.ITOps.AspNetCore.Versioning;

namespace ApiVersioningExample.Controllers
{
    // For demonstration purposes we will use uri versioning
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [ApiController]
    [IntroducedInApiVersion(1)]
    [RemovedAsOfApiVersion(3)]
    public class ValuesController : ControllerBase
    {
        // GET api/v1/values
        // GET api/v2/values
        /// <summary>
        /// This action is deprecated in V1 and V2, not in V3</summary>
        /// <remarks>
        /// This was introduced in V1 via the `IntroducedInApiVersion(1)` attribute on the controller.
        /// This has been removed as of Api V3 via the `RemovedAsOfApiVersion(3)` attribute on the controller.
        /// </remarks>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2"};
        }
        
        // GET api/v2/values/123
        /// <summary>
        /// This action is not in v1, deprecated in v2, not in v3
        /// </summary>
        /// <remarks>
        /// Although the controller was introduced in V1 via the `IntroducedInApiVersion(1)` attribute,
        /// the `IntroducedInApiVersion(2)` attribute on the action overrides this.
        /// As such was introduced in V2.
        /// As the controller was `RemovedAsOfApiVersion(3)`, all controller actions are removed in this version.
        /// </remarks>
        [HttpGet("{id}")]
        [IntroducedInApiVersion(2)]
        public ActionResult<string> Get(int id)
        {
            return $"{id}";
        }
    }
}