using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using CoreBot.Clu;

namespace CoreBot.CognitiveModels;

public class MedichatModel : IRecognizerConvert
{
    public enum Intent
    {
        PlaceAppointment,
        ViewAppointments,
        ViewDoctors,
        RequestPrescription,
        ViewPrescriptions,
        None
    }

    public string Text { get; set; }

    public string AlteredText { get; set; }

    public Dictionary<Intent, IntentScore> Intents { get; set; }

    public CluEntities Entities { get; set; }

    public IDictionary<string, object> Properties { get; set; }

    public void Convert(dynamic result)
    {
        var jsonResult = JsonConvert.SerializeObject(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        var app = JsonConvert.DeserializeObject<MedichatModel>(jsonResult);

        Text = app.Text;
        AlteredText = app.AlteredText;
        Intents = app.Intents;
        Entities = app.Entities;
        Properties = app.Properties;
    }

    public (Intent intent, double score) GetTopIntent()
    {
        var maxIntent = Intent.None;
        var max = 0.0;
        foreach (var entry in Intents)
        {
            if (entry.Value.Score > max)
            {
                maxIntent = entry.Key;
                max = entry.Value.Score.Value;
            }
        }

        return (maxIntent, max);
    }

    public class CluEntities
    {
        public CluEntity[] Entities;

        public string GetName() => EntityByName("name");
        public string GetTime() => EntityByName("time");
        public string GetContact() => EntityByName("contact");
        public string GetReason() => EntityByName("reason");

        private string EntityByName(string name)
        {
            return Entities.Where(e => e.Category == name).ToArray().FirstOrDefault()?.Text;
        }
    }
}