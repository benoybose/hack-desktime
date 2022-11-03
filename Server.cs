using System;
using System.Net;

namespace DeskTime
{
    public class Server : IEquatable<Server>
    {
        public int Id { get; set; }

        public string Host { get; set; }

        public string Region { get; set; }

        public HttpStatusCode Status { get; set; } = HttpStatusCode.BadRequest;


        public string Error { get; set; } = string.Empty;


        public string Response { get; set; } = string.Empty;


        public long Time { get; set; } = -1L;


        public override string ToString()
        {
            return "Id: " + Id + ", Host: " + Host + ", Region: " + Region + ", Status: " + Status.ToString() + ", Time: " + Time + ", Error: " + Error + ", Response: " + Response;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Server server = obj as Server;
            if (server == null)
            {
                return false;
            }
            return Equals(server);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public bool Equals(Server other)
        {
            if (other == null)
            {
                return false;
            }
            return Host.Equals(other.Host);
        }
    }
}
