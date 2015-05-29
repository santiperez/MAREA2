using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MareaUnitTests
{
    public class Results
    {
        private double serializationElapsedMs;

        public double SerializationElapsedMs
        {
            get { return serializationElapsedMs; }
            set { serializationElapsedMs = value; }
        }

        private double deserializationElapsedMs;

        public double DeserializationElapsedMs
        {
            get { return deserializationElapsedMs; }
            set { deserializationElapsedMs = value; }
        }
        private double totalElapsedMs;

        public double TotalElapsedMs
        {
            get { return totalElapsedMs; }
            set { totalElapsedMs = value; }
        }
        private string type=null;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        private int length;

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

       public Results(long serializeTicks, long deserializeTicks,long clock_freq, int codifications,int length, string type)
       {
           this.length = length;
           this.type = type;
   
           this.serializationElapsedMs = (1000.0 * serializeTicks / clock_freq) / codifications;
           this.deserializationElapsedMs = (1000.0 * deserializeTicks / clock_freq) / codifications;
           this.totalElapsedMs = (1000.0 * (deserializeTicks + serializeTicks) / clock_freq) / codifications;
       }

       public string GetSummary()
       {
           string message= "Type: "+type;
           message += (" Ser.: " + serializationElapsedMs + " ms ");
           message += ("Des.: " + deserializationElapsedMs + " ms ");
           message += ("Total: " + totalElapsedMs + " ms ");
           message += ("Size: " + length + " bytes ");
           return message;

       }

       public override string ToString()
       {
           string message = "Type: " + type + "\n";
           message += ("Serialization: " + serializationElapsedMs + " ms\n");
           message += ("Deserialization: " + deserializationElapsedMs + " ms\n");
           message += ("Total: " + totalElapsedMs + " ms\n");
           message += ("Size: " + length + " bytes\n\n");
           return message;
       }
    }

    public class ResultsManager
    {
        public static Results GetResults(long serializeTicks, long deserializeTicks,long clock_freq, int codifications,int length, string type)
        {
            Results results= new Results(serializeTicks,deserializeTicks,clock_freq,codifications,length, type);
            return results;
        }
    }
}
