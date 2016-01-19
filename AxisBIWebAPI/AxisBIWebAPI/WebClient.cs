using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AxisBIWebAPI
{
    public class WebClient
    {
        public static async Task<string> WorkItemPatch(string url, WorkItem wi)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(wi.fieldsOut), Encoding.UTF8, "application/json-patch+json");

            string ret = "";

            var username = "user";
            var password = "5goubabd7rptrjji52ljq4cup47tr6vgyleh436wldj4cpeassca";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", username, password))));


                var method = new HttpMethod("PATCH");

                var request = new HttpRequestMessage(method, url)
                {
                    Content = httpContent
                };

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    ret = await response.Content.ReadAsStringAsync();
                }

            }

            return ret;

        }


        public static async Task<string> WorkItemPost(string url, string strContent)
        {
            var httpContent = new StringContent(strContent, Encoding.UTF8, "application/json");

            string ret = "";

            var username = "user";
            var password = "5goubabd7rptrjji52ljq4cup47tr6vgyleh436wldj4cpeassca";

            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Accept.Add(
                //    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", username, password))));

                //var response = await client.GetStreamAsync(url)
                //    .ConfigureAwait(false);

                var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                ret = await response.Content.ReadAsStringAsync();


            }

            return ret;

        }


        public static async Task<string> WorkItemGet(string url)
        {
            //var httpContent = new StringContent(strContent, Encoding.UTF8, "application/json");

            string ret = "";

            var username = "user";
            var password = "5goubabd7rptrjji52ljq4cup47tr6vgyleh436wldj4cpeassca";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", username, password))));


                var method = new HttpMethod("GET");



                using (var response = await client.GetAsync(url).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    ret = await response.Content.ReadAsStringAsync();
                }

            }

            return ret;

        }

        public static async Task<string> PutAsJsonAsync(string url, WorkItem wi)
        {
            string ret = "";

            var username = "user";
            var password = "5goubabd7rptrjji52ljq4cup47tr6vgyleh436wldj4cpeassca";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", username, password))));



                using (HttpResponseMessage response = await client.PutAsJsonAsync<WorkItem>(url, wi).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    ret = await response.Content.ReadAsStringAsync();
                }

            }

            return ret;

        }
    }
}
