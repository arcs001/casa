using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;




namespace AxisBIWebAPI
{
    public class Hours
    {
        public string url = "https://axisbi.VisualStudio.com/DefaultCollection/";
        
        public string TeamProject = "TJSP";
        public WorkItem CreateTask(string ross, DateTime date, decimal hours, string user, int idParent)
        {
            WorkItem wi = new WorkItem(ross, date, hours, user);
            WorkItem ret = JsonConvert.DeserializeObject<WorkItem>(WebClient.WorkItemPatch(string.Concat(url, TeamProject, "/_apis/wit/workitems/$task?api-version=1.0"), wi).Result);

            DoneTask(ret.id);
            AddLink(ret.id, idParent);

            return ret;

        }


        public void AddLink(int IdChild, int IdParent)
        {

        }

        public decimal DebitarHora(string ross, DateTime date, decimal hours, string user)
        {
            WorkItem wi = FindTaks(ross, user);
            int id = wi.id;
            decimal remainingwork = wi.GetRemainingWork();
            CreateTask(ross, date, hours, user,id);
            if (remainingwork <= hours)
            {
                DoneTask(id);
                return hours - remainingwork;
            }
            else
            {
                UpdateRemainigWork(id, remainingwork - hours);
                return hours;
            }
            

        }

        public  WorkItem FindTaks(string ross, object user)
        {
            string WIQ = string.Format("{{\"query\": \"Select [System.WorkItemType],[System.Title],[System.State],[System.Id], [Microsoft.VSTS.Scheduling.RemainingWork] FROM WorkItems WHERE [System.AssignedTo] = '{0}' AND [System.State] = 'To Do'  AND  [System.TeamProject] = @project \"}}", user);

            var json = WebClient.WorkItemPost(string.Format("{0}{1}/_apis/wit/wiql?api-version=1.0", url, TeamProject), WIQ).Result;
            var Objeto = new { WorkItems = new WorkItem[0] };
            var wis = JsonConvert.DeserializeAnonymousType(json, Objeto);
            if(wis.WorkItems.Count() > 0)
            {
                return  JsonConvert.DeserializeObject<WorkItem>(WebClient.WorkItemGet( wis.WorkItems[0].url).Result) ;
            }


            throw new NotImplementedException();
        }

        public string UpdateRemainigWork(int id, decimal RemainingWork)
        {
            WorkItem wi = new WorkItem(RemainingWork);
            return WebClient.WorkItemPatch(string.Format("{0}/_apis/wit/workitems/{1}?api-version=1.0",url,id.ToString()), wi).Result;           
            
        }

        public WorkItem DoneTask(int id)
        {
            InProgressTask(id);
            var wi = new WorkItem() { id = id, fieldsOut = new List<Field>() { new Field() { op = "add", path = "/fields/System.State", value = "Done" } } };
            return JsonConvert.DeserializeObject<WorkItem>(WebClient.WorkItemPatch(string.Format("{0}/_apis/wit/workitems/{1}?api-version=1.0", url, id.ToString()), wi).Result);
        }


        public WorkItem ChangeStatusTask(int id, string state)
        {
            var wi = new WorkItem() { id = id, fieldsOut = new List<Field>() { new Field() { op = "add", path = "/fields/System.State", value = state } } };
            return JsonConvert.DeserializeObject<WorkItem>(WebClient.WorkItemPatch(string.Format("{0}/_apis/wit/workitems/{1}?api-version=1.0", url, id.ToString()), wi).Result);
        }

        private WorkItem InProgressTask(int id)
        {
            WorkItem wi = new WorkItem() { id = id, fieldsOut = new List<Field>() { new Field() { op = "add", path = "/fields/System.State", value = "In Progress" } } };
            JsonConvert.DeserializeObject<WorkItem>(WebClient.WorkItemPatch(string.Format("{0}/_apis/wit/workitems/{1}?api-version=1.0", url, id.ToString()), wi).Result);
            return wi;
        }


        public  bool HorasNaoLogadas(string ross, DateTime date)
        {
            string WIQ = string.Format("{{\"query\": \"Select [System.WorkItemType],[System.Title],[System.State],[System.Id] FROM WorkItems WHERE [System.Title] = '{0} - {1}'   AND  [System.TeamProject] = @project \"}}", ross, date.ToString("dd/MM/yy"));

            var json = WebClient.WorkItemPost(string.Format("{0}{1}/_apis/wit/wiql?api-version=1.0",url,TeamProject), WIQ).Result;
            var Objeto = new { WorkItems = new object[0] };
             var wis = JsonConvert.DeserializeAnonymousType(json, Objeto);
            return wis.WorkItems.Count() == 0; ;
        }

        public void LogarHoras(string Ross, DateTime Date, decimal Hours, string user)
        {
            if (HorasNaoLogadas(Ross, Date))
            {
                while (Hours > 0)
                {
                    Hours = Hours - DebitarHora(Ross, Date, Hours, user);
                }
                
            }
        }



        
    }

    
}
