using UnityEngine;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;

public class HTTP_order_receiver : MonoBehaviour
{
    private HttpListener listener;
    private Thread thread;

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://+:5000/order/");
        listener.Start();

        thread = new Thread(HandleRequests);
        thread.Start();

        Debug.Log("HTTP Server started on port 5000");
    }

    void HandleRequests()
    {
        while (true)
        {
            var context = listener.GetContext();
            var request = context.Request;

            // CORS preflight
            if (request.HttpMethod == "OPTIONS")
            {
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                context.Response.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
                context.Response.StatusCode = 200;
                context.Response.Close();
                continue;
            }

            if (request.HttpMethod == "POST")
            {
                using var reader = new StreamReader(request.InputStream);
                string body = reader.ReadToEnd();

                Debug.Log("Received order: " + body);

                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                byte[] responseBytes = Encoding.UTF8.GetBytes("OK");
                context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                context.Response.Close();
            }
        }
    }

    void OnApplicationQuit()
    {
        listener.Stop();
        thread.Abort();
    }
}
