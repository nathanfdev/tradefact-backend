using Core.Enums;
using Core.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp.Email.Common
{
    public static class EmailSender
    {
		public static IRestResponse SendSimpleMessage(BaseEmail email, string variables, string template, string subject)
		{
			RestClient client = new RestClient();
			client.BaseUrl = new Uri("https://api.eu.mailgun.net/v3/");
			client.Authenticator = new HttpBasicAuthenticator("api", "41b4b7d17c18920f27d95fb439b034a1-ee13fadb-a4f20cc9");
			RestRequest request = new RestRequest();
			request.AddParameter("domain", "mg.tradefact.com", ParameterType.UrlSegment);
			request.Resource = "{domain}/messages";
			request.AddParameter("from", "Tradefact <postmaster@mg.tradefact.com>");
			request.AddParameter("to", $"<{email.Email}>");
			request.AddParameter("subject", subject);

			request.AddParameter("template", template);

			request.AddParameter("h:X-Mailgun-Variables", variables);
			request.Method = Method.POST;
			var response = client.Execute(request);
			return response;
		}

		public static IRestResponse SendMessageWithAttachments(BaseEmail email, string variables, string template, string subject, byte[] bytes, string filename)
        {
			RestClient client = new RestClient();
			client.BaseUrl = new Uri("https://api.eu.mailgun.net/v3/");
			client.Authenticator = new HttpBasicAuthenticator("api", "41b4b7d17c18920f27d95fb439b034a1-ee13fadb-a4f20cc9");
			RestRequest request = new RestRequest();
			request.AddParameter("domain", "mg.tradefact.com", ParameterType.UrlSegment);
			request.Resource = "{domain}/messages";
			request.AddParameter("from", "Tradefact <postmaster@mg.tradefact.com>");
			request.AddParameter("to", $"<{email.Email}>");
			request.AddParameter("subject", subject);
			request.AddFileBytes("attachment", bytes, filename);

			request.AddParameter("template", template);

			request.AddParameter("h:X-Mailgun-Variables", variables);
			request.Method = Method.POST;
			var response = client.Execute(request);
			return response;
		}
	}
}
