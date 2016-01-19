using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace AxisBIWebAPI.Controllers
{
    public class RossController : ApiController
    {

        public void Post([FromBody]input values)
        {
            new AxisBIWebAPI.Hours().LogarHoras(values.Ross, values.Date, values.Hours, values.user);
        }



        [HttpPut]
        public void Put(string Ross, DateTime Date, decimal Hours, string user)
        {
            new AxisBIWebAPI.Hours().LogarHoras(Ross, Date, Hours, user);
        }

        public string Get()
        {
            //CallAPI("https://axisbi.VisualStudio.com/DefaultCollection/_apis/projects?api-version=2.0");
            return "bingo";
        }

    }


    public class input
    {
        public string Ross { get; set; }
        public string user { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }

    }
}