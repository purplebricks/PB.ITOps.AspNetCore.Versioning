using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PB.ITOps.AspNetCore.Versioning;

namespace ApiVersioningExample.Controllers
{
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [ApiController]
    [IntroducedInApiVersion(2)]
    public class AnotherController : ControllerBase
    {
        // GET api/v2/another
        // GET api/v3/another
        /// <summary>This action is not in V1, deprecated in V2, supported in V3</summary>
        /// <remarks>
        /// This was introduced in V2 via the `IntroducedInApiVersion(2)` attribute on the controller.
        /// This has not been removed.
        /// </remarks>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2"};
        }

        // GET api/v2/another/5
        /// <summary>This action is not in V1, deprecated in V2, not in V3</summary>
        /// <remarks>
        /// This was introduced in V2 via the `IntroducedInApiVersion(2)` attribute on the controller.
        /// This has been removed as of Api V3 via the `RemovedAsOfApiVersion(3)` attribute on the action.
        /// </remarks>
        [HttpGet("{id}")]
        [RemovedAsOfApiVersion(3)]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
        
        // GET api/v3/another/123abc
        /// <summary>This action is not in V1, not in V2, supported in V3</summary>
        /// <remarks>
        /// Although the controller was introduced in V2 via the `IntroducedInApiVersion(2)` attribute,
        /// the `IntroducedInApiVersion(3)` attribute on the action overrides this.
        /// As such was introduced in V3.
        /// </remarks>
        [HttpGet("{id}")]
        [IntroducedInApiVersion(3)]
        public ActionResult<string> Get(string id)
        {
            return "value";
        }
    }
}