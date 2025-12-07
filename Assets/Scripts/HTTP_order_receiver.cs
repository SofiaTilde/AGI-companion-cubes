using UnityEngine;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System;

public class HTTP_order_receiver : MonoBehaviour
{
    public OrderingSystem ordering_system;

    private HttpListener listener;
    private Thread thread;

    // to solve the thread bug:
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();
    private readonly object lockObj = new object();

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://+:5000/order/");
        listener.Start();

        thread = new Thread(HandleRequests);
        thread.Start();

        Debug.Log("HTTP Server started on port 5000");

    }

    void Update()
    {
        lock (lockObj)
        {
            while (mainThreadActions.Count > 0)
            {
                var action = mainThreadActions.Dequeue();
                action.Invoke();
            }
        }
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

                Debug.Log("Received HTTP order: " + body);

                ParseOrder(body);

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

    private void ParseOrder(string body)
    {
        string size = "";
        List<string> toppings = new List<string>();

        // Extract size
        int sizeIndex = body.IndexOf("\"size\"");
        if (sizeIndex != -1)
        {
            int firstQuote = body.IndexOf("\"", sizeIndex + 6) + 1;
            int secondQuote = body.IndexOf("\"", firstQuote);
            string sizeValue = body.Substring(firstQuote, secondQuote - firstQuote);

            size = sizeValue switch
            {
                "small" => "Small",
                "medium" => "Medium",
                "large" => "Big",
                _ => "Unknown"
            };
        }

        // Extract toppings
        int toppingIndex = body.IndexOf("\"topping\"");
        if (toppingIndex != -1)
        {
            int firstQuote = body.IndexOf("\"", toppingIndex + 9) + 1;
            int secondQuote = body.IndexOf("\"", firstQuote);
            string toppingsValue = body.Substring(firstQuote, secondQuote - firstQuote);

            string[] split = toppingsValue.Split(',');

            foreach (string raw in split)
            {
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                string formatted = char.ToUpper(raw[0]) + raw.Substring(1).ToLower();
                toppings.Add(formatted);
            }
        }

        //Debug.Log("Pancake: " + size);
        //Debug.Log("Toppings: " + string.Join(", ", toppings));

        lock (lockObj)
        {
            mainThreadActions.Enqueue(() =>
            {
                ordering_system.AddOrder(size, toppings);
            });
        }

    }

}
