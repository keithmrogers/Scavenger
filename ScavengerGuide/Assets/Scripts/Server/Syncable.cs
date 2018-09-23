using System;
using System.Runtime.Serialization;

namespace Scavenger.Server
{
    [KnownType(typeof(Position))]
    [KnownType(typeof(InfoOnlyMessage))]
    [KnownType(typeof(Connect))]
    public class Syncable
    {

    }

    public class InfoOnlyMessage : Syncable
    {
        public string Message { get; set; }
    }

    public class Position : Syncable
    {
        public Position()
        {

        }
        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return string.Format("Position (X={0},Y={1})", X.ToString(), Y.ToString());
        }
    }

    public enum ClientTypes
    {
        Guide = 1,
        Scavenger = 2
    }

    public class Connect : Syncable
    {
        public ClientTypes ClientType { get; set; }
        public string UserName { get; set; }
        public Guid Id { get; set; }
    }

    public class Disconnect : Syncable
    {
        public string UserName { get; set; }
    }
}