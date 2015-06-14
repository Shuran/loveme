using System;
using System.Threading.Tasks;
using System.Json;
using System.Net;
using System.IO;
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace meloveShared.BL
{
	public class WebConnectUtility
	{
		public static MobileServiceClient mMobileService = new MobileServiceClient
		(
				Constants.mServerUrl,
				Constants.mApplicationKey
		);

		public WebConnectUtility ()
		{
			
		}

		public async Task<JsonValue> WebRestGet(string pUrl)
		{
			// How to call a rest web service: http://developer.xamarin.com/recipes/android/web_services/consuming_services/call_a_rest_web_service/
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (pUrl));
			request.ContentType = "application/json";
			request.Method = "GET";
			try
			{
				//'using' statement for automatic and safe GC
				//http://www.codeproject.com/Articles/6564/Understanding-the-using-statement-in-C
				//Dispose
				//http://forums.asp.net/t/1879665.aspx?type+used+in+using+statement+must+be+implicitly+convertible+to+System+IDisposable
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream()) 
					{
						JsonValue jsonDoc = await Task.Run (()=>JsonObject.Load(stream));
						return jsonDoc;
					}
				}
			}
			catch(HttpListenerException e)
			{
				Console.WriteLine (e.Message);
				return null;
			}
		}

		//TODO-working: Make the input parameters and returned data more specific to the defined type
		public async Task<JObject> WebAzurePost(string pWebApi, WebObject pRequest)
		{
			try
			{
				JObject pResult = await mMobileService.InvokeApiAsync<WebObject,JObject>(pWebApi,pRequest);
				//This will be called immediately after executing the callback function in higher layers
				return pResult;
			}
			catch(MobileServiceInvalidOperationException e) 
			{
				Console.WriteLine (e.Message);
				return null;
			}
		}

	}
}

